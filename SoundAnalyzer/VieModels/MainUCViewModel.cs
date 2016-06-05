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

        public IEnumerable<MMDevice> CaptureDevices { get; private set; }
        public double TargetFreaquency { get; set; }
        public double Power { get; set; }


        public MMDevice SelectedDevice {
            get { return selectedDevice; }
            set {
                if (selectedDevice != value) {
                    selectedDevice = value;
                    OnPropertyChanged("SelectedDevice");
                    GetSampleRate(value);
                }
            }
        }

        public float Peak {
            get { return peak; }
            set {
                if (peak != value) {
                    peak = value;
                    OnPropertyChanged("Peak");
                }
            }
        }

        public string Line {
            get { return line; }
            set {
                line = value;
                OnPropertyChanged("Line");
            }
        }

        public string NoteList {
            get { return noteList; }
            set {
                noteList = value;
                OnPropertyChanged("NoteList");
            }
        }

        public MainUCViewModel() {
            synchronizationContext = SynchronizationContext.Current;
            var enumerator = new MMDeviceEnumerator();
            CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
            SetMelody();
            Record();
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

            if (NotePlayed(floatBuffer.ToArray())) {
                NoteList += " " + sheet.CurrentNoteName()+" "+Power;
                TargetFreaquency = sheet.NextNoteFreaquency();
                if (TargetFreaquency == 0) Reset();
            }
        }

        private bool NotePlayed(float[] buffer) {
            double power = GoertzelFilter(buffer, TargetFreaquency, buffer.Length);
            if (power > 80) return true;
            return false;
        }

        private double GoertzelFilter(float[] samples, double targetFreaquency, int end) {
            double sPrev = 0.0;
            double sPrev2 = 0.0;
            int i;
            double normalizedfreq = targetFreaquency / sampleRate;
            double coeff = 2 * Math.Cos(2 * Math.PI * normalizedfreq);
            for (i = 0; i < end; i++) {
                double s = samples[i] + coeff * sPrev - sPrev2;
                sPrev2 = sPrev;
                sPrev = s;
            }
            double power = sPrev2 * sPrev2 + sPrev * sPrev - coeff * sPrev * sPrev2;
            Power = power;
            OnPropertyChanged("Power");
            return power;
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
