using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace TvTamer.Core.Torrents
{
    public class KickassSearchProvider : ISearchProvider
    {
        private readonly IWebRequester _webRequester;
        private readonly IAnalyticsService _analyticsService;
        //TODO make ignore words configurable and global across searchproviders
        private readonly string[] _ignoreWords = {"german", "french", "core2hd", "dutch", "swedish", "reenc", "MrLss"};

        public KickassSearchProvider(ISearchProvider nextProvider, IWebRequester webRequester, IAnalyticsService analyticsService)
        {

            if(nextProvider == null)
                throw new ArgumentNullException(nameof(nextProvider), "nextProvider Cannot be null");

            NextSearchProvider = nextProvider;
            _webRequester = webRequester;
            _analyticsService = analyticsService;
        }

        public Torrent GetTorrent(string search)
        {

            _analyticsService.ReportEvent(AnalyticEvent.Search, search);
            var encodedSearch = WebUtility.UrlEncode(search);

            //TODO Get query string from config file
            var url = $"https://kat.cr/usearch/{encodedSearch}/?field=seeders&sorder=desc&rss=1";
            var xml = _webRequester.GetXml(url, "http://torcache.net");

            var nodes = xml?.SelectNodes("rss/channel/item");

            if (nodes == null || nodes.Count == 0)
                return NextSearchProvider.GetTorrent(search);

            foreach (XmlNode node in nodes)
            {
                var torrent = BuildTorrent(node);

                if(torrent == null) continue;
                return torrent;
            }

            return NextSearchProvider.GetTorrent(search);
        }

        private Torrent BuildTorrent(XmlNode node)
        {
            var name = node["title"]?.InnerText;

            if (_ignoreWords.Any(name.Contains)) return null;

            var downloadUrl = node["enclosure"].Attributes["url"].InnerText;

            var torrent = new Torrent
            {
                Name = name,
                PublicationDate = Convert.ToDateTime(node["pubDate"].InnerText),
                PageUrl = node["guid"].InnerText,
                MagnetUrl = node["torrent:magnetURI"].InnerText,
                DownloadUrl = downloadUrl
            };

            return torrent;
        }

        public ISearchProvider NextSearchProvider { get; }
    }
}
