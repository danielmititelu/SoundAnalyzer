using MusicXml;
using SoundAnalyzer.UserControls;
using System.Windows;

namespace SoundAnalyzer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            PagePanel.Children.Add(new PageUC());
           // PagePanel.Children.Add(new PageUC());
        }
    }
}
