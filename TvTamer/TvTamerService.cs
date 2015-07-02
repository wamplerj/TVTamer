using System;
using System.Threading;
using NLog;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.Persistance;

namespace TvTamer
{
    public class TvTamerService
    {
        private readonly IDatabaseUpdater _databaseUpdater;
        private readonly IEpisodeProcessor _episodeProcessor;
        private readonly IEpisodeDownloader _episodeDownloader;
        private readonly ScheduleSettings _scheduleSettings;

        private readonly Logger _logger = LogManager.GetLogger("log");

        private Timer _databaseUpdateTimer;
        private Timer _downloadFolderCleanupTimer;
        private Timer _episodeSearchTimer;

        public TvTamerService(IDatabaseUpdater databaseUpdater, IEpisodeProcessor episodeProcessor, IEpisodeDownloader episodeDownloader, ScheduleSettings scheduleSettings)
        {
            _databaseUpdater = databaseUpdater;
            _episodeProcessor = episodeProcessor;
            _episodeDownloader = episodeDownloader;
            _scheduleSettings = scheduleSettings;

        }

        private void StartBackgroundTasks()
        {

            _logger.Info($"DatabaseUpdateFrequency set to {_scheduleSettings.DatabaseUpdateFrequency} minutes");
            _logger.Info($"ProcessDownloadedFilesFrequency set to {_scheduleSettings.ProcessDownloadedFilesFrequency} minutes");
            _logger.Info($"NewEpisodeSearchFrequency set to {_scheduleSettings.NewEpisodeSearchFrequency} minutes");

            _databaseUpdateTimer = new Timer(_ =>
            {
                _logger.Info("Database Update Starting: {0}", DateTime.Now);
                _databaseUpdater.Update();
            }, null, 100000, 60000 * _scheduleSettings.DatabaseUpdateFrequency);

            _downloadFolderCleanupTimer = new Timer(_ =>
            {
                _logger.Info("Download Cleanup Starting: {0}", DateTime.Now);
                _episodeProcessor.ProcessDownloadedEpisodes();
            }, null, 600000, 60000 * _scheduleSettings.ProcessDownloadedFilesFrequency);

            _episodeSearchTimer = new Timer(_ => 
            {
                _logger.Info("Searching for Episodes to Download: {0}", DateTime.Now);
                _episodeDownloader.DownloadWantedEpisodes();
            }, null, 1000, 60000 * _scheduleSettings.NewEpisodeSearchFrequency);
        }

        private void StopBackgroundTasks()
        {
            _downloadFolderCleanupTimer.Dispose();
            _databaseUpdateTimer.Dispose();
            _episodeSearchTimer.Dispose();
        }

        public void Start() { StartBackgroundTasks(); }

        public void Stop() { StopBackgroundTasks(); }

    }
}
