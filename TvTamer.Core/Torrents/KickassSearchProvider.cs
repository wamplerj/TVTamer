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
        //TODO make ignore words configurable and global across searchproviders
        private readonly string[] _ignoreWords = {"german", "french", "core2hd", "dutch", "swedish", "reenc", "MrLss"};

        public KickassSearchProvider(ISearchProvider nextProvider, IWebRequester webRequester)
        {

            if(nextProvider == null)
                throw new ArgumentNullException(nameof(nextProvider), "nextProvider Cannot be null");

            NextSearchProvider = nextProvider;
            _webRequester = webRequester;
        }

        public Torrent GetTorrent(string search)
        {

            var encodedSearch = HttpUtility.HtmlEncode(search);

            //TODO Get query string from config file
            var url = $"https://kat.cr/usearch/{encodedSearch}/?field=seeders&sorder=desc&rss=1";
            var xml = _webRequester.GetXml(url);

            var nodes = xml?.SelectNodes("rss/channel/item");

            if (nodes == null)
                NextSearchProvider.GetTorrent(search);

            foreach (XmlNode node in nodes)
            {

                var name = node["title"]?.InnerText;

                if (_ignoreWords.Any(name.Contains)) continue;

                var torrent = new Torrent
                {
                    Name = name,
                    PublicationDate = Convert.ToDateTime(node["pubDate"].InnerText),
                    PageUrl = node["guid"].InnerText,
                    MagnetUrl = node["torrent:magnetURI"].InnerText,
                    DownloadUrl = node["enclosure"].Attributes["url"].InnerText
                };

                return torrent;
            }

            return NextSearchProvider.GetTorrent(search);
        }

        public ISearchProvider NextSearchProvider { get; }
    }
}
