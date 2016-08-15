using System.Linq;
using System.Threading;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System;
using NAudio.Wave;
using SoundAnalyzer.Sheets;
using System.Windows.Media;
using System.Windows;

namespace SoundAnalyzer.ViewModels {
    class MainUCViewModel : ViewModelBase {
        private readonly SynchronizationContext synchronizationContext;
        private MMDevice selectedDevice;
        private WasapiCapture capture;
        private float peak;
        private string line;
        private string noteList;
        private int sampleRate;
        private Sheet _sheet;
        private GoertzelAlgorithm goertzelFilter;
        private int noteA4;
        public Dictionary<string, int> key = new Dictionary<string, int>();
        private List<NotesGroup> notes;

        public IEnumerable<MMDevice> CaptureDevices { get; private set; }
        public double TargetFreaquency { get; set; }
        public double Power { get; set; }

        public MMDevice SelectedDevice {
            get { return selectedDevice; }
            set { if (selectedDevice != value) { selectedDevice = value; OnPropertyChanged("SelectedDevice"); GetSampleRate(value); } }
        }

        public float Peak {
            get { return peak; }
            set { if (peak != value) { peak = value; OnPropertyChanged("Peak"); } }
        }

        public string Line {
            get { return line; }
            set { line = value; OnPropertyChanged("Line"); }
        }

        public string NoteList {
            get { return noteList; }
            set { noteList = value; OnPropertyChanged("NoteList"); }
        }

        public int NoteA4 {
            get { return noteA4; }
            set { noteA4 = value; OnPropertyChanged("NoteA4"); }
        }

        public Dictionary<string, int> Keys {
            get { return key; }
            set { key = value; OnPropertyChanged("Keys"); }
        }


        public List<NotesGroup> Notes {
            get { return notes; }
            set { notes = value; OnPropertyChanged("Notes"); }
        }

        public MainUCViewModel() {
            synchronizationContext = SynchronizationContext.Current;
            AddAllNotes();
            InitializeNotes();
            var enumerator = new MMDeviceEnumerator();
            CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
            Record();

        }

        private void InitializeNotes() {
            _sheet = new Sheet(@"D:\SoundAnalyzer\Exercise.txt");
            Notes = _sheet.Notes;
            Line = _sheet.GetCurrentNotesAsString();
            ChangeNotesColor();
        }

        private void AddAllNotes() {
            var noteRepository = new NoteRepository();
            for (int i = 1; i <= 88; i++) {
                var key = noteRepository.GetNote(i);
                Keys.Add(key.Name + key.Octave, 0);
            }
        }

        public void Record() {
            capture = new WasapiCapture(selectedDevice);
            WaveFormat myformat = new WaveFormat(44100, 16, 2);
            capture.WaveFormat = myformat;
            capture.DataAvailable += CaptureOnDataAvabile;
            capture.StartRecording();
        }

        private void GetSampleRate(MMDevice value) {
            using (var c = new WasapiCapture(value)) {
                sampleRate = c.WaveFormat.SampleRate;
                goertzelFilter = new GoertzelAlgorithm(sampleRate);
            }
        }

        private void CaptureOnDataAvabile(object sender, WaveInEventArgs e) {
            UpdatePeakMeter();
            var floatBuffer = new List<float>();
            for (int index = 0; index < e.BytesRecorded; index += 2) {
                short sample = BitConverter.ToInt16(e.Buffer, index);
                float sample32 = sample / (float)Int16.MaxValue;
                floatBuffer.Add(sample32);
            }

            Keys = goertzelFilter.DetectAllNotesPlayed(floatBuffer);
            if (_sheet.GetCurentNotes().Select(n => n.ToString()).All(key => Keys[key] == 1)) {
                ChangeNotesColor();
                _sheet.IncreaseCounter();
                Line = _sheet.GetCurrentNotesAsString();
            }
        }

        private void ChangeNotesColor() {
            synchronizationContext.Post(s => Notes.ForEach(n => n.Color = new SolidColorBrush(Colors.Black)), null);
            synchronizationContext.Post(s => Notes[_sheet.GetCurentCounter()].Color = new SolidColorBrush(Colors.Green), null);
            synchronizationContext.Post(s => OnPropertyChanged("Notes"), null);
        }

        void UpdatePeakMeter() {
            synchronizationContext.Post(s => Peak = SelectedDevice.AudioMeterInformation.MasterPeakValue, null);
        }
    }
}
