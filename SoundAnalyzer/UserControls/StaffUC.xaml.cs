using SoundAnalyzer.Sheets;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace SoundAnalyzer.UserControls {
    public partial class StaffUC : UserControl {
        LengthConverter lengthConverter = new LengthConverter();
        public double LeftMargin { get; set; }
        public string Clef { get; set; }
        Dictionary<int, Key> _columnKey = new Dictionary<int, Key>();
        NoteRepository noteRepository = new NoteRepository();
        private int _firstNoteBassClef = 9;
        private int _lastNoteBassClef = 51;
        private int _firstNoteTrebleClef = 30;
        private int _lastNoteTrebleClef = 71;
        private int _invalidValue = 999;

        public StaffUC() {
            InitializeComponent();
            LeftMargin = 1;
            InitializeNotes("Treble");
            AddClef("Treble");
            Clef = "Treble";
            //AddNote("F1");
            //AddNote("D3");
            //AddNote("G6");
            //AddNote("C#4");
            //AddNotes(new List<string> { "F1","C4"});
            AddNotes(new List<string> { "D3","C4","C5" });
        }


        public StaffUC(string clef) {
            InitializeComponent();
            LeftMargin = 1;
            InitializeNotes(clef);
            AddClef(clef);
            Clef = clef;
        }

        public bool AddNotes(List<string> notes) {
            LeftMargin += 1;
            if (LeftMargin > 19) return false;
            foreach (var note in notes) {
                AddNote(note);
            }
            return true;
        }

        private void AddNote(string keyName) {
            var row = GetRow(keyName);
            if (row == _invalidValue) return;

            var note = new NoteUC();
            Grid.SetRowSpan(note, 2);
            Grid.SetRow(note, row);
            note.HorizontalAlignment = HorizontalAlignment.Left;
            var leftMargin = (double)lengthConverter.ConvertFrom(LeftMargin + "cm");
            note.Margin = new Thickness(leftMargin, 0, 0, 0);
            mainGrid.Children.Add(note);
            AddHelperLines(row);
        }

        private void AddClef(string clef) {
            if (clef == "Bass") {
                var bassClef = new BassClefUC();
                Grid.SetRowSpan(bassClef, 11);
                Grid.SetRow(bassClef, 7);
                bassClef.HorizontalAlignment = HorizontalAlignment.Left;
                bassClef.Margin = new Thickness(10, 0, 0, 0);
                mainGrid.Children.Add(bassClef);
            } else if(clef =="Treble") {
                var trebleClef = new TrebleClefUC();
                Grid.SetRowSpan(trebleClef, 15);
                Grid.SetRow(trebleClef, 7);
                trebleClef.HorizontalAlignment = HorizontalAlignment.Left;
                trebleClef.Margin = new Thickness(10, 0, 0, 0);
                mainGrid.Children.Add(trebleClef);
            }
        }

        private void InitializeNotes(string clef) {
            var firstNote = 0;
            var lastNote = 0;
            if (clef == "Bass") {
                firstNote = _firstNoteBassClef;
                lastNote = _lastNoteBassClef;
            } else if (clef == "Treble") {
                firstNote = _firstNoteTrebleClef;
                lastNote = _lastNoteTrebleClef;
            }
            var keys = noteRepository.PianoKeys.FindAll(k => !k.Name.Contains("#") && k.Number >= firstNote && k.Number <= lastNote)
                       .OrderByDescending(k => k.Number);
            int column = 0;
            foreach (var key in keys) {
                _columnKey.Add(column, key);
                column++;
            }
        }

        private int GetRow(string keyName) {
            var name = keyName.First().ToString();
            var octave = Int32.Parse(keyName.Last().ToString());
            var note = _columnKey.FirstOrDefault(k => k.Value.Name == name && k.Value.Octave == octave);
            if (note.Value == null) {
                Console.WriteLine($"Key {keyName} can't be represented on {Clef} clef");
                return _invalidValue;
            }
            return note.Key;
        }

        private void AddHelperLines(int row) {
            if (row < 9) {
                for (int i = 7; i > row; i = i - 2) {
                    AddHelperLine(i);
                }
            } else if (row > 17) {
                for (int i = 19; i <= row + 1; i = i + 2) {
                    AddHelperLine(i);
                }
            }
        }

        private void AddHelperLine(int row) {
            Line helperLine = new Line();
            Grid.SetRow(helperLine, row);
            helperLine.X1 = (double)lengthConverter.ConvertFrom((LeftMargin - 0.2) + "cm");
            helperLine.X2 = (double)lengthConverter.ConvertFrom((LeftMargin + 0.6) + "cm");
            helperLine.Y1 = 0;
            helperLine.Y2 = 0;
            helperLine.SetValue(Shape.StrokeThicknessProperty, (double)1);
            helperLine.Stroke = Brushes.Black;
            mainGrid.Children.Add(helperLine);
        }
    }
}
