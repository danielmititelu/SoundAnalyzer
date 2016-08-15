namespace SoundAnalyzer.Sheets {
    public class Key {
        public int Number { get; set; }
        public string Name { get; set; }
        public double Freaquency { get; set; }
        public int Octave { get; set; }

        public override string ToString() {
            return Name + Octave;
        }
    }
}
