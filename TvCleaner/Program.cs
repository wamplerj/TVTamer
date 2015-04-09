using System;
using System.IO;
using Autofac;
using CommandLine;
using NLog;
using TvTamer.Core;

namespace TvCleaner
{
    class Program
    {

        private static Logger _logger = LogManager.GetLogger("log");
        private static Arguments _arguments = new Arguments();

        static int Main(string[] args)
        {
            LoadArguments(args);

            var processor = new EpisodeProcessor(_arguments.SourceFolder, _arguments.DestinationFolder);
            processor.ProcessDownloadedEpisodes();

            return 0;
        }

        private static void LoadArguments(string[] args)
        {

            Parser.Default.ParseArguments(args, _arguments);

            if (!Directory.Exists(_arguments.SourceFolder))
            {
                _logger.Info("Source folder: {0} does not exist on disk", _arguments.SourceFolder);
                Environment.Exit(-2);
            }

            if (!Directory.Exists(_arguments.DestinationFolder))
            {
                _logger.Info("Destination folder: {0} does not exist on disk", _arguments.DestinationFolder);
                Environment.Exit(-2);
            }


        }
    }
}
