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

        int i = 0;
        private void CaptureOnDataAvabile(object sender, WaveInEventArgs e) {
            for (int index = 0; index < e.BytesRecorded; index += 2) {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                float sample32 = sample / 32768f;
                i++;
                Console.WriteLine("I have been summoned" + i + "    " + sample32);
            }
            UpdatePeakMeter();
        }

        void UpdatePeakMeter() {
            // can't access this on a different thread from the one it was created on, so get back to GUI thread
            synchronizationContext.Post(s => Peak = SelectedDevice.AudioMeterInformation.MasterPeakValue, null);
        }
    }
}

