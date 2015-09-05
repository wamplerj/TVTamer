using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;
using TvTamer.Web.Models;

namespace TvTamer.Web.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly ITvContext _context;

        public EpisodeController(ITvContext context)
        {
            _context = context;
        }


        // GET: Episode
        public ActionResult Calendar()
        {
            var query = @"SELECT 
                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], DATENAME(dw,e.[FirstAired]) As [DayOfWeek], e.[DownloadStatus] AS [DownloadStatus], 
                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
                WHERE (DATEDIFF(day, SysDateTime(), e.[FirstAired])) BETWEEN 0 AND 6 AND N'WANT' = e.[DownloadStatus] ORDER BY e.[FirstAired] ASC";

            var thisWeeksEpisodes = _context.QuerySql<TvEpisode>(query).GroupBy(e => e.FirstAired).ToDictionary(e => e.Key, o => o.ToList());

            var model = new CalendarViewModel() { Episodes = thisWeeksEpisodes};

            return View(model);

        }

        [HttpPost]
        public ActionResult UpdateDownloadStatus(int id, string status)
        {

            if(status != "want" && status != "skip")
                throw new ArgumentOutOfRangeException(nameof(status), "Download status can only be skip or want");

            var episode = _context.TvEpisodes.FirstOrDefault(e => e.Id == id);

            if(episode == null)
                throw new ArgumentOutOfRangeException(nameof(id),"Episode ID was invalid or not in the database");

            episode.DownloadStatus = status.ToUpper();
            _context.TvEpisodes.AddOrUpdate(episode);

            _context.SaveChanges();
           
            return PartialView("_DownloadStatusToggle", episode);

        }

        [HttpPost]
        public ActionResult ShowSeasonEpisodes(int seriesId, int season)
        {
            var episodes = _context.TvEpisodes.Where(e => e.SeriesId == seriesId && e.Season == season).ToList();

            return PartialView("_EpisodeList", episodes);

        }
    }
}