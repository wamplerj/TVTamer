using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;
using TvTamer.Core.Torrents;

namespace TvTamer.UnitTests
{
    [TestFixture]
    public class EpisodeDownloaderTests
    {

        [Test]
        public void DownloadWantedEpisodes()
        {

            var context = new Mock<ITvContext>();
            context.Setup(c => c.QuerySql<TvEpisode>(It.IsAny<string>())).Returns(new List<TvEpisode> { new TvEpisode() { SeriesName = "SomeSeries", Season = 1, EpisodeNumber = 1 } });

            var torrent = new Torrent()
            {
                DownloadUrl = "DownloadUrl",
                Name = "Torrent"
            };

            var searchProvider = new Mock<ISearchProvider>();
            searchProvider.Setup(sp => sp.GetTorrent(It.IsAny<string>())).Returns(torrent);

            var webRequester = new Mock<IWebRequester>();
            var analyticsService = new Mock<IAnalyticsService>();

            var downloader = new EpisodeDownloader(context.Object, searchProvider.Object, webRequester.Object, new TorrentSearchSettings() { TorrentWatchFolder = "WatchFolder\\" }, analyticsService.Object);
            downloader.DownloadWantedEpisodes();

            webRequester.Verify(wr => wr.DownloadFileAsync(torrent.DownloadUrl, "WatchFolder\\Torrent.torrent", null), Times.Once);
        }


    }
}
