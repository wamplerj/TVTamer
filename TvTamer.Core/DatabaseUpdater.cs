using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Linq;
using NLog;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

namespace TvTamer.Core
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

        private List<string> GetSeriesIds(List<string> updatedSeriesIds = null)
        {
            var seriesIds = _context.TvSeries.Select(s => s.SeriesId).ToList();
            if (updatedSeriesIds == null) return seriesIds;

            return updatedSeriesIds.Intersect(seriesIds).ToList();
        }

        private void UpdateSeriesList(List<string> seriesIds)
        {
            foreach (var id in seriesIds)
            {
                var tvSeries = _searchService.GetTvSeries(id);
                _logger.Info("Updating {0}...", tvSeries.Name);
                _context.TvSeries.AddOrUpdate(t => t.SeriesId, tvSeries);
            }

            _context.LoggedEvents.Add(new LoggedEvent()
            {
                EventTime = DateTime.Now,
                EventType = LoggedEventType.DatabaseUpdate,
                Message = "Updating Database from TvDB"
            });

            _context.SaveChanges();
        }

        public void Update()
        {

            _logger.Info("Updating TV Database...");

            var seriesList = _context.TvSeries
                .Where(s => SqlFunctions.DateDiff("day", s.LastUpdated, DateTime.Now) >= 7)
                .Select(s => new {s.SeriesId, s.Name ,s.LastUpdated}).ToList();

            foreach (var series in seriesList)
            {

                if (!_searchService.HasUpdates(series.SeriesId, series.LastUpdated)) continue;

                _logger.Info("Updates found for Series: {0}", series.Name);
                var updatedSeries = _searchService.GetTvSeries(series.SeriesId);

                _context.TvSeries.AddOrUpdate(t => t.SeriesId, updatedSeries);

            }

            _context.SaveChanges();

            //var epochLastUpdateTime = lastTimeEvent.EventTime.ToEpochTime();
            //var updates = _searchService.GetUpdates(epochLastUpdateTime);
            //var seriesIds = GetSeriesIds(updates.SeriesIds);

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
