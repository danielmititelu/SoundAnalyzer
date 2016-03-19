using System.Linq;
using System.Threading;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System;
using NAudio.Wave;

namespace SoundAnalyzer {
    class MainUCViewModel : ViewModelBase {
        private readonly SynchronizationContext synchronizationContext;
        private MMDevice selectedDevice;
        private WasapiCapture capture;
        private float peak;
        private string decibels;

        public IEnumerable<MMDevice> CaptureDevices { get; private set; }

        public MMDevice SelectedDevice {
            get { return selectedDevice; }
            set { if (selectedDevice != value) {
                    selectedDevice = value;
                    OnPropertyChanged("SelectedDevice");
                }
            }
        }

        public float Peak {
            get { return peak; }
            set { if (peak != value) {
                    peak = value;
                    OnPropertyChanged("Peak");
                }
            }
        }

        public string Decibels { get { return decibels; } }

        public MainUCViewModel() {
            synchronizationContext = SynchronizationContext.Current;
            var enumerator = new MMDeviceEnumerator();
            CaptureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            SelectedDevice = CaptureDevices.FirstOrDefault(c => c.ID == defaultDevice.ID);
        }

        public void Record() {
            capture = new WasapiCapture(selectedDevice);
            int i = capture.WaveFormat.ConvertLatencyToByteSize(100);
            capture.StartRecording();
            capture.DataAvailable += CaptureOnDataAvabile;
        }

        private void CaptureOnDataAvabile(object sender, WaveInEventArgs e) {
            //WaveBuffer buffer = new WaveBuffer(e.Buffer);
            //foreach (var i in buffer.ShortBuffer) {
            //    decibels += "Decibels:" + i +"\n";
            //    OnPropertyChanged("Decibels");
            //}
            double sum = 0;
            for (var i = 0; i< e.Buffer.Length; i = i+2) {
                double sample = BitConverter.ToInt16(e.Buffer, i) / 32768.0;
                sum += (sample * sample); 
            }
            double rms = Math.Sqrt(sum / (e.Buffer.Length / 2));
            var decibel = 20 * Math.Log10(rms);
            decibels = "Decibels:" + decibel ;
            OnPropertyChanged("Decibels");
            UpdatePeakMeter();
        }

        void UpdatePeakMeter() {
            // can't access this on a different thread from the one it was created on, so get back to GUI thread
            synchronizationContext.Post(s => Peak = SelectedDevice.AudioMeterInformation.MasterPeakValue, null);
        }
    }
}
