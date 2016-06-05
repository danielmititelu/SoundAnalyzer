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
            //viewModel.Record();
        }

        private void RecordClick(object sender, RoutedEventArgs e) {
            
        }
    }
}
