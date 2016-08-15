using SoundAnalyzer.Sheets;
using SoundAnalyzer.ViewModels;
using System.Windows.Controls;

namespace SoundAnalyzer.UserControls {
    public partial class MainUC : UserControl {
        //MainUCViewModel viewModel = new MainUCViewModel();

        public MainUC() {
            InitializeComponent();
            //DataContext = viewModel;
            AddAllKeys();
        }

        private void AddAllKeys() {
            var noteRepository = new NoteRepository();
            for (int i = 1; i <= 88; i++) {
                var key = noteRepository.GetNote(i);
                keys.Children.Add(new LightUC(key.Name + key.Octave));
            }
        }
    }
}
