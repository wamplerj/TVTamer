using System;
using System.Data.Entity;
using System.Linq;
using TvTamer.Core.Models;

namespace TvTamer.Core.Persistance
{
    public interface ITvContext
    {
        IDbSet<TvSeries> TvSeries { get; set; }
        IDbSet<TvEpisode> TvEpisodes { get; set; }
        IDbSet<LoggedEvent> LoggedEvents { get; set; }

        Database Database { get;}
        int SaveChanges();
    }

    public class TvContext : DbContext, ITvContext
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
