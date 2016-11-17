using System;
using System.IO;

using RateChanger.Structures;

namespace RateChanger.Parser
{
    public static class GeneralParser
    {
        public static General Parse(string filename)
        {
            var data = new General();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Audio File Name
                    if (currentLine.StartsWith("AudioFilename:"))
                        data.AudioFilename = currentLine.Substring(currentLine.IndexOf(' ') + 1);

                    //  Audio Leadin
                    if (currentLine.StartsWith("AudioLeadIn:"))
                        data.AudioLeadIn = Convert.ToInt32(currentLine.Split(':')[1]);

                    //  Preview Time
                    if (currentLine.StartsWith("PreviewTime:"))
                        data.PreviewTime = Convert.ToInt32(currentLine.Split(':')[1]);

                    //  Countdown
                    if (currentLine.StartsWith("Countdown:"))
                        data.Countdown = Convert.ToInt32(currentLine.Split(':')[1]) == 1;

                    //  Sample Set
                    if (currentLine.StartsWith("SampleSet:"))
                        data.SampleSet = currentLine.Substring(currentLine.IndexOf(' ') + 1);

                    //  Stack Leniency
                    if (currentLine.StartsWith("StackLeniency:"))
                        data.StackLeniency = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  Mode
                    if (currentLine.StartsWith("Mode:"))
                        data.Mode = Convert.ToInt32(currentLine.Split(':')[1]);

                    //  Letterbox In Break
                    if (currentLine.StartsWith("LetterboxInBreaks:"))
                        data.LetterboxInBreaks = Convert.ToInt32(currentLine.Split(':')[1]) == 1;

                    //  Special Style
                    if (currentLine.StartsWith("SpecialStyle:"))
                        data.SpecialStyle = Convert.ToInt32(currentLine.Split(':')[1]) == 1;

                    //  Widescreen Storyboard
                    if (currentLine.StartsWith("WidescreenStoryboard:"))
                    {
                        data.WidescreenStoryboard = Convert.ToInt32(currentLine.Split(':')[1]) == 1;
                        break;
                    }
                }
            }

            return data;
        }
    }
}
