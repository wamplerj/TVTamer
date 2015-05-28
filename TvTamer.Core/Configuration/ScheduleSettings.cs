using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvTamer.Core.Configuration
{

    public class ScheduleSettings : ConfigurationSection
    {

        [ConfigurationProperty("newEpisodeSearchFrequency")]
        public int NewEpisodeSearchFrequency
        {
            get { return (int)this["newEpisodeSearchFrequency"]; }
            set { this["newEpisodeSearchFrequency"] = value; }
        }

        [ConfigurationProperty("databaseUpdateFrequency")]
        public int DatabaseUpdateFrequency
        {
            get { return (int)this["databaseUpdateFrequency"]; }
            set { this["databaseUpdateFrequency"] = value; }
        }

        [ConfigurationProperty("processDownloadedFilesFrequency")]
        public int ProcessDownloadedFilesFrequency
        {
            get { return (int)this["processDownloadedFilesFrequency"]; }
            set { this["processDownloadedFilesFrequency"] = value; }
        }

    }
}
