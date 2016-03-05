using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
