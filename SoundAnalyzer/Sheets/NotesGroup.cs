using System.Collections.Generic;

namespace SoundAnalyzer.Sheets {
    class NotesGroup {
        public List<Key> FirstStaffNotes { get; set; }
        public List<Key> SecondtaffNotes { get; set; }
        public string Brush { get; set; }

        public override string ToString() {
            return string.Join(" ", FirstStaffNotes) + " " + string.Join(" ", SecondtaffNotes);
        }
    }
}
