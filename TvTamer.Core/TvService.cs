using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

namespace TvTamer.Core
{

    public interface ITvService
    {
        int SaveChanges();

        void AddOrUpdate(TvSeries series);
        void AddOrUpdate(TvEpisode episode);

        TvSeries GetTvSeriesById(int seriesId);
        List<TvSeries> GetTvSeries(int page = 0, int pageSize = 500);

        List<int> GetSeasons(int seriesId);
        

        TvEpisode GetEpisodeBySeriesName(string seriesName, int season, int episodeNumber, bool searchByAlternateName = false);
        List<TvEpisode> GetEpisodesBySeason(int id, int lastSeason);
    }

    public class TvService : ITvService
    {
        private readonly ITvContext _context;
        private readonly Logger _logger = LogManager.GetLogger("log");

        public TvService(ITvContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
           return _context.SaveChanges();
        }

        public void AddOrUpdate(TvSeries series)
        {
            _context.TvSeries.Add(series);
        }

        public void AddOrUpdate(TvEpisode episode)
        {
            _context.TvEpisodes.AddOrUpdate(e => e.Id, episode);
        }

        public TvSeries GetTvSeriesById(int seriesId)
        {
            return _context.TvSeries
                .Include(s => s.AlternateNames)
                .FirstOrDefault(s => s.Id == seriesId);
        }

        public List<TvSeries> GetTvSeries(int page=0, int pageSize = 500)
        {
            return _context.TvSeries.OrderBy(s => s.Name).Take(pageSize).Skip(pageSize * page).ToList();
        }

        public List<int> GetSeasons(int seriesId)
        {
            return _context.QuerySql<int>($"select Distinct(Season) from TvEpisodes where seriesId = {seriesId} and Season > 0").ToList();
        }

        public TvEpisode GetEpisodeBySeriesName(string seriesName, int season, int episodeNumber, bool searchByAlternateName = false)
        {

            var query =
                $@"SELECT TOP 1 s.Id, s.TvDbSeriesId, s.Name, s.FirstAired, s.AirsDayOfWeek, s.AirsTimeOfDay, s.Network, s.Summary, s.Status, s.Rating, s.LastUpdated FROM [dbo].[AlternateNames] an
                   RIGHT JOIN TVSeries s ON s.Id = an.SeriesId WHERE s.Name = '{seriesName}'";

            if(searchByAlternateName)
                query += $" OR an.Name = '{seriesName}'";

            _logger.Debug($"GetEpisodeBySeriesName on TVContext, SQL Query: {query}");

            var tvSeries =  _context.Set<TvSeries>().SqlQuery(query).AsNoTracking().FirstOrDefault(); //DBSet<T>.SqlQuery is mockable

            if (tvSeries == null)
            {
                _logger.Info($"GetEpisodeBySeriesName on TVContext, TV Series: {seriesName} could not be found in the database.  searchByAlternateName={searchByAlternateName}");
                return null;
            }

            return
                _context.TvEpisodes.FirstOrDefault(
                    e => e.SeriesId == tvSeries.Id && e.Season == season && e.EpisodeNumber == episodeNumber);

        }

        public List<TvEpisode> GetEpisodesBySeason(int seriesId, int lastSeason)
        {
            return _context.TvEpisodes.Where(e => e.SeriesId == seriesId && e.Season == lastSeason).OrderBy(e => e.Season).ThenBy(e => e.EpisodeNumber).ToList();
        }
    }
}
