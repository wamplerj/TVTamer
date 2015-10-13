using System.Web.Mvc;
using TvTamer.Web.Models;

namespace TvTamer.Web.Controllers
{
    public class AnalyticsController : Controller
    {
        // GET: Analytics
        [HttpGet]
        public ActionResult GetRecentActivity()
        {
            var chartData = new RecentActivityChart()
            {
                DaysOfWeek = new[] {"Tue 10/6", "Wed 10/7", "Thu 10/8", "Fri 10/9", "Sat 10/10", "Sun 10/11", "Mon 10/12"},
                EpisodeDownloads = new[] {5, 2, 6, 3, 1, 13, 7},
                SearchAttempts = new[] {25, 32, 64, 13, 6, 88, 32},
                SearchFailures = new[] {20, 30, 58, 10, 5, 75, 25},
                ProcessedEpisodes = new[] {5, 2, 6, 3, 1, 12, 7},
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