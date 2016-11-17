namespace RateChanger.Structures
{
    public class General
    {
        public General()
        {
            AudioFilename = null;
            AudioLeadIn = 0;
            PreviewTime = 0;
            Countdown = false;
            SampleSet = "Normal";
            StackLeniency = 0.7;
            Mode = 0;
            LetterboxInBreaks = false;
            SpecialStyle = false;
            WidescreenStoryboard = false;
        }

        public General(string audio, int leadin, int preview, bool countdown, string sampleset,
            double stack, int mode, bool letterbox, bool special, bool widescreen)
        {
            AudioFilename = audio;
            AudioLeadIn = leadin;
            PreviewTime = preview;
            Countdown = countdown;
            SampleSet = sampleset;
            StackLeniency = stack;
            Mode = mode;
            LetterboxInBreaks = letterbox;
            SpecialStyle = special;
            WidescreenStoryboard = widescreen;
        }

        public General(General prevGeneral)
        {
            AudioFilename = prevGeneral.AudioFilename;
            AudioLeadIn = prevGeneral.AudioLeadIn;
            PreviewTime = prevGeneral.PreviewTime;
            Countdown = prevGeneral.Countdown;
            SampleSet = prevGeneral.SampleSet;
            StackLeniency = prevGeneral.StackLeniency;
            Mode = prevGeneral.Mode;
            LetterboxInBreaks = prevGeneral.LetterboxInBreaks;
            SpecialStyle = prevGeneral.SpecialStyle;
            WidescreenStoryboard = prevGeneral.WidescreenStoryboard;
        }

        public string AudioFilename { get; set; }

        public int AudioLeadIn { get; set; }

        public int PreviewTime { get; set; }

        public bool Countdown { get; set; }

        public string SampleSet { get; set; }

        public double StackLeniency { get; set; }

        public int Mode { get; set; }

        public bool LetterboxInBreaks { get; set; }

        public bool SpecialStyle { get; set; }

        public bool WidescreenStoryboard { get; set; }
    }
}
