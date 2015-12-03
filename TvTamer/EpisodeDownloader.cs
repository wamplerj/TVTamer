using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;
using TvTamer.Core.Torrents;

namespace TvTamer
{

    public interface IEpisodeDownloader
    {
        void DownloadWantedEpisodes();
        string BuildSearchQuery(TvEpisode episode);
    }

    public class EpisodeDownloader : IEpisodeDownloader
    {
        private readonly ITvContext _context;
        private readonly Logger _logger = LogManager.GetLogger("log");
        private readonly ISearchProvider _searchProvider;
        private readonly TorrentSearchSettings _settings;
        private readonly IAnalyticsService _analyticsService;
        private readonly IWebRequester _webRequester;

        public EpisodeDownloader(ITvContext context, ISearchProvider searchProvider, IWebRequester webRequester, TorrentSearchSettings settings, IAnalyticsService analyticsService)
        {
            _context = context;
            _searchProvider = searchProvider;
            _webRequester = webRequester;
            _settings = settings;
            _analyticsService = analyticsService;
        }

        public void DownloadWantedEpisodes()
        {
            var query = @"SELECT 
                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], e.[DownloadStatus] AS [DownloadStatus], 
                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
                WHERE (DATEDIFF(day, e.[FirstAired], SysDateTime())) >= 0 AND N'WANT' = e.[DownloadStatus] ORDER BY e.[FirstAired] ASC";

            var episodesToDownload = _context.QuerySql<TvEpisode>(query).ToList();

            foreach (var episode in episodesToDownload)
            {

                var search = BuildSearchQuery(episode);
                var torrent = _searchProvider.GetTorrent(search);

                if(torrent == null)
                {
                    _logger.Info($"No torrent found for {search}");
                    _analyticsService.ReportEvent(AnalyticEvent.SearchFailed);
                    continue;
                }

                var torrentWatchFolder = _settings.TorrentWatchFolder;
                _webRequester.DownloadFileAsync(torrent.DownloadUrl, torrentWatchFolder + torrent.Name + ".torrent");
            }

        }

        public string BuildSearchQuery(TvEpisode episode)
        {
            var query = $"{episode.SeriesName} s{episode.Season:D2}e{episode.EpisodeNumber:D2} 720".ToLower();
            query = new Regex("[^a-zA-Z0-9 -]").Replace(query, "");

            return query;
        }
    }
}
