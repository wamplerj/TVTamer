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
        private string kickassXmlResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <rss version = ""2.0"" xmlns:torrent=""http://xmlns.ezrss.it/0.1/"">
                <channel>
                    <title>Torrents by keyword ""the big bang theory"" - KickassTorrents</title>
                        <link>http://kickass.to/</link>
                        <description>Torrents by keyword ""the big bang theory""</description>
                        <item>
                            <title>The Big Bang Theory S08E24 HDTV x264-LOL[ettv]</title>

                            <category>TV</category>
                          <author>http://kickass.to/user/ettv/</author>        <link>http://kickass.to/the-big-bang-theory-s08e24-hdtv-x264-lol-ettv-t10616173.html</link>
                            <guid>http://kickass.to/the-big-bang-theory-s08e24-hdtv-x264-lol-ettv-t10616173.html</guid>
                            <pubDate>Fri, 08 May 2015 00:31:01 +0000</pubDate>
                            <torrent:contentLength>122173669</torrent:contentLength>
                            <torrent:infoHash>1A16AFA4F2BCEB2D289F1E075E2BB08980C5D954</torrent:infoHash>
                            <torrent:magnetURI><![CDATA[magnet:?xt=urn:btih:1A16AFA4F2BCEB2D289F1E075E2BB08980C5D954&dn=the+big+bang+theory+s08e24+hdtv+x264+lol+ettv&tr=udp%3A%2F%2Fglotorrents.pw%3A6969%2Fannounce&tr=udp%3A%2F%2Fopen.demonii.com%3A1337]]></torrent:magnetURI>
                            <torrent:seeds>21276</torrent:seeds>
                            <torrent:peers>27208</torrent:peers>
                            <torrent:verified>1</torrent:verified>
                            <torrent:fileName>the.big.bang.theory.s08e24.hdtv.x264.lol.ettv.torrent</torrent:fileName>
                            <enclosure url = ""http://torcache.net/torrent/1A16AFA4F2BCEB2D289F1E075E2BB08980C5D954.torrent?title=[kickass.to]the.big.bang.theory.s08e24.hdtv.x264.lol.ettv"" length=""122173669"" type=""application/x-bittorrent"" />
                        </item>
                        <item>
                            <title>The Big Bang Theory S08E23 HDTV x264-LOL[ettv]</title>

                            <category>TV</category>
                          <author>http://kickass.to/user/ettv/</author>        <link>http://kickass.to/the-big-bang-theory-s08e23-hdtv-x264-lol-ettv-t10584333.html</link>
                            <guid>http://kickass.to/the-big-bang-theory-s08e23-hdtv-x264-lol-ettv-t10584333.html</guid>
                            <pubDate>Fri, 01 May 2015 00:32:49 +0000</pubDate>
                            <torrent:contentLength>133611005</torrent:contentLength>
                            <torrent:infoHash>43F614EEF26DBC23A15A0A55033D40BF052D48E5</torrent:infoHash>
                            <torrent:magnetURI><![CDATA[magnet:?xt=urn:btih:43F614EEF26DBC23A15A0A55033D40BF052D48E5&dn=the+big+bang+theory+s08e23+hdtv+x264+lol+ettv&tr=udp%3A%2F%2F9.rarbg.to%3A2710%2Fannounce&tr=udp%3A%2F%2Fopen.demonii.com%3A1337]]></torrent:magnetURI>
                            <torrent:seeds>9251</torrent:seeds>
                            <torrent:peers>9709</torrent:peers>
                            <torrent:verified>1</torrent:verified>
                            <torrent:fileName>the.big.bang.theory.s08e23.hdtv.x264.lol.ettv.torrent</torrent:fileName>
                            <enclosure url = ""http://torcache.net/torrent/43F614EEF26DBC23A15A0A55033D40BF052D48E5.torrent?title=[kickass.to]the.big.bang.theory.s08e23.hdtv.x264.lol.ettv"" length=""133611005"" type=""application/x-bittorrent"" />
                        </item>
                </channel>
                </rss>";


        [Test]
        public void NoEpisodeFoundReturnsNull()
        {

            var webRequestor = new Mock<IWebRequester>();
            webRequestor.Setup(wr => wr.GetXml(It.IsAny<string>())).Returns(new XmlDocument());
            
            var nullSearchProvider = new NullSearchProvider();
            var katSearchProvider = new KickassSearchProvider(nullSearchProvider, webRequestor.Object);
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

            var webRequestor = new Mock<IWebRequester>();
            webRequestor.Setup(wr => wr.GetXml(It.IsAny<string>())).Returns(document);

            var nullSearchProvider = new NullSearchProvider();
            var katSearchProvider = new KickassSearchProvider(nullSearchProvider, webRequestor.Object);

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

            var webRequestor = new Mock<IWebRequester>();
            webRequestor.Setup(wr => wr.GetXml(It.IsAny<string>())).Returns(document);

            var nullSearchProvider = new NullSearchProvider();
            var katSearchProvider = new KickassSearchProvider(nullSearchProvider, webRequestor.Object);

            var result = katSearchProvider.GetTorrent("some valid search goes here");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Good Result"));
            Assert.That(result.DownloadUrl, Is.Not.Null.Or.Empty);

        }

    }

}
