using MusicXml;
using SoundAnalyzer.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SoundAnalyzer.UserControls {
    public partial class MainUC : UserControl {
        MainUCViewModel viewModel = new MainUCViewModel();

        public MainUC() {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void fill_Click(object sender, RoutedEventArgs e) {
            noteA4.Value = noteA4.Value == 1 ? 0 : 1;
        }
    }
}
