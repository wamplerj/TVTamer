using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

namespace TvTamer.Core
{

    public interface ITvService
    {
        int SaveChanges();

        void AddOrUpdate(TvEpisode episode);

        TvEpisode GetEpisodeBySeriesName(string seriesName, int season, int episodeNumber, bool searchByAlternateName = false);
    }

    public class TvService : ITvService
    {
        private readonly ITvContext _context;

        public TvService(ITvContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
           return _context.SaveChanges();
        }

        public void AddOrUpdate(TvEpisode episode)
        {
            _context.TvEpisodes.AddOrUpdate(e => e.Id, episode);
        }

        public TvEpisode GetEpisodeBySeriesName(string seriesName, int season, int episodeNumber, bool searchByAlternateName = false)
        {

            var query =
                $@"SELECT TOP 1 s.Id, S.SeriesId, s.Name, s.FirstAired, s.AirsDayOfWeek, s.AirsTimeOfDay, s.Network, s.Summary, s.Status, s.Rating, s.LastUpdated FROM [dbo].[AlternateNames] an
                              INNER JOIN TVSeries s
                              ON s.Id = an.SeriesId
                              WHERE s.Name = '{seriesName}'";

            if(searchByAlternateName)
                query += " OR an.Name = '{seriesName}'";

            var tvSeries =  _context.Set<TvSeries>().SqlQuery(query).AsNoTracking().FirstOrDefault(); //DBSet<T>.SqlQuery is mockable

            return
                _context.TvEpisodes.FirstOrDefault(
                    e => e.SeriesId == tvSeries.Id && e.Season == season && e.EpisodeNumber == episodeNumber);

        }
    }
}
