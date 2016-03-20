using System.Linq;
using System.Threading;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System;
using NAudio.Wave;
using NAudio.Dsp;

namespace SoundAnalyzer {
    class MainUCViewModel : ViewModelBase {
        private readonly SynchronizationContext synchronizationContext;
        private MMDevice selectedDevice;
        private WasapiCapture capture;
        private float peak;
        private string freaq;
        private FftBuffer fftBuffer;
        private int fftLength = 8192;

        public IEnumerable<MMDevice> CaptureDevices { get; private set; }

        public MMDevice SelectedDevice {
            get { return selectedDevice; }
            set {
                if (selectedDevice != value) {
                    selectedDevice = value;
                    OnPropertyChanged("SelectedDevice");
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

        public string Freaquency {
            get { return freaq; }
            set {
                freaq = value;
                OnPropertyChanged("Freaquency");
            }
        }

        public MainUCViewModel() {
            synchronizationContext = SynchronizationContext.Current;
            var enumerator = new MMDeviceEnumerator();
            CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
            fftBuffer = new FftBuffer(fftLength);
            fftBuffer.ThresholdReached += FftBuffer_ThresholdReached;
        }

        private void FftBuffer_ThresholdReached(object sender, EventArgs e) {
            if (peak > 0.2) { CalculateFreaquency(fftBuffer.retrieveArray(fftLength)); }

        }

        private void CalculateFreaquency(Complex[] complex) {
            List<double> buffer = new List<double>();
            for (int i = 1; i < complex.Length / 2; i += 1) {
                buffer.Add(Math.Sqrt(complex[i].X * complex[i].X + complex[i].Y * complex[i].Y));
            }

            double freaquency;
            freaquency = buffer.IndexOf(buffer.Max()) * 44100 / complex.Length;
            //synchronizationContext.Post(s => Freaquency = "" + freaquency,null);
            Freaquency = "" + freaquency;
        }

        public void Record() {
            capture = new WasapiCapture(selectedDevice);
            int i = capture.WaveFormat.ConvertLatencyToByteSize(100);
            capture.StartRecording();
            capture.DataAvailable += CaptureOnDataAvabile;
        }

        private void CaptureOnDataAvabile(object sender, WaveInEventArgs e) {
            for (int index = 0; index < e.BytesRecorded; index += 2) {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                float sample32 = sample / 32768f;
                fftBuffer.AddValue(sample32);
            }
            UpdatePeakMeter();
        }

        void UpdatePeakMeter() {
            // can't access this on a different thread from the one it was created on, so get back to GUI thread
            synchronizationContext.Post(s => Peak = SelectedDevice.AudioMeterInformation.MasterPeakValue, null);
        }
    }
}
