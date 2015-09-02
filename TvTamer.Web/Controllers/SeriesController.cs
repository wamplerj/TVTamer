using System.Data.Entity;
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
        private readonly ITvContext _context;

        public SeriesController(ITvSearchService searchService, ITvContext context)
        {
            _searchService = searchService;
            _context = context;
        }

        public ActionResult Index()
        {
            var series = _context.TvSeries.OrderBy(s => s.Name).ToList();

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


        [HttpGet]
        public ActionResult Details(int id)
        {
            var series = _context.TvSeries.Include(s => s.Episodes).FirstOrDefault(s => s.Id == id);

            return View(series);
        }

        [HttpPost]
        public ActionResult Save(string seriesId)
        {
            var series = _searchService.GetTvSeries(seriesId);

            _context.TvSeries.Add(series);
            _context.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult ImportDirectory()
        {

            var seriesFolders = Directory.GetDirectories(@"\\wampler-server\Storage\Media\TV");

            foreach (var seriesFolder in seriesFolders)
            {

                var showName = seriesFolder.Split('\\').Last();

                if (showName.Contains('\\')) continue;

                var tvShows = _searchService.FindTvSeries(showName).ToList();

                if (!tvShows.Any()) continue;

                var tvSeries = _searchService.GetTvSeries(tvShows[0].SeriesId);

                if (tvSeries == null) continue;

                _context.TvSeries.Add(tvSeries);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}
