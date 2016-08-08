using System;
using System.Collections.Generic;

namespace SoundAnalyzer.Sheets {
    class NoteRepository {
        public List<Key> PianoKeys { get; set; }
        public int _numberOfPianoKeys = 88;
        public double _A4Freaquency = 440;
        public int _A4Number = 49;

        public NoteRepository() {
            GeneratePianoKeys();
        }

        private void GeneratePianoKeys() {
            PianoKeys = new List<Key>();
            var keyNames = new List<string> { "C", "C#", "D", "D#", "E", "F", " F#", " G", " G#", "A", "A#", "B" };
            int keyNumber = -8;
            for (int octave = 0; octave < 9; octave++)
                foreach (var keyName in keyNames) {
                    PianoKeys.Add(new Key {
                        Number = keyNumber,
                        Freaquency = GetFreaquency(keyNumber),
                        Name = keyName,
                        Octave = octave
                    });
                    keyNumber++;
                }
        }

        private double GetFreaquency(int keyNumber) {
            return (Math.Pow(2, (double)(keyNumber - _A4Number) / 12)) * _A4Freaquency;
        }
    }
}
