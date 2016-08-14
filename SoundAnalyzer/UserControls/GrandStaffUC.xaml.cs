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

namespace SoundAnalyzer.UserControls {
    /// <summary>
    /// Interaction logic for GrandStaffUC.xaml
    /// </summary>
    public partial class GrandStaffUC : UserControl {
        public GrandStaffUC() {
            InitializeComponent();
            AddStaffs("Treble", "Bass");
        }
        public GrandStaffUC(string firstClef, string secondClef) {
            InitializeComponent();
            AddStaffs(firstClef, secondClef);
        }

        private void AddStaffs(string firstClef, string secondClef) {
            var firstStaff = new StaffUC(firstClef);
            var secondStaff = new StaffUC(secondClef);
            grandStaff.Children.Add(firstStaff);
            grandStaff.Children.Add(secondStaff);
        }
    }
}
