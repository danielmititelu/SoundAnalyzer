using System;
using System.Windows;
using System.Windows.Controls;

namespace SoundAnalyzer {
    /// <summary>
    /// Interaction logic for MainUC.xaml
    /// </summary>
    public partial class MainUC : UserControl {
        MainUCViewModel viewModel = new MainUCViewModel();

        public MainUC() {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Record_Click(object sender, RoutedEventArgs e) {
            Console.WriteLine("Recording");
            viewModel.Record();
        }
    }
}
