using SoundAnalyzer.Sheets;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Linq;

namespace SoundAnalyzer.UserControls {

    public partial class PagesUC : UserControl {
        private int _distanceBetweenPages = 5;
        private int _notesPerPage = 51;
        public List<NotesGroup> NotesGroup {
            get { return (List<NotesGroup>)GetValue(NotesGroupProperty); }
            set { SetValue(NotesGroupProperty, value); }
        }

        public static readonly DependencyProperty NotesGroupProperty = DependencyProperty.Register("NotesGroup",
            typeof(List<NotesGroup>), typeof(PagesUC), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        public PagesUC() {
            InitializeComponent();
        }

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            PagesUC pagesUC = dependencyObject as PagesUC;
            pagesUC.OnValueChanged(e);
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs e) {
            var notesGroup = (List<NotesGroup>)e.NewValue;

            foreach (var group in notesGroup.GroupBy(_notesPerPage)) {
                var page = new PageUC(group.ToList());
                PagePanel.Children.Add(page);
                PagePanel.Children.Add(new Rectangle { Margin = new Thickness(_distanceBetweenPages) });
            }
        }
    }
}
