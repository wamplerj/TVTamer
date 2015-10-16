using System;
using System.Linq;
using System.Web.Mvc;
using TvTamer.Core;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;
using TvTamer.Web.Models;

namespace TvTamer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITvService _tvService;

        public HomeController(ITvService tvService)
        {
            _tvService = tvService;
        }

        public ActionResult Index()
        {

            var todaysEpisodes = _tvService.GetEpisodesByDate(DateTime.Today);
            var recentlyDownloadedEpisodes = _tvService.GetRecentlyDownloadedEpisodes();

            var model = new HomeViewModel() {TodaysEpisodes = todaysEpisodes, RecentlyDownloadedEpisodes = recentlyDownloadedEpisodes };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}