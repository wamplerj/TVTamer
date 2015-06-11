using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Linq;
using NLog;

namespace TvTamer.Core.Persistance
{
    public interface IDatabaseUpdater
    {
        void Update();
    }

    public class DatabaseUpdater : IDatabaseUpdater
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

                var currentSeries = _context.TvSeries.Include(s => s.Episodes).FirstOrDefault(s => s.SeriesId == series.SeriesId);
                var updatedSeries = _searchService.GetTvSeries(series.SeriesId);

                if (currentSeries == null)
                {
                    _context.TvSeries.AddOrUpdate(updatedSeries);
                    _context.SaveChanges();
                    return;
                }

                currentSeries.Name = updatedSeries.Name;
                currentSeries.Network = updatedSeries.Network;
                currentSeries.AirsDayOfWeek = updatedSeries.AirsDayOfWeek;
                currentSeries.AirsTimeOfDay = updatedSeries.AirsTimeOfDay;
                currentSeries.FirstAired = updatedSeries.FirstAired;
                currentSeries.Rating = updatedSeries.Rating;
                currentSeries.Status = updatedSeries.Status;
                currentSeries.Summary = updatedSeries.Summary;
                currentSeries.LastUpdated = DateTime.Now;

                foreach (var episode in updatedSeries.Episodes)
                {

                    var currentEpisode =
                        currentSeries.Episodes.FirstOrDefault(
                            e => e.Season == episode.Season && e.EpisodeNumber == episode.EpisodeNumber);

                    if (currentEpisode == null)
                    {
                        episode.DownloadStatus = "WANT";
                        currentSeries.Episodes.Add(episode);
                        continue;
                    }

                    currentEpisode.FirstAired = episode.FirstAired;
                    currentEpisode.Summary = episode.Summary;
                    currentEpisode.Title = episode.Title;
                   
                }

                _context.TvSeries.AddOrUpdate(t => t.SeriesId, currentSeries);
            }

            _context.SaveChanges();

            _logger.Info("TV Database Update complete.");
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
