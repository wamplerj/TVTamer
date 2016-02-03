using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvTamer.Core.Configuration
{

    public class EpisodeProcessorSettings : ConfigurationSection
    {

        [ConfigurationProperty("downloadFolder", IsRequired = true),]
        public string DownloadFolder
        {
            get { return (string)this["downloadFolder"]; }
            set { this["downloadFolder"] = value; }
        }

        [ConfigurationProperty("tvLibraryFolder", IsRequired = true)]
        public string TvLibraryFolder
        {
            get { return (string)this["tvLibraryFolder"]; }
            set { this["tvLibraryFolder"] = value; }
        }

        [ConfigurationProperty("fileExtentions", IsRequired = true)]
        public string FileExtentions
        {
            get
            {
                return (string)this["fileExtentions"];
            }
            set { this["fileExtentions"] = value; }
        }

        [ConfigurationProperty("deleteUnmatchedEpisodes", DefaultValue = false)]
        public bool DeleteUnmatchedEpisodes
        {
            get { return (bool)this["deleteUnmatchedEpisodes"]; }
            set { this["deleteUnmatchedEpisodes"] = value; }
        }

    }
}
