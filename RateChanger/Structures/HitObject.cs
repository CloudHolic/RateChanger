using System;
using System.Collections.Generic;

namespace RateChanger.Structures
{
    public class HitObject
    {
        public HitObject()
        {
            X = 0;
            Y = 0;
            Time = 0;
            Type = 0;
            Hitsound = 0;
            Endtime = 0;
            Addition = Tuple.Create(0, 0, 0, 0, "");
            SliderType = null;
            Curves = new List<Tuple<int, int>>();
            Repeat = 1;
            PixelLength = 0;
            EdgeHitsound = Tuple.Create(0, 0);
            EdgeAddition = Tuple.Create(0, 0, 0, 0, "");
        }

        public HitObject(int x, int y, int time, int type, int hitsound, int endtime, Tuple<int, int, int, int, string> addition, string slidertype, 
            List<Tuple<int, int>> curves, int repeat, double pixellength, Tuple<int, int> edgehitsound, Tuple<int, int, int, int, string> edgeadd)
        {
            X = x;
            Y = y;
            Time = time;
            Type = type;
            Hitsound = hitsound;
            Endtime = endtime;
            Addition = addition;
            SliderType = slidertype;
            Curves = curves;
            Repeat = repeat;
            PixelLength = pixellength;
            EdgeHitsound = edgehitsound;
            EdgeAddition = edgeadd;
        }

        public HitObject(HitObject prevHitObject)
        {
            X = prevHitObject.X;
            Y = prevHitObject.Y;
            Time = prevHitObject.Time;
            Type = prevHitObject.Type;
            Hitsound = prevHitObject.Hitsound;
            Endtime = prevHitObject.Endtime;
            Addition = prevHitObject.Addition;
            SliderType = prevHitObject.SliderType;
            Curves = prevHitObject.Curves;
            Repeat = prevHitObject.Repeat;
            PixelLength = prevHitObject.PixelLength;
            EdgeHitsound = prevHitObject.EdgeHitsound;
            EdgeAddition = prevHitObject.EdgeAddition;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Time { get; set; }

        public int Type { get; set; }

        public int Hitsound { get; set; }

        public int Endtime { get; set; }

        public Tuple<int, int, int, int, string> Addition { get; set; }

        public string SliderType { get; set; }

        public List<Tuple<int, int>> Curves { get; set; }

        public int Repeat { get; set; }
        
        public double PixelLength { get; set; }

        public Tuple<int, int> EdgeHitsound { get; set; }

        public Tuple<int, int, int, int, string> EdgeAddition { get; set; }
    }
}