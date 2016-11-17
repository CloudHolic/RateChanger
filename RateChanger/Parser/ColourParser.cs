using System;
using System.Collections.Generic;
using System.IO;

using RateChanger.CustomExceptions;
using RateChanger.Structures;

namespace RateChanger.Parser
{
    public static class ColourParser
    {
        public static List<Colours> Parse(string filename)
        {
            var data = new List<Colours>();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Find Colours Section.
                    if (currentLine == "[Colours]")
                    {
                        while ((currentLine = reader.ReadLine()) != null)
                        {
                            if (!currentLine.StartsWith("Combo"))
                                break;
                            
                            var parsed = currentLine.Substring(currentLine.IndexOf(':') + 1).Split(',');
                            if (parsed.Length != 3)
                                throw new InvalidBeatmapException("Wrong Colour Found.");

                            var temp = new Colours
                            {
                                //  Number
                                Number = Convert.ToInt32(currentLine.Substring(5).Split(':')[0].Trim()),

                                //  Red
                                Red = Convert.ToInt32(parsed[0]),
                                
                                //  Green
                                Green = Convert.ToInt32(parsed[1]),

                                //  Blue
                                Blue = Convert.ToInt32(parsed[2])
                            };

                            data.Add(temp);
                        }
                    }
                }
            }

            if(data.Count == 0)
                data.Add(new Colours());

            return data;
        }
    }
}
