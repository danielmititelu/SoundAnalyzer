using System.Collections.Generic;
using System.Windows.Media;

namespace SoundAnalyzer.Sheets {
    public class NotesGroup {
        public List<Key> FirstStaffNotes { get; set; }
        public List<Key> SecondtaffNotes { get; set; }
        public SolidColorBrush Color { get; set; }
        public int Number { get; set; }

        public NotesGroup() {
            FirstStaffNotes = new List<Key>();
            SecondtaffNotes = new List<Key>();
        }

        public override string ToString() {
            return string.Join(" ", FirstStaffNotes) + ";" + string.Join(" ", SecondtaffNotes);
        }
    }
}
