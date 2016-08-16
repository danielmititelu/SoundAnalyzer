using System.Windows;
using SoundAnalyzer.ViewModels;

namespace SoundAnalyzer {
    public partial class MainWindow : Window {
        MainUCViewModel viewModel = new MainUCViewModel();

        public MainWindow() {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
