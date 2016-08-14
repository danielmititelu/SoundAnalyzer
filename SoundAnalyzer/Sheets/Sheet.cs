using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoundAnalyzer.Sheets {
    class Sheet {
        NoteRepository noteRepository = new NoteRepository();
        List<List<Key>> _notes;
        public int CurentCount { get; set; }

        public Sheet(string path) {
            _notes = InitializeNotes(path);
        }

        private List<List<Key>> InitializeNotes(string path) {
            var notes = new List<List<Key>>();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines) {
                var notesOnLine = line.Split(',');
                var keys = new List<Key>();
                foreach (var note in notesOnLine) {
                    var noteName = note.Length == 3 ? note.Substring(0, 2) : note.Substring(0, 1);
                    var octave = int.Parse(note.Last().ToString());
                    keys.Add(noteRepository.GetNote(noteName, octave));
                }
                notes.Add(keys);
            }
            return notes;
        }

        public List<Key> GetCurentNotes() {
            return _notes.ElementAt(CurentCount);
        }

        public List<Key> GetNextNotes() {
            CurentCount++;
            if (CurentCount >= _notes.Count) {
                CurentCount = 0;
            }
            return _notes.ElementAt(CurentCount);
        }
    }
}
