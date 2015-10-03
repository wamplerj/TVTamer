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
        void ReportEvent(AnalyticEvent type);
        void ReportEvent(AnalyticEvent type, string message);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly ITvContext _context;

        public AnalyticsService(ITvContext context)
        {
            _context = context;
        }


        public void ReportEvent(AnalyticEvent type)
        {
            ReportEvent(type, null);
        }

        public void ReportEvent(AnalyticEvent type, string message)
        {

            var eventType = Enum.GetName(typeof (AnalyticEvent), type).ToUpper();

            _context.LoggedEvents.AddOrUpdate(new LoggedEvent() { EventTime = DateTime.Now, EventType = eventType, Message = message});
            _context.SaveChanges();

        }

    }
}
