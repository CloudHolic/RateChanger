using System;
using System.IO;
using System.Linq;

using RateChanger.Structures;

namespace RateChanger.Parser
{
    public static class MetadataParser
    {
        public static Metadata Parse(string filename)
        {
            var data = new Metadata();

            using (var reader = new StreamReader(filename))
            {
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    //  Title
                    if (currentLine.StartsWith("Title:"))
                        data.Title = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Unicode Title
                    if (currentLine.StartsWith("TitleUnicode:"))
                        data.TitleUnicode = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Artist
                    if (currentLine.StartsWith("Artist:"))
                        data.Artist = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Unicode Artist
                    if (currentLine.StartsWith("ArtistUnicode:"))
                        data.ArtistUnicode = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Creator
                    if (currentLine.StartsWith("Creator:"))
                        data.Creator = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Difficulty
                    if (currentLine.StartsWith("Version:"))
                        data.Version = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Source
                    if (currentLine.StartsWith("Source:"))
                        data.Source = currentLine.Substring(currentLine.IndexOf(':') + 1);

                    //  Tags
                    if (currentLine.StartsWith("Tags:"))
                        data.Tags = currentLine.Substring(currentLine.IndexOf(':') + 1).Split(' ').Select(cur => cur).ToList();

                    //  Beatmap ID
                    if (currentLine.StartsWith("BeatmapID:"))
                        data.BeatmapID = Convert.ToInt32(currentLine.Split(':')[1]);

                    //  Beatmapset ID
                    if (currentLine.StartsWith("BeatmapSetID:"))
                    {
                        data.BeatmapSetID = Convert.ToInt32(currentLine.Split(':')[1]);
                        break;
                    }
                }
            }

            return data;
        }
    }
}
