using Microsoft.Win32;
using SoundAnalyzer.Sheets;
using SoundAnalyzer.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SoundAnalyzer.UserControls {
    public partial class MainUC : UserControl {
        public MainUC() {
            InitializeComponent();
            AddAllKeys();
            sensibility.Value = 3000;
        }

        private void AddAllKeys() {
            var noteRepository = new NoteRepository();
            for (int i = 1; i <= 88; i++) {
                var key = noteRepository.GetNote(i);
                keys.Children.Add(new LightUC(key.Name + key.Octave));
            }
        }

        private void LoadFileClick(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();

            bool? result = dialog.ShowDialog();

            if (result == true) {
                string filename = dialog.FileName;
                path.Text = filename;
                var viewModel = DataContext as MainUCViewModel;
                viewModel.Path = filename;
            }
        }
    }
}
