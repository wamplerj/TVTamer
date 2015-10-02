using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TvTamer.Core;
using TvTamer.Core.Persistance;
using System.Web.Mvc;
using System.IO;
using TvTamer.Web.Models;

namespace TvTamer.Web.Controllers
{
    public class SeriesController : Controller
    {
        private readonly ITvSearchService _searchService;
        private readonly ITvService _service;

        public SeriesController(ITvSearchService searchService, ITvService service)
        {
            _searchService = searchService;
            _service = service;
        }

        public ActionResult Index()
        {
            var series = _service.GetTvSeries();

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
            var series = _service.GetTvSeriesById(id);

            if (series == null)
                return RedirectToAction("Index", "Series");

            var seasons = _service.GetSeasons(id);
            seasons.Reverse();
            var lastSeason = seasons.Max();

            var episodes = _service.GetEpisodesBySeason(id, lastSeason);
            series.Episodes = episodes;

            var model = new SeriesDetailsViewModel()
            {
                Series = series,
                Seasons = seasons
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(string seriesId)
        {
            var series = _searchService.GetTvSeries(seriesId);
            _service.AddOrUpdate(series);
            _service.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}
