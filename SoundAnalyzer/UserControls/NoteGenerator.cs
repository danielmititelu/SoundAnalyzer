using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SoundAnalyzer.UserControls {
    class NoteGenerator {
        public object getNote() {
            Ellipse note = new Ellipse();
            note.Fill = Brushes.Black;
            note.Width = (double)new LengthConverter().ConvertFrom("0.4cm");
            note.Stretch = Stretch.Fill;
            note.HorizontalAlignment = HorizontalAlignment.Left;
            note.Margin = new Thickness(10, 0, 0, 0);
            note.SetValue(Grid.RowProperty, 11);
            note.SetValue(Grid.RowSpanProperty, 2);
            return null;
        }
    }
}
