using MusicXml;
using SoundAnalyzer.UserControls;
using System.Windows;
using System.Windows.Shapes;
using System;

namespace SoundAnalyzer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            PagePanel.Children.Add(new PageUC());
            PagePanel.Children.Add(new Rectangle {
                Margin = new Thickness(5)
            });
            PagePanel.Children.Add(new PageUC());
            AddNoteToPage();
        }

        private void AddNoteToPage() {
            foreach (var child in PagePanel.Children) {
                if (child is PageUC) {
                    var newc = child as PageUC;
                    foreach (var deeperChild in newc.pageGrid.Children) {
                        var grandStaff = deeperChild as GrandStaffUC;
                        grandStaff.AddNote();
                        grandStaff.AddNote();
                        grandStaff.AddNote();
                        break;
                    }
                    break;
                }
            }
        }
    }
}
