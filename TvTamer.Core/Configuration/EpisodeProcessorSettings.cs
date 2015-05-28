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

        [ConfigurationProperty("dryRun", IsRequired = false, DefaultValue = false)]
        public bool DryRun
        {
            get { return (bool)this["dryRun"]; }
            set { this["dryRun"] = value; }
        }

        [ConfigurationProperty("fileExtentions", IsRequired = true)]
        public string FileExtentions
        {
            get
            {
                //var csvFileExtentions = (string) this["fileExtentions"];

                //if (string.IsNullOrEmpty(csvFileExtentions)) return Array.Empty<string>();

                //var fileExtentions = csvFileExtentions.Split(',');
                //return fileExtentions;
                return (string)this["fileExtentions"];

            }
            //set { this["fileExtentions"] = string.Join(",", value); }
        }

    }
}
