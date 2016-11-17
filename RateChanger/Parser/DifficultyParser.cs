using System;
using System.IO;

using RateChanger.Structures;

namespace RateChanger.Parser
{
    public static class DifficultyParser
    {
        public static Difficulty Parse(string filename)
        {
            var data = new Difficulty();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  HP
                    if (currentLine.StartsWith("HPDrainRate:"))
                        data.HPDrainRate = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  CS
                    if (currentLine.StartsWith("CircleSize:"))
                        data.CircleSize = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  OD
                    if (currentLine.StartsWith("OverallDifficulty:"))
                        data.OverallDifficulty = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  AR
                    if (currentLine.StartsWith("Approachrate:"))
                        data.ApproachRate = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  Slider Multiplier
                    if (currentLine.StartsWith("SliderMultiplier:"))
                        data.SliderMultiplier = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  Slider Tick Rate
                    if (currentLine.StartsWith("SliderTickRate:"))
                    {
                        data.SliderTickRate = Convert.ToDouble(currentLine.Split(':')[1]);
                        break;
                    }
                }
            }

            return data;
        }
    }
}