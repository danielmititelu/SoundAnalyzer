using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace SoundAnalyzer.Sheets {
    class Sheet {
        NoteRepository noteRepository = new NoteRepository();
        public List<NotesGroup> Notes { get; set; }
        public int Counter { get; set; }

        public Sheet(string path) {
            Notes = InitializeNotes(path);
        }

        private List<NotesGroup> InitializeNotes(string path) {
            var notes = new List<NotesGroup>();
            var lines = File.ReadAllLines(path);
            var counter = 0;
            foreach (var line in lines) {
                var allNotes = line.Split(';');
                var noteGroup = new NotesGroup();

                if (allNotes[0] != "") {
                    var firstStaffNotes = allNotes[0].Split(',');
                    foreach (var note in firstStaffNotes) {
                        var noteName = note.Length == 3 ? note.Substring(0, 2) : note.Substring(0, 1);
                        var octave = int.Parse(note.Last().ToString());
                        noteGroup.FirstStaffNotes.Add(noteRepository.GetNote(noteName, octave));
                    }
                }

                if (allNotes[1] != "") {
                    var secondStaffNotes = allNotes[1].Split(',');
                    foreach (var note in secondStaffNotes) {
                        var noteName = note.Length == 3 ? note.Substring(0, 2) : note.Substring(0, 1);
                        var octave = int.Parse(note.Last().ToString());
                        noteGroup.SecondtaffNotes.Add(noteRepository.GetNote(noteName, octave));
                    }
                }

                noteGroup.Color = new SolidColorBrush(Colors.Black);
                noteGroup.Number = counter;
                counter++;
                notes.Add(noteGroup);
            }
            return notes;
        }

        public IEnumerable<Key> GetCurentNotes() {
            var noteGroup = Notes.ElementAt(Counter);
            return noteGroup.FirstStaffNotes.Concat(noteGroup.SecondtaffNotes);
        }

        public string GetCurrentNotesAsString() {
            var noteGroup = Notes.ElementAt(Counter);
            return noteGroup.ToString();
        }

        public void IncreaseCounter() {
            Counter++;
            if (Counter >= Notes.Count) {
                Counter = 0;
            }
        }

        public int GetCurentCounter() {
            return Counter;
        }

        public int GetNextCounter() {
            if (Counter + 1 >= Notes.Count) {
                return 0;
            } else {
                return Counter + 1;
            }
        }
    }
}
