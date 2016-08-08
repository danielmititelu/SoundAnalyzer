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
        public int lefMargin = 100;
        public GrandStaffUC() {
            InitializeComponent();
        }

        public void AddNote() {
            Ellipse note = new Ellipse();
            note.Fill = Brushes.Black;
            note.Width = (double)new LengthConverter().ConvertFrom("0.4cm");
            note.Stretch = Stretch.Fill;
            note.HorizontalAlignment = HorizontalAlignment.Left;
            note.Margin = new Thickness(lefMargin, 0, 0, 0);
            note.SetValue(Grid.RowProperty, 11);
            note.SetValue(Grid.RowSpanProperty, 2);
            note.MouseLeftButtonDown += NoteMouseLeftButtonDown;
            note.MouseMove += NoteMouseMove;
            note.MouseLeftButtonUp += NoteMouseLeftButtonUp;
            myGrid.Children.Add(note);
            lefMargin += 100;
        }

        public List<object> GetAllNotes() {
            var notes = new List<object>();
            foreach (var note in myGrid.Children) {
                if (note is Ellipse) {
                    notes.Add(note);
                }
            }
            return notes;
        }

        private void NoteMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Ellipse note = (Ellipse)sender;
            note.CaptureMouse();
        }

        private void NoteMouseMove(object sender, MouseEventArgs e) {
            Ellipse note = (Ellipse)sender;
            var mousePosition = e.GetPosition(myGrid);
            if (note.IsMouseCaptured) {
                note.SetValue(Grid.RowProperty, GetRow(mousePosition));
                note.Margin = new Thickness(mousePosition.X,0,0,0);
            }
        }

        private void NoteMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Ellipse note = (Ellipse)sender;
            note.ReleaseMouseCapture();
        }

        private int GetRow(Point point) {
            var accumulatedHeight = 0.0;
            var row = 0;
            foreach (var rowDefinition in myGrid.RowDefinitions) {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }
            return row;
        }
    }
}
