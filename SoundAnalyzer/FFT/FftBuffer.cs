using NAudio.Dsp;
using System;
using System.Collections.Generic;

namespace SoundAnalyzer.FFT {
    class FftBuffer {
        private Queue<float> queue;
        private int fftLength;
        private Complex[] fftBuffer;
        private int m;
        public event EventHandler ThresholdReached;

        public bool BufferIsFull { get { return queue.Count > fftLength; } }

        public FftBuffer(int fftLength) {
            this.fftLength = fftLength;
            this.m = (int)Math.Log(fftLength, 2.0);
            queue = new Queue<float>();
            fftBuffer = new Complex[fftLength];

        }

        public void AddValue(float inputBuffer) {
            queue.Enqueue(inputBuffer);
            if (BufferIsFull) { OnThresholdReaced(EventArgs.Empty); }
        }

        public Complex[] retrieveArray(int length) {
            for (int i = 0; i < length; i++) {
                fftBuffer[i].X = (float)(queue.Dequeue() * FastFourierTransform.HammingWindow(i, fftLength));
                fftBuffer[i].Y = 0;
            }
            FastFourierTransform.FFT(true, m, fftBuffer);
            return fftBuffer;
        }

        protected virtual void OnThresholdReaced(EventArgs e) {
            EventHandler handler = ThresholdReached;
            if (handler != null) { handler(this, e); }
        }
    }
}