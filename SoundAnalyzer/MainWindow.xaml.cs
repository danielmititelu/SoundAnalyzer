using SoundAnalyzer.UserControls;
using System.Windows;
using System.Collections.Generic;
using SoundAnalyzer.ViewModels;

namespace SoundAnalyzer {
    public partial class MainWindow : Window {
        //public List<PageUC> Pages { get; set; }
        MainUCViewModel viewModel = new MainUCViewModel();

        public MainWindow() {
            InitializeComponent();
            DataContext = viewModel;
            //Pages = new List<PageUC>();
            //Pages.Add(new PageUC());
            //Pages.Add(new PageUC());
        }
    }
}
