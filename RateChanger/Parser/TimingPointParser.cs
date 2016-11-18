using System;
using System.Collections.Generic;
using System.IO;

using RateChanger.CustomExceptions;
using RateChanger.Structures;

namespace RateChanger.Parser
{
    public static class TimingPointParser
    {
        public static List<TimingPoint> Parse(string filename)
        {
            var data = new List<TimingPoint>();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Find TimingPoint Section.
                    if (currentLine == "[TimingPoints]")
                    {
                        while ((currentLine = reader.ReadLine()) != null)
                        {
                            if (currentLine.Length == 0)
                                break;

                            var parsed = currentLine.Split(',');
                            if(parsed.Length != 8)
                                throw new InvalidBeatmapException("Wrong TimingPoint Found.");

                            var temp = new TimingPoint
                            {
                                //  Offset
                                Offset = Convert.ToDouble(parsed[0]),
                                
                                //  Milliseconds per Beat
                                MsPerBeat = Convert.ToDouble(parsed[1]),

                                //  Meter
                                Meter = Convert.ToInt32(parsed[2]),

                                //  Sample Type
                                SampleType = Convert.ToInt32(parsed[3]),

                                //  Sample Set
                                SampleSet = Convert.ToInt32(parsed[4]),

                                //  Volume
                                Volume = Convert.ToInt32(parsed[5]),

                                //  Inherited
                                Inherited = Convert.ToInt32(parsed[6]) == 1,

                                //  Kiai
                                Kiai = Convert.ToInt32(parsed[7]) == 1
                            };

                            data.Add(temp);
                        }
                    }
                }
            }

            if(data.Count == 0)
                data.Add(new TimingPoint());

            return data;
        }
    }
}
