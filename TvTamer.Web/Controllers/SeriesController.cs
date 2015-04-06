using System.Linq;
using TvTamer.Core;
using TvTamer.Core.Persistance;
using System.Web.Mvc;

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

            var series = _searchService.FindTvShow(seriesName);

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
    }
}
