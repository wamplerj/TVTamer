using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvTamer.Core
{
    public class TvEpisode
    {
        public string SeriesName { get; set; }
        public string Title { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public string FileName { get; set; }
    }
}
