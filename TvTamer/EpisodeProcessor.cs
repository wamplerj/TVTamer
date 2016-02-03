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
        EpisodeProcessResult GetTvEpisodesFiles();
        bool DestinationFileExists(TvEpisode episode);
        void DeleteSourceFile(string filePath);
    }

    public class EpisodeProcessResult
    {
        public List<TvEpisode> MatchedEpisodes { get; set; } = new List<TvEpisode>();
        public List<TvEpisodeFile> UnmatchedEpisodes { get; set; } = new List<TvEpisodeFile>();
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

            //Get the List of Files in the Download folder that match and don't match 
            var episodesToProcess = GetTvEpisodesFiles();

            ProcessMatchedEpisodes(episodesToProcess.MatchedEpisodes);
            ProcessUnmatchedEpisodes(episodesToProcess.UnmatchedEpisodes);
        }

        public EpisodeProcessResult GetTvEpisodesFiles()
        {
            var result = new EpisodeProcessResult();

            var files = _sourceFolder.EnumerateFiles("*", true)
                .Where(f => _settings.FileExtentions.Contains(new File(f).Extension.ToLower()) && !f.Contains("sample"));

            var episodeFiles = TvEpisodeFileMatcher.GetTvEpisodesFiles(files);
            if (!episodeFiles.Any())
            {
                _logger.Info("No Downloaded episodes found to process");
                return result;
            }

            foreach (var file in episodeFiles)
            {
                var episode = _tvService.GetEpisodeBySeriesName(file.SeriesName, file.Season, file.EpisodeNumber, true);

                if (episode != null)
                {
                    episode.FileName = file.FileName;
                    result.MatchedEpisodes.Add(episode);
                    continue;
                }

                _logger.Info($"Could not find Season: {file.Season} Episode: {file.EpisodeNumber} of Series: {file.SeriesName} in database");
                result.UnmatchedEpisodes.Add(file);
            }

            return result;
        }

        public void ProcessMatchedEpisodes(List<TvEpisode> matchedEpisodes)
        {

            foreach (var episode in matchedEpisodes)
            {
                var cleanEpisodeTitle = CleanFileName(episode.Title);
                var sourceFile = _fileSystem.GetFile(episode.FileName);

                //TODO Refactor episode file naming somewhere centralized.
                var destinationFilename = $"{_destinationFolder.Path}\\{episode.SeriesName}\\Season {episode.Season:D2}\\S{episode.Season:D2}E{episode.EpisodeNumber:D2} - {cleanEpisodeTitle}{sourceFile.Extension}";

                if (!DestinationFileExists(episode))
                {
                    _logger.Info($"Skipped copying File {sourceFile} to {destinationFilename}.  Destination file already exists");
                    DeleteSourceFile(episode.FileName);
                    continue;
                }

                try
                {
                    sourceFile.Copy(destinationFilename);

                    _logger.Info($"Copying {sourceFile} to {destinationFilename}");
                    _analyticsService.ReportEvent(AnalyticEvent.Episode, $"Copying {sourceFile} to {destinationFilename}");
                }
                catch (Exception ex)
                {
                    _analyticsService.ReportEvent(AnalyticEvent.EpisodeFailed, $"Unable to copy {sourceFile} to {destinationFilename}");
                    _logger.Error(ex);
                }

                episode.FileName = destinationFilename;
                episode.DownloadStatus = "HAVE";

                _tvService.AddOrUpdate(episode);
            }

            _tvService.SaveChanges();
        }

        public void ProcessUnmatchedEpisodes(List<TvEpisodeFile> ummatchedEpisodes)
        {
            if (!_settings.DeleteUnmatchedEpisodes) return;

            foreach (var file in ummatchedEpisodes)
            {
                DeleteSourceFile(file.FileName);
            }
        }


        private static string CleanFileName(string fileName)
        {
            //TODO.  Refactor Multiple replace calls into somethign that doesn't suck
            var cleanEpisodeTitle = fileName.Replace("\\", "");
            cleanEpisodeTitle = cleanEpisodeTitle.Replace("/", "");
            cleanEpisodeTitle = cleanEpisodeTitle.Replace(":", "-");
            cleanEpisodeTitle = cleanEpisodeTitle.Replace("*", "");
            cleanEpisodeTitle = cleanEpisodeTitle.Replace("?", "");
            cleanEpisodeTitle = cleanEpisodeTitle.Replace("<", "");
            cleanEpisodeTitle = cleanEpisodeTitle.Replace(">", "");
            cleanEpisodeTitle = cleanEpisodeTitle.Replace("|", "-");
            return cleanEpisodeTitle;
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
                if (!_settings.DeleteUnmatchedEpisodes) return;

                file.Delete();
                _logger.Info("Deleted file from source at: {0}\r\n", filePath);
            }
            else
            {
                var folderParts = filePath.Substring(_sourceFolder.Path.Length + 1).Split('\\');
                var rootFolder = _fileSystem.GetDirectory(_sourceFolder.Path + "\\" + folderParts[0]);

                if (!rootFolder.Exists()) return;

                //TODO Prevent deleting folder if multiple processable items are contained within it
                //https://trello.com/c/gSD5ido6

                if (!_settings.DeleteUnmatchedEpisodes) return;

                rootFolder.Delete(true);
                _logger.Info("Deleted folder from source at: {0}\r\n", rootFolder.Path);
            }
        }
    }
}
