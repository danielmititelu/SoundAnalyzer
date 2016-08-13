using SoundAnalyzer.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundAnalyzer {
    class GoertzelAlgorithm {
        public int SampleRate { get; set; }
        NoteRepository noteRepository;

        public GoertzelAlgorithm(int sampleRate) {
            noteRepository = new NoteRepository();
            SampleRate = sampleRate;
        }

        public bool NotePlayed(List<float> buffer, double targetFreaquency) {
            return FrequencyExists(buffer, targetFreaquency);
        }

        public bool NotePlayed(List<float> buffer, string noteName, int octave) {
            var note = noteRepository.GetNote(noteName, octave);
            return FrequencyExists(buffer, note.Freaquency);
        }

        public bool NotePlayed(List<float> buffer, int keyNumber) {
            var note = noteRepository.GetNote(keyNumber);
            return FrequencyExists(buffer, note.Freaquency);
        }

        public Dictionary<string, int> DetectAllNotesPlayed(List<float> buffer) {
            var notesPlayed = new Dictionary<string, int>();
            for (int i = 1; i <= 88; i++) {
                var note = noteRepository.GetNote(i);
                var exists = NotePlayed(buffer, note.Freaquency);
                var value = exists ? 1 : 0;
                notesPlayed.Add(note.Name + note.Octave, value);
            }
            return notesPlayed;
        }

        private bool FrequencyExists(List<float> buffer, double targetFreaquency) {
            var power = GoertzelFilter(buffer, targetFreaquency, buffer.Count);
            if (power > 1000) return true;
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

