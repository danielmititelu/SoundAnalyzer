using System.Windows.Controls;

namespace SoundAnalyzer.UserControls {
    public partial class LightUC : UserControl {
        public LightUC() {
            InitializeComponent();
        }

        public LightUC(string noteName) {
            InitializeComponent();
            label.Content = noteName;
            light.SetBinding(ProgressBar.ValueProperty, $"Keys[{noteName}]");
        }
    }
}
