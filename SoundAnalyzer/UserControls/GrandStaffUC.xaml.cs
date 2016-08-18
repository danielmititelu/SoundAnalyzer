using System.Collections.Generic;
using System.Windows.Controls;
using SoundAnalyzer.Sheets;

namespace SoundAnalyzer.UserControls {
    public partial class GrandStaffUC : UserControl {
        public GrandStaffUC() {
            InitializeComponent();
            AddStaffs("Treble", "Bass", null);
        }

        public GrandStaffUC(List<NotesGroup> noteGroup) {
            InitializeComponent();
            AddStaffs("Treble", "Bass", noteGroup);
        }

        public GrandStaffUC(string firstClef, string secondClef) {
            InitializeComponent();
            AddStaffs(firstClef, secondClef, null);
        }

        private void AddStaffs(string firstClef, string secondClef, List<NotesGroup> noteGroup) {
            var firstStaff = new StaffUC(firstClef, noteGroup,1);
            var secondStaff = new StaffUC(secondClef, noteGroup,2);
            grandStaff.Children.Add(firstStaff);
            grandStaff.Children.Add(secondStaff);
        }
    }
}
