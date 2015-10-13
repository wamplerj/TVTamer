using System.Linq;
using System.Web.Mvc;
using TvTamer.Core;
using TvTamer.Web.Models;

namespace TvTamer.Web.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // GET: Analytics
        [HttpGet]
        public ActionResult GetRecentActivity()
        {
            //TODO Introduce caching.  Maybe 6 hours?
            var recentActivity = _analyticsService.GetRecentActivity().ToList();

            var daysOfWeek = recentActivity.Select(ra => ra.EventDay.ToString("ddd M/d")).ToArray();
            var episodeDownloads = recentActivity.Select(ra => (int)ra.Downloads).ToArray();
            var searchAttempts = recentActivity.Select(ra => (int)ra.SearchAttempts).ToArray();
            var searchFailures = recentActivity.Select(ra => (int)ra.SearchFailures).ToArray();
            var processedEpisodes = recentActivity.Select(ra => (int)ra.ProcessedEpisodes).ToArray();

            var chartData = new RecentActivityChart()
            {
                DaysOfWeek = daysOfWeek,
                EpisodeDownloads = episodeDownloads,
                SearchAttempts = searchAttempts,
                SearchFailures = searchFailures,
                ProcessedEpisodes = processedEpisodes,
            };

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json",
                Data = chartData
            };
        }
    }
}