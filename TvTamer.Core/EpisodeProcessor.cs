using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using NLog;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;
using Directory = TvTamer.Core.FileSystem.Directory;
using File = TvTamer.Core.FileSystem.File;

namespace TvTamer.Core
{
    public class EpisodeProcessor
    {
        private readonly Directory _sourceFolder;
        private readonly Directory _destinationFolder;

        private readonly Dictionary<string, string> _alternateSeriesNames = new Dictionary<string, string>()
        {
            {"Marvels Agents of S H I E L D","Marvels Agents of SHIELD"},
        };

        private readonly Logger _logger = LogManager.GetLogger("log");
        private readonly string[] _extensions = { ".mkv", ".mp4", ".m4v", ".avi", ".mpg", ".mpeg", ".wmv" };

        private readonly bool dryRun = false;  //TODO this should come via commandline argument

        public EpisodeProcessor(string sourceFolder, string destinationFolder)
        {
            _sourceFolder = new Directory(sourceFolder);
            _destinationFolder = new Directory(destinationFolder);
        }

        public void ProcessDownloadedEpisodes()
        {
            var episodes = GetTvEpisodeFiles();

            _logger.Info("Found {0} files in {1}", episodes.Count(), _sourceFolder);
            _logger.Info("===========================================================================");
            DisplayFiles(episodes.Select(e => e.FileName).ToList());

            var deletedFiles = new List<string>();
            foreach (var episode in episodes)
            {
                if (!DestinationFileExists(episode))
                {
                    CopyEpisodeToDestination(episode);
                }

                DeleteSourceFile(episode.FileName);
                deletedFiles.Add(episode.FileName);
            }

            _logger.Info("\r\n\r\nDeleted {0} files from {1}", deletedFiles.Count(), _sourceFolder);
            _logger.Info("===========================================================================");
            DisplayFiles(deletedFiles);

        }

        private void DisplayFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var fileName = file.Substring(_sourceFolder.Path.Length);
                if (fileName.Length > 70)
                {
                    var parts = fileName.Split('\\');
                    fileName = parts[0];
                }
                _logger.Info(fileName);
            }
        }

        public IEnumerable<TvEpisode> GetTvEpisodeFiles()
        {

            var files = _sourceFolder.EnumerateFiles("*", true)
                .Where(f => _extensions.Contains(new File(f).Extension.ToLower()) && !f.Contains("sample"));
            return TvEpisodeFilter.GetEpisodes(files);
        }

        private void CopyEpisodeToDestination(TvEpisode episode)
        {

            var context = new TvContext();
            var sourceFile = new File(episode.FileName);

            //TODO Look up episode name from DB
            var tvSeries =
                context.TvSeries.Include(e => e.Episodes).FirstOrDefault(s => s.Name.ToLower() == episode.SeriesName.ToLower());

            if (tvSeries == null) return;

            var episodeName =
                tvSeries.Episodes.Find(
                    e => e.Season == episode.Season && e.EpisodeNumber == episode.EpisodeNumber)?.Title;

            var destinationFilename = $"{_destinationFolder.Path}\\{episode.SeriesName}\\Season {episode.Season:D2}\\S{episode.Season:D2}E{episode.EpisodeNumber:D2} - {episodeName}{sourceFile.Extension}";

            sourceFile.Copy(destinationFilename);
        }

        public bool DestinationFileExists(TvEpisode episode)
        {

            if (_alternateSeriesNames.ContainsKey(episode.SeriesName))
                episode.SeriesName = _alternateSeriesNames[episode.SeriesName];

            var seriesDestinationFolderPath = $"{_destinationFolder}\\{episode.SeriesName}\\Season {episode.Season:D2}\\";
            var seriesDestinationFolder = new Directory(seriesDestinationFolderPath);

            if (!seriesDestinationFolder.Exists()) return false;

            var episodeSearchPattern = $"S{episode.Season:D2}E{episode.EpisodeNumber:D2}*";
            var files = seriesDestinationFolder.EnumerateFiles(episodeSearchPattern, false);

            return files.Any();

        }

        public void DeleteSourceFile(string filePath)
        {
            var file = new File(filePath);
            if (file.DirectoryName == _sourceFolder.Path)
            {
                file.Delete();
                _logger.Info("Deleted file from source at: {0}\r\n", filePath);
            }
            else
            {
                var folderParts = filePath.Substring(_sourceFolder.Path.Length).Split('\\');
                var rootFolder = new Directory(_sourceFolder + folderParts[0]);

                if (!rootFolder.Exists()) return;

                rootFolder.Delete(true);
                _logger.Info("Deleted folder from source at: {0}\r\n", rootFolder);
            }


        }
    }
}
