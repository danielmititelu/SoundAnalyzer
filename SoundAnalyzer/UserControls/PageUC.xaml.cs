using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using SoundAnalyzer.Sheets;

namespace SoundAnalyzer.UserControls {
    public partial class PageUC : UserControl {

        public PageUC() {
            InitializeComponent();
        }

        public PageUC(List<NotesGroup> notesGroup) {
            InitializeComponent();
            foreach (var noteGroup in notesGroup.GroupBy(17)) {
                var grandStaff = new GrandStaffUC(noteGroup.ToList());
                page.Children.Add(grandStaff);
            }
        }
    }
}
