using System;
using System.Collections.Generic;
using System.IO;

using RateChanger.CustomExceptions;
using RateChanger.Structures;

namespace RateChanger.Parser
{
    public static class HitObjectParser
    {
        public static List<HitObject> Parse(string filename)
        {
            var data = new List<HitObject>();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Find HitObject Section.
                    if (currentLine == "[HitObjects]")
                    {
                        while ((currentLine = reader.ReadLine()) != null)
                        {
                            if (currentLine.Length == 0)
                                break;

                            var parsed = currentLine.Split(',');
                            if (parsed.Length != 6 && parsed.Length != 7 && parsed.Length != 8 && parsed.Length != 11)
                                throw new InvalidBeatmapException("Wrong HitObject Found.");

                            var temp = new HitObject
                            {
                                //  X-coordinate
                                X = Convert.ToInt32(parsed[0]),

                                //  Y-coordinate
                                Y = Convert.ToInt32(parsed[1]),

                                //  Time
                                Time = Convert.ToInt32(parsed[2]),

                                //  Type
                                Type = Convert.ToInt32(parsed[3]),

                                //  Hitsound
                                Hitsound = Convert.ToInt32(parsed[4])
                            };

                            if (parsed.Length >= 8)
                            {
                                var sliderinfo = parsed[5].Split('|');

                                //  Slider Type
                                temp.SliderType = sliderinfo[0];

                                //  Curve X, Y coordinates
                                var tempList = new List<Tuple<int, int>>();
                                for (var i = 1; i < sliderinfo.Length; i++)
                                    tempList.Add(Tuple.Create(Convert.ToInt32(sliderinfo[i].Split(':')[0]), 
                                        Convert.ToInt32(sliderinfo[i].Split(':')[1])));
                                temp.Curves = tempList;

                                //  Repeat
                                temp.Repeat = Convert.ToInt32(parsed[6]);

                                //  Pixel Length
                                temp.PixelLength = Convert.ToDouble(parsed[7]);

                                if (parsed.Length == 11)
                                {
                                    //  Edge Hitsound
                                    temp.EdgeHitsound = Tuple.Create(Convert.ToInt32(parsed[8].Split('|')[0]),
                                        Convert.ToInt32(parsed[8].Split('|')[1]));

                                    //  Edge Addition
                                    temp.EdgeAddition = Tuple.Create(Convert.ToInt32(parsed[9].Split('|')[0].Split(':')[0]),
                                        Convert.ToInt32(parsed[9].Split('|')[0].Split(':')[1]),
                                        Convert.ToInt32(parsed[9].Split('|')[1].Split(':')[0]),
                                        Convert.ToInt32(parsed[9].Split('|')[1].Split(':')[1]), "");
                                }
                            }

                            if (parsed.Length != 8)
                            {
                                var addition = parsed[parsed.Length - 1].Split(':');

                                //  Endtime for spinner
                                if (parsed.Length == 7)
                                    temp.Endtime = Convert.ToInt32(parsed[5]);

                                //  Endtime for Mania LN
                                else if (parsed.Length == 6 && addition.Length == 6)
                                    temp.Endtime = Convert.ToInt32(addition[0]);

                                //  Addition
                                if (addition.Length == 4)
                                    temp.Addition = Tuple.Create(Convert.ToInt32(addition[addition.Length - 4]),
                                        Convert.ToInt32(addition[addition.Length - 3]),
                                        Convert.ToInt32(addition[addition.Length - 2]), 0, "");
                                else
                                    temp.Addition = Tuple.Create(Convert.ToInt32(addition[addition.Length - 5]),
                                        Convert.ToInt32(addition[addition.Length - 4]),
                                        Convert.ToInt32(addition[addition.Length - 3]),
                                        Convert.ToInt32(addition[addition.Length - 2]),
                                        addition[addition.Length -1]);
                            }

                            data.Add(temp);
                        }
                    }
                }
            }

            return data;
        }
    }
}