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
            var series = _context.TvSeries.FirstOrDefault(s => s.Id == id);

            if (series == null)
                return RedirectToAction("Index", "Series");

            var seasons = _context.QuerySql<int>($"select Distinct(Season) from TvEpisodes where seriesId = {series.Id} and Season > 0");

            var minSeason = seasons.Min();

            var episodes =
                _context.TvEpisodes.Where(e => e.SeriesId == series.Id && e.Season == minSeason).OrderBy(e => e.Season).ThenBy(e => e.EpisodeNumber);
            series.Episodes = episodes.ToList();

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
