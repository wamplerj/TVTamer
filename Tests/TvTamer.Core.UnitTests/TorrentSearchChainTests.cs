using System.IO;
using System.Reflection;
using System.Xml;
using Moq;
using NUnit.Framework;
using TvTamer.Core.Torrents;

namespace TvTamer.Core.UnitTests
{
    [TestFixture]
    public class TorrentSearchChainTests
    {
        [Test]
        public void NoEpisodeFoundReturnsNull()
        {

            var webRequestor = new Mock<IWebClient>();
            webRequestor.Setup(wr => wr.GetXml(It.IsAny<string>(), null)).Returns(new XmlDocument());

            var analyticService = new Mock<IAnalyticsService>();

            var nullSearchProvider = new NullSearchProvider();
            var katSearchProvider = new KickassSearchProvider(nullSearchProvider, webRequestor.Object, analyticService.Object);
            var tpbSearchProvider = new ThePirateBaySearchProvider(katSearchProvider);

            var result = tpbSearchProvider.GetTorrent("some valid search goes here");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void FirstEpisodeMatchingSearchReturnsTorrent()
        {

            var xml = string.Empty;

            using (
                var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("TvTamer.Core.UnitTests.XmlSamples.KatEpisodeSearchResults.xml"))
            {
                using (var reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
            }

            var document = new XmlDocument();
            document.LoadXml(xml);

            var analyticService = new Mock<IAnalyticsService>();

            var webRequestor = new Mock<IWebClient>();
            webRequestor.Setup(wr => wr.GetXml(It.IsAny<string>(), It.IsAny<string>())).Returns(document);

            var nullSearchProvider = new NullSearchProvider();
            var katSearchProvider = new KickassSearchProvider(nullSearchProvider, webRequestor.Object, analyticService.Object);

            var result = katSearchProvider.GetTorrent("some valid search goes here");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.Not.Null.Or.Empty);
            Assert.That(result.MagnetUrl, Is.Not.Null.Or.Empty);

        }

        [Test]
        public void EpisodeMatchingIgnoreWordIsSkipped()
        {

            var xml = string.Empty;

            using (
                var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("TvTamer.Core.UnitTests.XmlSamples.KatEpisodeIgnoreWordSearchResults.xml"))
            {
                using (var reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
            }

            var document = new XmlDocument();
            document.LoadXml(xml);

            var webRequestor = new Mock<IWebClient>();
            webRequestor.Setup(wr => wr.GetXml(It.IsAny<string>(), It.IsAny<string>())).Returns(document);

            var analyticService = new Mock<IAnalyticsService>();

            var nullSearchProvider = new NullSearchProvider();
            var katSearchProvider = new KickassSearchProvider(nullSearchProvider, webRequestor.Object, analyticService.Object);

            var result = katSearchProvider.GetTorrent("some valid search goes here");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Good Result"));
            Assert.That(result.DownloadUrl, Is.Not.Null.Or.Empty);

        }

    }

}
