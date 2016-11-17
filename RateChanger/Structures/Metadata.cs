using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace RateChanger.Structures
{
    public class Metadata
    {
        public Metadata()
        {
            Title = null;
            TitleUnicode = null;
            Artist = null;
            ArtistUnicode = null;
            Creator = null;
            Version = null;

            Source = null;
            Tags = new List<string>();
            BeatmapID = -1;
            BeatmapSetID = -1;
        }

        public Metadata(string title, string utitle, string artist, string uartist, string creator, string ver,
            string source, List<string> tags, int mapId, int mapsetId)
        {
            Title = title;
            TitleUnicode = utitle;
            Artist = artist;
            ArtistUnicode = uartist;
            Creator = creator;
            Version = ver;

            Source = source;
            Tags = tags;
            BeatmapID = mapId;
            BeatmapSetID = mapsetId;
        }

        public Metadata(Metadata prevMetadata)
        {
            Title = prevMetadata.Title;
            TitleUnicode = prevMetadata.TitleUnicode;
            Artist = prevMetadata.Artist;
            ArtistUnicode = prevMetadata.ArtistUnicode;
            Creator = prevMetadata.Creator;
            Version = prevMetadata.Version;

            Source = prevMetadata.Source;
            Tags = new List<string>(prevMetadata.Tags);
            BeatmapID = prevMetadata.BeatmapID;
            BeatmapSetID = prevMetadata.BeatmapSetID;
        }

        public string Title { get; set; }

        public string TitleUnicode { get; set; }

        public string Artist { get; set; }

        public string ArtistUnicode { get; set; }

        public string Creator { get; set; }

        public string Version { get; set; }

        public string Source { get; set; }

        public List<string> Tags { get; set; }

        public int BeatmapID { get; set; }

        public int BeatmapSetID { get; set; }
    }
}
