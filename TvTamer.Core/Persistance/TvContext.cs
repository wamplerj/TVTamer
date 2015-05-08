using System;
using System.Data.Entity;
using System.Linq;
using TvTamer.Core.Models;

namespace TvTamer.Core.Persistance
{
    public class TvContext : DbContext
    {
        public IDbSet<TvSeries> TvSeries { get; set; }
        public IDbSet<TvEpisode> TvEpisodes { get; set; }
        public IDbSet<LoggedEvent> LoggedEvents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            LoadMaps(modelBuilder);
        }

        protected void LoadMaps(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TvSeriesMap());
            modelBuilder.Configurations.Add(new TvEpisodeMap());
            modelBuilder.Configurations.Add(new LoggedEventMap());

        }

    }
}
