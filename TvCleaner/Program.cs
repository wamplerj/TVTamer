using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.LogReceiverService;
using TvTamer.Core;

namespace TvCleaner
{
    class Program
    {

        private Logger _logger = LogManager.GetLogger("log");

        static int Main(string[] args)
        {

            //Scan source directory for matching files

            if (args.Length != 2)
            {
                Console.WriteLine("Run with 2 arguments, the paths to source and destination.");
                Console.WriteLine("\tex. tvcleaner.exe c:\\source\\ c:\\destination\\");
                return -1;
            }

            var source = args[0];
            var destination = args[1];

            if (!Directory.Exists(source))
            {
                Console.WriteLine("Source folder: {0} does not exist on disk");
                return -2;
            }

            var processor = new EpisodeProcessor();
            var episodes = processor.GetTvEpisodeFiles(source);

            foreach (var episode in episodes)
            {
                if (processor.DestinationFileExists(episode, destination))
                {
                    processor.DeleteSourceFile(episode.FileName);
                }
            }

            return 0;
        }
    }
}
