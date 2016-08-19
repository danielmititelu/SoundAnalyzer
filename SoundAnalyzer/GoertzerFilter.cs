using SoundAnalyzer.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundAnalyzer {
    internal class GoertzelAlgorithm {
        public int SampleRate { get; set; }
        public int Sensibility { get; set; }
        private NoteRepository _noteRepository;

        public GoertzelAlgorithm(int sampleRate) {
            _noteRepository = new NoteRepository();
            SampleRate = sampleRate;
            Sensibility = 80;
        }

        public bool NotePlayed(List<float> buffer, double targetFreaquency) {
            return FrequencyExists(buffer, targetFreaquency);
        }

        public bool NotePlayed(List<float> buffer, string noteName, int octave) {
            var note = _noteRepository.GetNote(noteName, octave);
            return FrequencyExists(buffer, note.Freaquency);
        }

        public bool NotePlayed(List<float> buffer, int keyNumber) {
            var note = _noteRepository.GetNote(keyNumber);
            return FrequencyExists(buffer, note.Freaquency);
        }

        public Dictionary<string, int> DetectAllNotesPlayed(List<float> buffer) {
            var notesPlayed = new Dictionary<string, int>();
            for (var i = 1; i <= 88; i++) {
                var note = _noteRepository.GetNote(i);
                var exists = NotePlayed(buffer, note.Freaquency);
                var value = exists ? 1 : 0;
                notesPlayed.Add(note.ToString(), value);
            }
            return notesPlayed;
        }

        public bool NotesPlayed(List<float> buffer, IEnumerable<Key> notes) {
            foreach (var note in notes) {
                if (!FrequencyExists(buffer,note.Freaquency)) {
                    return false;
                }
            }
            return true;
        }

        private bool FrequencyExists(List<float> buffer, double targetFreaquency) {
            var power = GoertzelFilter(buffer, targetFreaquency, buffer.Count);
            if (power > Sensibility) return true;
            return false;
        }

        private double GoertzelFilter(List<float> samples, double targetFreaquency, int end) {
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

