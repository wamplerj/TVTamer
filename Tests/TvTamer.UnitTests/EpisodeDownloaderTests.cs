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

            var service = new Mock<ITvService>();
            service.Setup(c => c.GetWantedEpisodes()).Returns(new List<TvEpisode> { new TvEpisode() { SeriesName = "SomeSeries", Season = 1, EpisodeNumber = 1 } });

            var torrent = new Torrent()
            {
                DownloadUrl = "DownloadUrl",
                Name = "Torrent"
            };

            var searchProvider = new Mock<ISearchProvider>();
            searchProvider.Setup(sp => sp.GetTorrent(It.IsAny<string>())).Returns(torrent);

            var webRequester = new Mock<IWebClient>();
            var analyticsService = new Mock<IAnalyticsService>();

            var downloader = new EpisodeDownloader(service.Object, searchProvider.Object, webRequester.Object, new TorrentSearchSettings() { TorrentWatchFolder = "WatchFolder\\" }, analyticsService.Object);
            downloader.DownloadWantedEpisodes().Wait();

            webRequester.Verify(wr => wr.DownloadFileAsync(torrent.DownloadUrl, "WatchFolder\\Torrent.torrent", null), Times.Once);
        }


        [Test]
        public void BuildEpisodeSearchQuery()
        {

            var service = new Mock<ITvService>();
            var searchProvider = new Mock<ISearchProvider>();
            var webRequester = new Mock<IWebClient>();
            var analyticsService = new Mock<IAnalyticsService>();

            var downloader = new EpisodeDownloader(service.Object, searchProvider.Object, webRequester.Object, new TorrentSearchSettings() { TorrentWatchFolder = "WatchFolder\\" }, analyticsService.Object);
            var query = downloader.BuildSearchQuery(new TvEpisode() { SeriesName = "Some Series (2015)", Season = 1, EpisodeNumber = 1 } );

            Assert.That(query, Is.EqualTo("some series 2015 s01e01 720"));
         
        }

    }
}
