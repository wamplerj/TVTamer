using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TvTamer.Web.Models
{
    public class RecentActivityChart
    {
        public string[] DaysOfWeek { get; set; }

        public int[] SearchAttempts { get; set; }
        public int[] SearchFailures { get; set; }
        public int[] EpisodeDownloads { get; set; }
        public int[] ProcessedEpisodes { get; set; }

    }
}