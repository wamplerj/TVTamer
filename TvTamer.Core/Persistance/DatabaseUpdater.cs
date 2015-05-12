using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Linq;
using NLog;

namespace TvTamer.Core.Persistance
{
    public class DatabaseUpdater
    {
        private readonly ITvSearchService _searchService;
        private readonly TvContext _context;
        private readonly Logger _logger = LogManager.GetLogger("log");

        public DatabaseUpdater(ITvSearchService searchService, TvContext context)
        {
            _searchService = searchService;
            _context = context;
        }

        public void Update()
        {

            _logger.Info("Updating TV Database...");

            var seriesList = _context.TvSeries
                .Where(s => SqlFunctions.DateDiff("day", s.LastUpdated, DateTime.Now) >= 7)
                .Select(s => new {s.Id, s.SeriesId, s.Name ,s.LastUpdated}).ToList();

            foreach (var series in seriesList)
            {

                if (!_searchService.HasUpdates(series.SeriesId, series.LastUpdated)) continue;

                _logger.Info("Updates found for Series: {0}", series.Name);
                var updatedSeries = _searchService.GetTvSeries(series.SeriesId);

                _context.TvSeries.AddOrUpdate(t => t.SeriesId, updatedSeries);


                foreach (var episode in updatedSeries.Episodes)
                {
                    episode.SeriesId = series.Id;
                    _context.TvEpisodes.AddOrUpdate(e => e.SeriesId, episode);
                }
            }

            _context.SaveChanges();
        }
    }

    public static class DateTimeExtentions
    {
        public static long ToEpochTime(this DateTime lastTime)
        {
            return ((DateTimeOffset)lastTime).ToUnixTimeSeconds();
        }

        public static DateTime FromEpochTime(this long lastEpochTime)
        {
            var lastUpdateOffset = DateTimeOffset.FromUnixTimeSeconds(lastEpochTime);
            return lastUpdateOffset.DateTime;
        }
    }
}
