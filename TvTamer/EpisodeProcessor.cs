using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.FileSystem;
using TvTamer.Core.Models;
using File = TvTamer.Core.FileSystem.File;

namespace TvTamer
{
    public interface IEpisodeProcessor
    {
        void ProcessDownloadedEpisodes();
        IEnumerable<TvEpisode> GetTvEpisodeFiles();
        bool DestinationFileExists(TvEpisode episode);
        void DeleteSourceFile(string filePath);
    }

    public class EpisodeProcessor : IEpisodeProcessor
    {
        private readonly EpisodeProcessorSettings _settings;
        private readonly ITvService _tvService;
        private readonly IFileSystem _fileSystem;
        private readonly IAnalyticsService _analyticsService;

        private readonly IDirectory _sourceFolder;
        private readonly IDirectory _destinationFolder;

        private readonly Logger _logger = LogManager.GetLogger("log");

        public EpisodeProcessor(EpisodeProcessorSettings settings, ITvService tvService, IFileSystem fileSystem, IAnalyticsService analyticsService)
        {
            _settings = settings;
            _tvService = tvService;
            _fileSystem = fileSystem;
            _analyticsService = analyticsService;

            _sourceFolder = _fileSystem.GetDirectory(_settings.DownloadFolder);
            _destinationFolder = _fileSystem.GetDirectory(_settings.TvLibraryFolder);

        }

        public void ProcessDownloadedEpisodes()
        {
            if (_settings.DryRun) _logger.Info("DryRun = true");

            var episodes = GetTvEpisodeFiles().ToList();

            if (!episodes.Any())
            {
                _logger.Info("No Downloaded episodes found to process");
                return;
            }

            LogFileInformation(episodes.Select(e => e.FileName).ToList(), "Found {0} files in {1}");

            foreach (var episode in episodes)
            {
                if (!DestinationFileExists(episode))
                {
                    CopyEpisodeToDestination(episode);
                }

                DeleteSourceFile(episode.FileName);
            }
        }

        private void LogFileInformation(List<string> files, string message)
        {
            _logger.Info("\r\n\r\nDeleted {0} files from {1}", files.Count(), _sourceFolder.Path);
            _logger.Info("===========================================================================");

            foreach (var file in files)
            {
                var fileName = file.Substring(_sourceFolder.Path.Length);
                if (fileName.Length > 70)
                {
                    var parts = fileName.Split('\\');
                    fileName = parts.Last();
                }
                _logger.Info(fileName);
            }
        }

        public IEnumerable<TvEpisode> GetTvEpisodeFiles()
        {
            var files = _sourceFolder.EnumerateFiles("*", true)
                .Where(f => _settings.FileExtentions.Contains(new File(f).Extension.ToLower()) && !f.Contains("sample"));
            return TvEpisodeFilter.GetEpisodes(files);
        }

        private void CopyEpisodeToDestination(TvEpisode episode)
        {

            var sourceFile = _fileSystem.GetFile(episode.FileName);

            var foundEpisode = _tvService.GetEpisodeBySeriesName(episode.SeriesName, episode.Season,
                episode.EpisodeNumber, true);

            if (foundEpisode == null)
            {
                _logger.Info($"Could not find Season: {episode.Season} Episode: {episode.EpisodeNumber} of Series: {episode.SeriesName} in database");
                return;
            }

            //TODO Refactor episode file naming somewhere centralized.
            var destinationFilename = $"{_destinationFolder.Path}\\{episode.SeriesName}\\Season {episode.Season:D2}\\S{episode.Season:D2}E{episode.EpisodeNumber:D2} - {foundEpisode.Title}{sourceFile.Extension}";
            try
            {
                sourceFile.Copy(destinationFilename);
            }
            catch (Exception ex)
            {
                _analyticsService.ReportEvent(AnalyticEvent.EpisodeFailed, $"Unable to copy {sourceFile} to {destinationFilename}");
            }
            _logger.Info($"Copying {sourceFile} to {destinationFilename}");
            _analyticsService.ReportEvent(AnalyticEvent.Episode, $"Copying {sourceFile} to {destinationFilename}");

            foundEpisode.FileName = destinationFilename;
            foundEpisode.DownloadStatus = "HAVE";

            _tvService.AddOrUpdate(foundEpisode);
            _tvService.SaveChanges();

        }

        public bool DestinationFileExists(TvEpisode episode)
        {
            var seriesDestinationFolderPath = $"{_destinationFolder.Path}\\{episode.SeriesName}\\Season {episode.Season:D2}\\";
            var seriesDestinationFolder = _fileSystem.GetDirectory(seriesDestinationFolderPath);

            if (!seriesDestinationFolder.Exists()) return false;

            var episodeSearchPattern = $"S{episode.Season:D2}E{episode.EpisodeNumber:D2}*";
            var files = seriesDestinationFolder.EnumerateFiles(episodeSearchPattern, false);

            return files.Any();
        }

        public void DeleteSourceFile(string filePath)
        {
            var file = _fileSystem.GetFile(filePath);
            if (file.DirectoryName == _sourceFolder.Path)
            {
                if (!_settings.DryRun)
                    file.Delete();

                _logger.Info("Deleted file from source at: {0}\r\n", filePath);
            }
            else
            {
                var folderParts = filePath.Substring(_sourceFolder.Path.Length + 1).Split('\\');
                var rootFolder = _fileSystem.GetDirectory(_sourceFolder.Path + "\\" + folderParts[0]);

                if (!rootFolder.Exists()) return;

                if(!_settings.DryRun)
                    rootFolder.Delete(true);

                _logger.Info("Deleted folder from source at: {0}\r\n", rootFolder.Path);
            }
        }
    }
}
