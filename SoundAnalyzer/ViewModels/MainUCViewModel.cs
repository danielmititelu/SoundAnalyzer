using System.Linq;
using System.Threading;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System;
using NAudio.Wave;
using SoundAnalyzer.Sheets;

namespace SoundAnalyzer.ViewModels {
    class MainUCViewModel : ViewModelBase {
        private readonly SynchronizationContext synchronizationContext;
        private MMDevice selectedDevice;
        private WasapiCapture capture;
        private float peak;
        private string line;
        private string noteList;
        private int sampleRate;
        private Sheet sheet;
        private GoertzelAlgorithm goertzelFilter;
        private int noteA4;

        public IEnumerable<MMDevice> CaptureDevices { get; private set; }
        public double TargetFreaquency { get; set; }
        public double Power { get; set; }

        public MMDevice SelectedDevice {
            get { return selectedDevice; }
            set { if (selectedDevice != value) { selectedDevice = value; OnPropertyChanged("SelectedDevice"); GetSampleRate(value); } }
        }

        public float Peak { get { return peak; } set { if (peak != value) { peak = value; OnPropertyChanged("Peak"); } } }
        public string Line { get { return line; } set { line = value; OnPropertyChanged("Line"); } }
        public string NoteList { get { return noteList; } set { noteList = value; OnPropertyChanged("NoteList"); } }
        public int NoteA4 { get { return noteA4; } set { noteA4 = value; OnPropertyChanged("NoteA4"); } }
        public Dictionary<string, int> notes = new Dictionary<string, int>();
        public Dictionary<string, int> Notes { get { return notes; } set { notes = value; OnPropertyChanged("Notes"); } }
        public MainUCViewModel() {
            AddAllNotes();
            synchronizationContext = SynchronizationContext.Current;
            var enumerator = new MMDeviceEnumerator();
            CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
            SetMelody();
            Record();
        }

        private void AddAllNotes() {
            var noteRepository = new NoteRepository();
            for (int i = 1; i <= 88; i++) {
                var key = noteRepository.GetNote(i);
                Notes.Add(key.Name + key.Octave, 0);
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

            Notes = goertzelFilter.DetectAllNotesPlayed(floatBuffer);

            if (goertzelFilter.NotePlayed(floatBuffer, TargetFreaquency)) {
                NoteList += " " + sheet.CurrentNoteName();
                TargetFreaquency = sheet.NextNoteFreaquency();
                if (TargetFreaquency == 0) Reset();
            }
        }

        void UpdatePeakMeter() {
            // can't access this on a different thread from the one it was created on, so get back to GUI thread
            synchronizationContext.Post(s => Peak = SelectedDevice.AudioMeterInformation.MasterPeakValue, null);
        }

        private void Reset() {
            NoteList = "";
            sheet.ResetCounter();
            TargetFreaquency = sheet.FirstNoteFreaquency();
        }

        private void SetMelody() {
            Line = "C4 D4 E4 F4";
            sheet = new Sheet(Line);
            TargetFreaquency = sheet.FirstNoteFreaquency();
        }
    }
}
