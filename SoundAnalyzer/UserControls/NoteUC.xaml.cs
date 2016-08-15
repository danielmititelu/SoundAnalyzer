using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SoundAnalyzer.UserControls {
    public partial class NoteUC : UserControl {
        public SolidColorBrush Color { get; set; }
        public NoteUC() {
            InitializeComponent();
        }

        public NoteUC(int number) {
            InitializeComponent();
            note.SetBinding(Ellipse.FillProperty, $"Notes[{number}].Color");
        }
    }
}
