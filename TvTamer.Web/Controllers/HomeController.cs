using System.Linq;
using System.Web.Mvc;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;
using TvTamer.Web.Models;

namespace TvTamer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITvContext _context;


        public HomeController(ITvContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {

            var query = @"SELECT 
                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], e.[DownloadStatus] AS [DownloadStatus], 
                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
                WHERE (DATEDIFF(day, SysDateTime(), e.[FirstAired])) = 0 AND N'WANT' = e.[DownloadStatus] ORDER BY e.[FirstAired] ASC";

            var todaysEpisodes = _context.QuerySql<TvEpisode>(query).ToList();
            var model = new HomeViewModel() {TodaysEpisodes = todaysEpisodes};

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