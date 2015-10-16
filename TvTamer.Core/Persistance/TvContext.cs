using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using NLog;
using TvTamer.Core.Models;

namespace TvTamer.Core.Persistance
{
    public interface ITvContext
    {
        IDbSet<TvSeries> TvSeries { get; set; }
        IDbSet<TvEpisode> TvEpisodes { get; set; }
        IDbSet<LoggedEvent> LoggedEvents { get; set; }
        DbSet<T> Set<T>() where T: class;

        List<T> QuerySql<T>(string query);
        List<T> QuerySql<T>(string query, params object[] parameters);

        int SaveChanges();
    }

    public class TvContext : DbContext, ITvContext
    {
        public TvContext()
        {
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif
        }

        public IDbSet<TvSeries> TvSeries { get; set; }
        public IDbSet<TvEpisode> TvEpisodes { get; set; }
        public IDbSet<LoggedEvent> LoggedEvents { get; set; }

        public List<T> QuerySql<T>(string query)
        {
            return Database.SqlQuery<T>(query).ToList();
        }

        public List<T> QuerySql<T>(string query, params object[] parameters)
        {
            return Database.SqlQuery<T>(query, parameters).ToList();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            LoadMaps(modelBuilder);
        }

        protected void LoadMaps(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TvSeriesMap());
            modelBuilder.Configurations.Add(new TvEpisodeMap());
            modelBuilder.Configurations.Add(new AlternateNameMap());
            modelBuilder.Configurations.Add(new LoggedEventMap());
        }

    }
}
