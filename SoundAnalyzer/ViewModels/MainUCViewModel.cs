using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System;
using NAudio.Wave;
using SoundAnalyzer.Sheets;
using System.Windows.Media;
using NAudio.CoreAudioApi;

namespace SoundAnalyzer.ViewModels {
    class MainUCViewModel : ViewModelBase {
        private readonly SynchronizationContext _synchronizationContext;
        private string _selectedDevice;
        private WaveIn _waveIn;
        private float _peak;
        private string _line;
        private int _sampleRate;
        private Sheet _sheet;
        private GoertzelAlgorithm _goertzelFilter;
        private Dictionary<string, int> _keys = new Dictionary<string, int>();
        private List<NotesGroup> _notes;
        private string _path;
        private int _sensibility;
        private MMDevice _defaultDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);

        public IEnumerable<string> CaptureDevices { get; private set; }
        public List<WaveInCapabilities> CaptureDevicesMetadata { get; private set; }
        public double TargetFreaquency { get; set; }
        public double Power { get; set; }


        public string SelectedDevice {
            get { return _selectedDevice; }
            set {
                _selectedDevice = value; OnPropertyChanged("SelectedDevice");
                GetSampleRate(value); Record();
            }
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
            get { return _notes; }
            set { _notes = value; OnPropertyChanged("Notes"); }
        }

        public string Path {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); InitializeNotes(Path); }
        }


        public int Sensibility {
            get { return _sensibility; }
            set { _sensibility = value; OnPropertyChanged("Sensibility"); _goertzelFilter.Sensibility = value; }
        }

        public MainUCViewModel() {
            _synchronizationContext = SynchronizationContext.Current;
            AddAllNotes();
            InitializeNotes();
            GetRecordingDevices();
            Sensibility = _goertzelFilter.Sensibility;
        }

        private void GetRecordingDevices() {
            //var enumerator = new MMDeviceEnumerator();
            //CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            //var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            //SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
            var waveInDevices = WaveIn.DeviceCount;
            CaptureDevicesMetadata = new List<WaveInCapabilities>();
            for (var waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++) {
                var deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                CaptureDevicesMetadata.Add(deviceInfo);
            }
            CaptureDevices = CaptureDevicesMetadata.Select(d => d.ProductName);
            SelectedDevice = CaptureDevices.First();
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
            _waveIn = new WaveIn();
            var captureDevice = CaptureDevicesMetadata.Find(c => c.ProductName == SelectedDevice);
            _waveIn.DeviceNumber = CaptureDevicesMetadata.IndexOf(captureDevice);
            var myformat = new WaveFormat(48000, 16, 2);
            _waveIn.WaveFormat = myformat;
            _waveIn.DataAvailable += CaptureOnDataAvabile;
            _waveIn.StartRecording();
        }

        private void GetSampleRate(string microphoneName) {
            using (var capture = new WaveIn()) {
                var captureDevice = CaptureDevicesMetadata.Find(c => c.ProductName == microphoneName);
                capture.DeviceNumber = CaptureDevicesMetadata.IndexOf(captureDevice);
                _sampleRate = capture.WaveFormat.SampleRate;
                _goertzelFilter = new GoertzelAlgorithm(48000);
            }
        }

        private void CaptureOnDataAvabile(object sender, WaveInEventArgs e) {
            UpdatePeakMeter();
            var buffer = new List<float>();
            var value = e.Buffer;
            var bytesRecorded = e.BytesRecorded;
            var bufferIncrement = _waveIn.WaveFormat.BlockAlign;
            for (var index = 0; index < bytesRecorded; index += 2) {
                short sample = BitConverter.ToInt16(value, index);
                var sample32 = sample / (float)Int16.MaxValue;
                buffer.Add(sample32);
            }

            //if (_goertzelFilter.NotesPlayed(buffer, _sheet.GetCurentNotes())) {
            //    ChangeNotesColor();
            //    _sheet.IncreaseCounter();
            //    Line = _sheet.GetCurrentNotesAsString();
            //}

            Keys = _goertzelFilter.DetectAllNotesPlayed(buffer);
            if (_sheet.GetCurentNotes().Select(n => n.ToString()).All(key => Keys[key] == 1)) {
                ChangeNotesColor();
                _sheet.IncreaseCounter();
                Line = _sheet.GetCurrentNotesAsString();
            }
        }

        private void ChangeNotesColor() {
            //_synchronizationContext.Post(s => Notes.ForEach(n => n.Color = new SolidColorBrush(Colors.Black)), null);
            _synchronizationContext.Post(s => Notes[_sheet.GetCurentCounter()].Color = new SolidColorBrush(Colors.Green), null);
            _synchronizationContext.Post(s => OnPropertyChanged("Notes"), null);
        }

        private void UpdatePeakMeter() {
            _synchronizationContext.Post(s => Peak = _defaultDevice.AudioMeterInformation.MasterPeakValue, null);
        }
    }
}