using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using NLog;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.FileSystem;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;
using Directory = TvTamer.Core.FileSystem.Directory;
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
        private readonly ITvContext _context;
        private readonly IFileSystemFactory _fileSystemFactory;

        private readonly IDirectory _sourceFolder;
        private readonly IDirectory _destinationFolder;

        //TODO Move alternatenames to the database
        private readonly Dictionary<string, string> _alternateSeriesNames = new Dictionary<string, string>()
        {
            {"Marvels Agents of S H I E L D","Marvels Agents of SHIELD"},
        };

        private readonly Logger _logger = LogManager.GetLogger("log");

        public EpisodeProcessor(EpisodeProcessorSettings settings, ITvContext context, IFileSystemFactory fileSystemFactory)
        {
            _settings = settings;
            _fileSystemFactory = fileSystemFactory;
            _context = context;

            _sourceFolder = _fileSystemFactory.GetDirectory(_settings.DownloadFolder);
            _destinationFolder = _fileSystemFactory.GetDirectory(_settings.TvLibraryFolder);

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

            //var deletedFiles = new List<string>();
            foreach (var episode in episodes)
            {
                if (!DestinationFileExists(episode))
                {
                    CopyEpisodeToDestination(episode);
                }

                DeleteSourceFile(episode.FileName);
                //deletedFiles.Add(episode.FileName);
            }

            //LogFileInformation(deletedFiles, "\r\n\r\nDeleted {0} files from {1}");

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

            var sourceFile = _fileSystemFactory.GetFile(episode.FileName);

            var tvSeries =
                _context.TvSeries.Include(e => e.Episodes).FirstOrDefault(s => s.Name.ToLower() == episode.SeriesName.ToLower());

            var loadedEpisode =
                tvSeries?.Episodes.Find(
                    e => e.Season == episode.Season && e.EpisodeNumber == episode.EpisodeNumber);

            if (loadedEpisode == null)
            {
                _logger.Info($"Could not find Season: {episode.Season} Episode: {episode.EpisodeNumber} of Series: {episode.SeriesName} in database");
                return;
            }

            var destinationFilename = $"{_destinationFolder.Path}\\{episode.SeriesName}\\Season {episode.Season:D2}\\S{episode.Season:D2}E{episode.EpisodeNumber:D2} - {loadedEpisode.Title}{sourceFile.Extension}";

            sourceFile.Copy(destinationFilename);
            _logger.Info($"Copying {sourceFile} to {destinationFilename}");

            loadedEpisode.FileName = destinationFilename;
            loadedEpisode.DownloadStatus = "HAVE";

            _context.TvEpisodes.AddOrUpdate(e => e.Id, loadedEpisode);
            _context.SaveChanges();

        }

        public bool DestinationFileExists(TvEpisode episode)
        {

            if (_alternateSeriesNames.ContainsKey(episode.SeriesName))
                episode.SeriesName = _alternateSeriesNames[episode.SeriesName];

            var seriesDestinationFolderPath = $"{_destinationFolder.Path}\\{episode.SeriesName}\\Season {episode.Season:D2}\\";
            var seriesDestinationFolder = _fileSystemFactory.GetDirectory(seriesDestinationFolderPath);

            if (!seriesDestinationFolder.Exists()) return false;

            var episodeSearchPattern = $"S{episode.Season:D2}E{episode.EpisodeNumber:D2}*";
            var files = seriesDestinationFolder.EnumerateFiles(episodeSearchPattern, false);

            return files.Any();

        }

        public void DeleteSourceFile(string filePath)
        {
            var file = _fileSystemFactory.GetFile(filePath);
            if (file.DirectoryName == _sourceFolder.Path)
            {
                if (!_settings.DryRun)
                    file.Delete();

                _logger.Info("Deleted file from source at: {0}\r\n", filePath);
            }
            else
            {
                var folderParts = filePath.Substring(_sourceFolder.Path.Length + 1).Split('\\');
                var rootFolder = _fileSystemFactory.GetDirectory(_sourceFolder.Path + "\\" + folderParts[0]);

                if (!rootFolder.Exists()) return;

                if(!_settings.DryRun)
                    rootFolder.Delete(true);

                _logger.Info("Deleted folder from source at: {0}\r\n", rootFolder.Path);
            }


        }
    }
}
