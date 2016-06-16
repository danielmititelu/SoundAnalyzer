using System.Collections.Generic;
using System.Linq;

namespace SoundAnalyzer.Sheets {
    class Sheet {
        List<string> pianoNotes = new List<string>();
        Note note = new Note();
        int counter = 0;

        public Sheet(string notes) {
            foreach (var note in notes.Split(' '))
                pianoNotes.Add(note);
        }

        public double FirstNoteFreaquency() {
            var asd = pianoNotes.ElementAt(counter);
            return Note.notes.First(n => n.Name == asd).Freaquency;
        }

        public double NextNoteFreaquency() {
            counter++;
            if (counter >= pianoNotes.Count) return 0;
            var asd = pianoNotes.ElementAt(counter);
            return Note.notes.First(n => n.Name == asd).Freaquency;
        }

        public string CurrentNoteName() {
            if (counter >= pianoNotes.Count) return "END";
            return pianoNotes.ElementAt(counter);
        }

        public void ResetCounter() {
            counter = 0;
        }
    }
}
