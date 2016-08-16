using System.Linq;
using System.Threading;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System;
using NAudio.Wave;
using SoundAnalyzer.Sheets;
using System.Windows.Media;

namespace SoundAnalyzer.ViewModels {
    class MainUCViewModel : ViewModelBase {
        private readonly SynchronizationContext _synchronizationContext;
        private MMDevice _selectedDevice;
        private WasapiCapture _capture;
        private float _peak;
        private string _line;
        private int _sampleRate;
        private Sheet _sheet;
        private GoertzelAlgorithm _goertzelFilter;
        private Dictionary<string, int> _keys = new Dictionary<string, int>();
        private List<NotesGroup> notes;
        private string _path;
        private int _sensibility;

        public IEnumerable<MMDevice> CaptureDevices { get; private set; }
        public double TargetFreaquency { get; set; }
        public double Power { get; set; }


        public MMDevice SelectedDevice {
            get { return _selectedDevice; }
            set { if (_selectedDevice != value) { _selectedDevice = value; OnPropertyChanged("SelectedDevice"); GetSampleRate(value); } }
        }

        public float Peak {
            get { return _peak; }
            set { if (_peak != value) { _peak = value; OnPropertyChanged("Peak"); } }
        }

        public string Line {
            get { return _line; }
            set { _line = value; OnPropertyChanged("Line"); }
        }

        public Dictionary<string, int> Keys {
            get { return _keys; }
            set { _keys = value; OnPropertyChanged("Keys"); }
        }

        public List<NotesGroup> Notes {
            get { return notes; }
            set { notes = value; OnPropertyChanged("Notes"); }
        }

        public string Path {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); InitializeNotes(Path); }
        }


        public int Sensibility {
            get { return _sensibility; }
            set { _sensibility = value; _goertzelFilter.Sensibility = value; }
        }

        public MainUCViewModel() {
            _synchronizationContext = SynchronizationContext.Current;
            AddAllNotes();
            InitializeNotes();
            var enumerator = new MMDeviceEnumerator();
            CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
            Record();
        }

        private void InitializeNotes() {
            _sheet = new Sheet(@"D:\SoundAnalyzer\MultipleNotes.txt");
            Notes = _sheet.Notes;
            Line = _sheet.GetCurrentNotesAsString();
            ChangeNotesColor();
        }

        private void InitializeNotes(string path) {
            _sheet = new Sheet(Path);
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
            _capture = new WasapiCapture(_selectedDevice);
            WaveFormat myformat = new WaveFormat(44100, 16, 2);
            _capture.WaveFormat = myformat;
            _capture.DataAvailable += CaptureOnDataAvabile;
            _capture.StartRecording();
        }

        private void GetSampleRate(MMDevice value) {
            using (var c = new WasapiCapture(value)) {
                _sampleRate = c.WaveFormat.SampleRate;
                _goertzelFilter = new GoertzelAlgorithm(_sampleRate);
            }
        }

        private void CaptureOnDataAvabile(object sender, WaveInEventArgs e) {
            UpdatePeakMeter();
            var buffer = new List<float>();
            for (int index = 0; index < e.BytesRecorded; index += 2) {
                short sample = BitConverter.ToInt16(e.Buffer, index);
                float sample32 = sample / (float)Int16.MaxValue;
                buffer.Add(sample32);
            }

            if (_goertzelFilter.NotesPlayed(buffer, _sheet.GetCurentNotes())) {
                ChangeNotesColor();
                _sheet.IncreaseCounter();
                Line = _sheet.GetCurrentNotesAsString();
            }
        }

        private void ChangeNotesColor() {
            _synchronizationContext.Post(s => Notes.ForEach(n => n.Color = new SolidColorBrush(Colors.Black)), null);
            _synchronizationContext.Post(s => Notes[_sheet.GetCurentCounter()].Color = new SolidColorBrush(Colors.Green), null);
            _synchronizationContext.Post(s => OnPropertyChanged("Notes"), null);
        }

        private void UpdatePeakMeter() {
            _synchronizationContext.Post(s => Peak = SelectedDevice.AudioMeterInformation.MasterPeakValue, null);
        }
    }
}
