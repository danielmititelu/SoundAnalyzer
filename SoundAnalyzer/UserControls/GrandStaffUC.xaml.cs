using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SoundAnalyzer.Sheets;

namespace SoundAnalyzer.UserControls {
    /// <summary>
    /// Interaction logic for GrandStaffUC.xaml
    /// </summary>
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
