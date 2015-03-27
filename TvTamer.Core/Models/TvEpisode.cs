using System;

namespace TvTamer.Core.Models
{
    public class TvEpisode
    {
        public string SeriesName { get; set; }
        public string Title { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public string FileName { get; set; }
        public string Summary { get; set; }
        public DateTime FirstAired { get; set; }
    }
}
