using System;
using System.Data.Entity;
using System.IO;
using CommandLine;
using NLog;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.Persistance;

namespace TvCleaner
{
    class Program
    {

        private static Logger _logger = LogManager.GetLogger("log");
        private static Arguments _arguments = new Arguments();

        static int Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TvContext>());
            LoadArguments(args);

            UpdateTvDatabase();

            var settings = new EpisodeProcessorSettings() { DownloadFolder = _arguments.SourceFolder, TvLibraryFolder = _arguments.DestinationFolder};

            //var processor = new EpisodeProcessor(settings, new TvContext());
            //processor.ProcessDownloadedEpisodes();

            return 0;
        }

        private static void UpdateTvDatabase()
        {
            var updater = new DatabaseUpdater(new TvDbSearchService(), new TvContext());
            updater.Update();

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
