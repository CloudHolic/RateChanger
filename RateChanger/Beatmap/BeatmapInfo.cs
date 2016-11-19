using System.Collections.Generic;
using System.IO;

using RateChanger.CustomExceptions;
using RateChanger.Parser;
using RateChanger.Structures;

// ReSharper disable InconsistentNaming
namespace RateChanger.Beatmap
{
    //  Represents a beatmap. It contains all information about a single osu-file.
    public class BeatmapInfo
    {
        public General Gen { get; set; }
        public Editor Edit { get; set; }
        public Metadata Meta { get; set; }
        public Difficulty Diff { get; set; }
        public List<TimingPoint> Timing { get; set; }
        public List<Colours> Color { get; set; }

        public BeatmapInfo()
        {
            Gen = new General();
            Edit = new Editor();
            Meta = new Metadata();
            Diff = new Difficulty();
            Timing = new List<TimingPoint>();
            Color = new List<Colours>();

            Timing.Add(new TimingPoint());
            Color.Add(new Colours());
        }

        public BeatmapInfo(string filename)
        {
            //  Load, and parse.
            if (File.Exists(filename))
            {
                if (filename.Split('.')[filename.Split('.').Length - 1] != "osu")
                    throw new InvalidBeatmapException("Unknown file format.");

                Gen = GeneralParser.Parse(filename);
                Edit = EditorParser.Parse(filename);
                Meta = MetadataParser.Parse(filename);
                Diff = DifficultyParser.Parse(filename);
                Timing = TimingPointParser.Parse(filename);
                Color = ColourParser.Parse(filename);
            }
            else
                throw new FileNotFoundException();
        }

        public BeatmapInfo(BeatmapInfo prevBeatmapInfo)
        {
            Gen = new General(prevBeatmapInfo.Gen);
            Edit = new Editor(prevBeatmapInfo.Edit);
            Meta = new Metadata(prevBeatmapInfo.Meta);
            Diff = new Difficulty(prevBeatmapInfo.Diff);

            Timing = new List<TimingPoint>();
            foreach (var cur in prevBeatmapInfo.Timing)
                Timing.Add(new TimingPoint(cur));
            
            Color = new List<Colours>();
            foreach(var cur in prevBeatmapInfo.Color)
                Color.Add(new Colours(cur));
        }
    }
}