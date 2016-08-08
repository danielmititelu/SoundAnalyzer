using SoundAnalyzer.UserControls;
using System.Windows;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Controls;
using SoundAnalyzer.ViewModels;
using SoundAnalyzer.Sheets;
using System.Linq;

namespace SoundAnalyzer {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            PagePanel.Children.Add(new PageUC());
            PagePanel.Children.Add(new Rectangle {
                Margin = new Thickness(5)
            });
            PagePanel.Children.Add(new PageUC());
            //AddNoteToPage();
            //NoteRepository noteRepository = new NoteRepository();
            //var asd = noteRepository.PianoKeys;
        }

        private List<object> GetAllNotes() {
            var notes = new List<object>();
            foreach (var child in PagePanel.Children) {
                if (child is PageUC) {
                    var newc = child as PageUC;
                    foreach (var deeperChild in newc.pageGrid.Children) {
                        var grandStaff = deeperChild as GrandStaffUC;
                        notes.AddRange(grandStaff.GetAllNotes());
                    }
                }
            }
            return notes;
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

        private void playButtonClick(object sender, RoutedEventArgs e) {
            //var notes = GetAllNotes();
            //string notesNames = "";
            //var dataContext = BackEnd.DataContext as MainUCViewModel;
            //foreach (var note in notes) {
            //    var elipse = note as Ellipse;
            //    notesNames += " " + Note.notes.First(n => n.Row == (int)elipse.GetValue(Grid.RowProperty)).Name;
            //}
            //dataContext.Line = notesNames;
            var noteGeneretor = new NoteGenerator();


        }
    }
}
