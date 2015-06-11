using System.Configuration;

namespace TvTamer.Core.Configuration
{
    public class TorrentSearchSettings : ConfigurationSection
    {

        [ConfigurationProperty("torrentWatchFolder")]
        public string TorrentWatchFolder
        {
            get { return (string)this["torrentWatchFolder"]; }
            set { this["torrentWatchFolder"] = value; }
        }

    }
}
