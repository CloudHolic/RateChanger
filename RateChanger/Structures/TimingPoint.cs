namespace RateChanger.Structures
{
    public class TimingPoint
    {
        public TimingPoint()
        {
            Offset = 0;
            MsPerBeat = 0;
            Meter = 4;
            SampleType = 0;
            SampleSet = 0;
            Volume = 30;
            Inherited = false;
            Kiai = false;
        }

        public TimingPoint(int offset, double msperbeat, int meter, int sampletype, int sampleset, int volume,
            bool inherited, bool kiai)
        {
            Offset = offset;
            MsPerBeat = msperbeat;
            Meter = meter;
            SampleType = sampletype;
            SampleSet = sampleset;
            Volume = volume;
            Inherited = inherited;
            Kiai = kiai;
        }

        public TimingPoint(TimingPoint prevTimingPoint)
        {
            Offset = prevTimingPoint.Offset;
            MsPerBeat = prevTimingPoint.MsPerBeat;
            Meter = prevTimingPoint.Meter;
            SampleType = prevTimingPoint.SampleType;
            SampleSet = prevTimingPoint.SampleSet;
            Volume = prevTimingPoint.Volume;
            Inherited = prevTimingPoint.Inherited;
            Kiai = prevTimingPoint.Kiai;
        }

        public int Offset { get; set; }

        public double MsPerBeat { get; set; }

        public int Meter { get; set; }

        public int SampleType { get; set; }

        public int SampleSet { get; set; }

        public int Volume { get; set; }

        public bool Kiai { get; set; }

        public bool Inherited { get; set; }
    }
}
