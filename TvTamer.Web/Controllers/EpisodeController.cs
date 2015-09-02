using System;
using System.Collections.Generic;
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
    }
}