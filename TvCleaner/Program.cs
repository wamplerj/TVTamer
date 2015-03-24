using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NLog;
using TvTamer.Core;

namespace TvCleaner
{
    class Program
    {

        private Logger _logger = LogManager.GetLogger("log");
        private static string _sourceFolder;
        private static string _destinationFolder;

        static int Main(string[] args)
        {

            //Scan source directory for matching files

            if (args.Length != 2)
            {
                Console.WriteLine("Run with 2 arguments, the paths to source and destination.");
                Console.WriteLine("\tex. tvcleaner.exe c:\\source\\ c:\\destination\\");
                return -1;
            }

            _sourceFolder = args[0];
            _destinationFolder = args[1];

            if (!Directory.Exists(_sourceFolder))
            {
                Console.WriteLine("Source folder: {0} does not exist on disk");
                return -2;
            }

            var processor = new EpisodeProcessor(_sourceFolder, _destinationFolder);
            var episodes = processor.GetTvEpisodeFiles();

            Console.WriteLine("Found {0} files in {1}", episodes.Count(), _sourceFolder);
            Console.WriteLine("===========================================================================");
            DisplayFiles(episodes.Select(e => e.FileName).ToList());

            var deletedFiles = new List<string>();
            foreach (var episode in episodes)
            {
                if (!processor.DestinationFileExists(episode)) continue;

                processor.DeleteSourceFile(episode.FileName);
                deletedFiles.Add(episode.FileName);
            }

            Console.WriteLine("\r\n\r\nDeleted {0} files from {1}", deletedFiles.Count(), _sourceFolder);
            Console.WriteLine("===========================================================================");
            DisplayFiles(deletedFiles);

            return 0;
        }

        private static void DisplayFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var fileName = file.Substring(_sourceFolder.Length);
                if (fileName.Length > 70)
                {
                    var parts = fileName.Split('\\');
                    fileName = parts[0];
                }
                Console.WriteLine(fileName);
            }
        }
    }
}
