using System.Linq;
using TvTamer.Core;
using TvTamer.Core.Persistance;
using System.Web.Mvc;
using System.IO;

namespace TvTamer.Web.Controllers
{
    public class SeriesController : Controller
    {
        private readonly ITvSearchService _searchService;

        public SeriesController(ITvSearchService searchService)
        {
            _searchService = searchService;
        }

        public ActionResult Index()
        {
            var context = new TvContext();
            var series = context.TvSeries.ToList();

            return View(series);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string seriesName)
        {

            var series = _searchService.FindTvSeries(seriesName);

            return View("ChooseSeriesResult",series);
        }

        [HttpPost]
        public ActionResult Save(string seriesId)
        {
            var series = _searchService.GetTvSeries(seriesId);

            var context = new TvContext();
            context.TvSeries.Add(series);
            context.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult ImportDirectory()
        {

            var seriesFolders = Directory.GetDirectories(@"\\wampler-server\Storage\Media\TV");
            var context = new TvContext();

            foreach (var seriesFolder in seriesFolders)
            {

                var showName = seriesFolder.Split('\\').Last();

                if (showName.Contains('\\')) continue;

                var tvShows = _searchService.FindTvSeries(showName).ToList();

                if (tvShows.Count() == 0) continue;

                var tvSeries = _searchService.GetTvSeries(tvShows[0].SeriesId);

                if (tvSeries == null) continue;

                context.TvSeries.Add(tvSeries);
            }

            context.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}
