using System;
using System.Collections.Generic;

namespace SoundAnalyzer.Sheets {
   internal class NoteRepository {
        public List<Key> PianoKeys { get; set; }
        public int NumberOfPianoKeys = 88;
        public double A4Freaquency = 440;
        public int A4Number = 49;
        private int NumberOfMissingNotesInFirstOctave = -8;

        public NoteRepository() {
            GeneratePianoKeys();
        }

        public Key GetNote(string noteName, int octave) {
            return PianoKeys.Find(n => n.Name == noteName && n.Octave == octave);
        }

        public Key GetNote(int keyNumber) {
            return PianoKeys.Find(n => n.Number == keyNumber);
        }

        private void GeneratePianoKeys() {
            PianoKeys = new List<Key>();
            var keyNames = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            var keyNumber = NumberOfMissingNotesInFirstOctave;
            for (var octave = 0; octave < 9; octave++)
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
            return (Math.Pow(2, (double)(keyNumber - A4Number) / 12)) * A4Freaquency;
        }
    }
}
