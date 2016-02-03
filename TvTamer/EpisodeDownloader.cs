using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.Models;
using TvTamer.Core.Torrents;

namespace TvTamer
{

    public interface IEpisodeDownloader
    {
        Task DownloadWantedEpisodes();
        string BuildSearchQuery(TvEpisode episode);
    }

    public class EpisodeDownloader : IEpisodeDownloader
    {
        private readonly ITvService _service;
        private readonly Logger _logger = LogManager.GetLogger("log");
        private readonly ISearchProvider _searchProvider;
        private readonly TorrentSearchSettings _settings;
        private readonly IAnalyticsService _analyticsService;
        private readonly IWebClient _webClient;

        public EpisodeDownloader(ITvService service, ISearchProvider searchProvider, IWebClient webClient, TorrentSearchSettings settings, IAnalyticsService analyticsService)
        {
            _service = service;
            _searchProvider = searchProvider;
            _webClient = webClient;
            _settings = settings;
            _analyticsService = analyticsService;
        }

        public async Task DownloadWantedEpisodes()
        {

            var episodesToDownload = _service.GetWantedEpisodes();

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

                try
                {
                    var torrentFileName = $"{_settings.TorrentWatchFolder}{torrent.Name}.torrent";
                    await _webClient.DownloadFileAsync(torrent.DownloadUrl, torrentFileName);

                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                episode.DownloadStatus = "DOWN";
                _service.AddOrUpdate(episode);
                _logger.Debug($"Updating {episode.SeriesName} s{episode.Season:D2}e{episode.EpisodeNumber:D2} as Downloaded");

            }

            _service.SaveChanges();
        }

        public string BuildSearchQuery(TvEpisode episode)
        {

            var query = $"{episode.SeriesName} s{episode.Season:D2}e{episode.EpisodeNumber:D2} 720".ToLower();
            _logger.Debug($"Search Query before stripping characters {query}");

            query = new Regex("[^a-zA-Z0-9 -]").Replace(query, "");
            _logger.Debug($"Search Query after stripping characters {query}");

            return query;
        }
    }
}
