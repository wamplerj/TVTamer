using System;

namespace TvTamer.Core.Torrents
{
    public class Torrent
    {
        
        public string PageUrl { get; set; }
        public DateTime PublicationDate { get; set; }
        public string DownloadUrl { get; set; }
        public string MagnetUrl { get; set; }
        public string Name { get; set; }

    }
}