using System.Windows.Controls;

namespace SoundAnalyzer.UserControls {
    public partial class Light : UserControl {
        public Light() {
            InitializeComponent();
        }

        public Light(string noteName) {
            InitializeComponent();
            label.Content = noteName;
            light.SetBinding(ProgressBar.ValueProperty, $"Keys[{noteName}]");
        }
    }
}
