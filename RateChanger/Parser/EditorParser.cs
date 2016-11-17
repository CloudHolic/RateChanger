using System;
using System.IO;
using System.Linq;

using RateChanger.Structures;

namespace RateChanger.Parser
{
    public static class EditorParser
    {
        public static Editor Parse(string filename)
        {
            var data = new Editor();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Bookmarks
                    if (currentLine.StartsWith("Bookmarks:"))
                        data.Bookmarks = currentLine.Substring(currentLine.IndexOf(':') + 1).Split(',')
                            .Select(cur => Convert.ToInt32(cur)).ToList();

                    //  Distance Spacing
                    if (currentLine.StartsWith("DistanceSpacing:"))
                        data.DistanceSpacing = Convert.ToDouble(currentLine.Split(':')[1]);

                    //  Beat Divisor
                    if (currentLine.StartsWith("BeatDivisor:"))
                        data.BeatDivisor = Convert.ToInt32(currentLine.Split(':')[1]);

                    //  Grid Size
                    if (currentLine.StartsWith("GridSize:"))
                        data.GridSize = Convert.ToInt32(currentLine.Split(':')[1]);

                    //  Timeline Zoom
                    if (currentLine.StartsWith("TimelineZoom:"))
                    {
                        data.TimelineZoom = Convert.ToDouble(currentLine.Split(':')[1]);
                        break;
                    }
                }
            }

            return data;
        }
    }
}
