using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using NLog;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

namespace TvTamer.Core
{

    public interface ITvService
    {
        int SaveChanges();

        void AddOrUpdate(TvSeries series);
        void AddOrUpdate(TvEpisode episode);

        TvSeries GetTvSeriesById(int seriesId);
        List<TvSeries> GetTvSeries(int page = 0, int pageSize = 500);

        List<int> GetSeasons(int seriesId);
       

        TvEpisode GetEpisodeBySeriesName(string seriesName, int season, int episodeNumber, bool searchByAlternateName = false);
        List<TvEpisode> GetEpisodesBySeason(int id, int lastSeason);
        List<TvEpisode> GetEpisodesByDate(DateTime today);
        List<TvEpisode> GetRecentlyDownloadedEpisodes();
        List<TvEpisode> GetWantedEpisodes();
    }

    public class TvService : ITvService
    {
        private readonly ITvContext _context;
        private readonly Logger _logger = LogManager.GetLogger("log");

        public TvService(ITvContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
           return _context.SaveChanges();
        }

        public void AddOrUpdate(TvSeries series)
        {
            _context.TvSeries.Add(series);
        }

        public void AddOrUpdate(TvEpisode episode)
        {
            _context.TvEpisodes.AddOrUpdate(e => e.Id, episode);
        }

        public TvSeries GetTvSeriesById(int seriesId)
        {
            return _context.TvSeries
                .Include(s => s.AlternateNames)
                .FirstOrDefault(s => s.Id == seriesId);
        }

        public List<TvSeries> GetTvSeries(int page=0, int pageSize = 500)
        {
            return _context.TvSeries.OrderBy(s => s.Name).Take(pageSize).Skip(pageSize * page).ToList();
        }

        public List<int> GetSeasons(int seriesId)
        {
            return _context.QuerySql<int>($"select Distinct(Season) from TvEpisodes where seriesId = {seriesId} and Season > 0").ToList();
        }

        public TvEpisode GetEpisodeBySeriesName(string seriesName, int season, int episodeNumber, bool searchByAlternateName = false)
        {

            var query =
                $@"SELECT TOP 1 s.Id, s.TvDbSeriesId, s.Name, s.FirstAired, s.AirsDayOfWeek, s.AirsTimeOfDay, s.Network, s.Summary, s.Status, s.Rating, s.LastUpdated FROM [dbo].[AlternateNames] an
                   RIGHT JOIN TVSeries s ON s.Id = an.SeriesId WHERE s.Name = @seriesName";

            if(searchByAlternateName)
                query += $" OR an.Name = @seriesName";

            _logger.Debug($"GetEpisodeBySeriesName on TVContext, SQL Query: {query}");

            var seriesNameParameter = new SqlParameter("@seriesName", seriesName);
            var tvSeries =  _context.Set<TvSeries>().SqlQuery(query, seriesNameParameter).AsNoTracking().FirstOrDefault(); //DBSet<T>.SqlQuery is mockable

            if (tvSeries == null)
            {
                _logger.Info($"GetEpisodeBySeriesName on TVContext, TV Series: {seriesName} could not be found in the database.  searchByAlternateName={searchByAlternateName}");
                return null;
            }

            return
                _context.TvEpisodes.FirstOrDefault(
                    e => e.SeriesId == tvSeries.Id && e.Season == season && e.EpisodeNumber == episodeNumber);

        }

        public List<TvEpisode> GetEpisodesBySeason(int seriesId, int lastSeason)
        {
            return _context.TvEpisodes.Where(e => e.SeriesId == seriesId && e.Season == lastSeason).OrderBy(e => e.Season).ThenBy(e => e.EpisodeNumber).ToList();
        }

        public List<TvEpisode> GetEpisodesByDate(DateTime airDate)
        {
            var query = @"SELECT 
                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], e.[DownloadStatus] AS [DownloadStatus], 
                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
                WHERE (DATEDIFF(day, SysDateTime(), e.[FirstAired])) = 0 AND N'WANT' = e.[DownloadStatus] ORDER BY e.[FirstAired] ASC";

            var airDateParameter = new SqlParameter("@airDate", airDate);
            var todaysEpisodes = _context.QuerySql<TvEpisode>(query, airDateParameter).ToList();
            return todaysEpisodes;

        }

        public List<TvEpisode> GetRecentlyDownloadedEpisodes()
        {
            var query = @"SELECT TOP 6
                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], e.[DownloadStatus] AS [DownloadStatus], 
                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
                WHERE N'HAVE' = e.[DownloadStatus] ORDER BY e.[FirstAired] DESC";

            var recentlyDownloadedEpisodes = _context.QuerySql<TvEpisode>(query).ToList();
            return recentlyDownloadedEpisodes;
        }

        public List<TvEpisode> GetWantedEpisodes()
        {

            var query = @"SELECT 
                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], e.[DownloadStatus] AS [DownloadStatus], 
                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
                WHERE (DATEDIFF(day, e.[FirstAired], SysDateTime())) >= 0 AND N'WANT' = e.[DownloadStatus] ORDER BY e.[FirstAired] ASC";

            var episodesToDownload = _context.QuerySql<TvEpisode>(query).ToList();
            return episodesToDownload;
        }
    }
}
