using NAudio.Wave;
using System.Windows;
using System;
using System.Collections.Generic;

namespace WpfTest {
    public partial class MainWindow : Window {
        private BufferedWaveProvider buffer;
        private WaveIn waveIn;
        private WaveOut waveOut;
        private const double TargetFreaquency = 880;//C4 note
        private const int SampleRate = 44100;

        public MainWindow() {
            InitializeComponent();
            InitializeSound();
            waveIn.StartRecording();
            // waveOut.Play();
        }

        private void InitializeSound() {
            waveIn = new WaveIn();
            WaveFormat checkformat = waveIn.WaveFormat;
            WaveFormat myformat = new WaveFormat(44100, 16, 2);
            waveIn.WaveFormat = myformat;
            waveIn.DataAvailable += WaveInDataAvailable;
            waveOut = new WaveOut();
            buffer = new BufferedWaveProvider(waveIn.WaveFormat);
            waveOut.Init(buffer);
        }

        private void WaveInDataAvailable(object sender, WaveInEventArgs e) {
            //buffer.AddSamples(e.Buffer, 0, e.BytesRecorded);

            var floatBuffer = new List<float>();
            for (int index = 0; index < e.BytesRecorded; index += 2) {
                short sample = BitConverter.ToInt16(e.Buffer, index);
                float sample32 = sample / (float)Int16.MaxValue;
                floatBuffer.Add(sample32);
            }

            if (NotePlayed(floatBuffer.ToArray())) {
                Console.WriteLine("You have played C4");
            }
        }

        private bool NotePlayed(float[] buffer) {
            double power = GoertzelFilter(buffer, TargetFreaquency, buffer.Length);
            if (power > 100) { Console.WriteLine("Power" + power); return true; };
            return false;
        }

        private double GoertzelFilter(float[] samples, double targetFreaquency, int end) {
            double sPrev = 0.0;
            double sPrev2 = 0.0;
            int i;
            double normalizedfreq = targetFreaquency / SampleRate;
            double coeff = 2 * Math.Cos(2 * Math.PI * normalizedfreq);
            for (i = 0; i < end; i++) {
                double s = samples[i] + coeff * sPrev - sPrev2;
                sPrev2 = sPrev;
                sPrev = s;
            }
            double power = sPrev2 * sPrev2 + sPrev * sPrev - coeff * sPrev * sPrev2;
            return power;
        }
    }
}
