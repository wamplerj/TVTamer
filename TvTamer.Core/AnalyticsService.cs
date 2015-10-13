using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

namespace TvTamer.Core
{
    public enum AnalyticEvent
    {
        DbUpdate,
        Download,
        DownloadFailed,
        Episode,
        EpisodeFailed,
        Search,
        SearchFailed
    }

    public interface IAnalyticsService
    {
        IEnumerable<Activity> GetRecentActivity();


        void ReportEvent(AnalyticEvent type);
        void ReportEvent(AnalyticEvent type, string message);
    }

    public class Activity
    {
        private int _processedEpisodes;
        private int _searchAttempts;
        private int _searchFailures;
        private int _downloads;

        public DateTime EventDay { get; set; }

        public int? Downloads
        {
            get { return _downloads; }
            set { _downloads = value ?? 0; }
        }

        public int? SearchAttempts
        {
            get { return _searchAttempts; }
            set { _searchAttempts = value ?? 0; }
        }

        public int? SearchFailures
        {
            get { return _searchFailures; }
            set { _searchFailures = value ?? 0; }
        }

        public int? ProcessedEpisodes
        {
            get { return _processedEpisodes; }
            set { _processedEpisodes = value ?? 0; }
        }
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly ITvContext _context;

        public AnalyticsService(ITvContext context)
        {
            _context = context;
        }


        public IEnumerable<Activity> GetRecentActivity()
        {
            var recentActivity = _context.QuerySql<Activity>("[dbo].[GetRecentActivity]");
            return recentActivity;
        }

        public void ReportEvent(AnalyticEvent type)
        {
            ReportEvent(type, null);
        }

        public void ReportEvent(AnalyticEvent type, string message)
        {
            var eventType = Enum.GetName(typeof (AnalyticEvent), type).ToUpper();

            //TODO increase message size
            if (!string.IsNullOrEmpty(message) && message.Length > 255)
                message = message.Substring(0, 254);

            _context.LoggedEvents.AddOrUpdate(new LoggedEvent()
            {
                EventTime = DateTime.Now,
                EventType = eventType,
                Message = message
            });
            _context.SaveChanges();
        }
    }
}