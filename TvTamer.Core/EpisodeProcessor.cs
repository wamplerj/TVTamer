using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace TvTamer.Core
{
    public class EpisodeProcessor
    {
        private readonly string _sourceFolder;
        private readonly string _destinationFolder;

        public EpisodeProcessor(string sourceFolder, string destinationFolder)
        {
            _sourceFolder = sourceFolder;
            _destinationFolder = destinationFolder;
        }

        private readonly Dictionary<string, string> _alternateSeriesNames = new Dictionary<string, string>()
        {
            {"Marvels Agents of S H I E L D","Marvels Agents of SHIELD"},
        };

        private Logger _logger = LogManager.GetLogger("log");
        private readonly string[] _extensions = { ".mkv", ".mp4", ".m4v", ".avi", ".mpg", ".mpeg", ".wmv" };

        private readonly bool dryRun = false;

        public IEnumerable<TvEpisode> GetTvEpisodeFiles()
        {

            var files = Directory.EnumerateFiles(_sourceFolder, "*", SearchOption.AllDirectories).Where(f => _extensions.Contains(new FileInfo(f).Extension.ToLower()));
            return TvEpisodeFilter.GetEpisodes(files);
        }

        public bool DestinationFileExists(TvEpisode episode)
        {

            if (_alternateSeriesNames.ContainsKey(episode.SeriesName))
                episode.SeriesName = _alternateSeriesNames[episode.SeriesName];

            var seriesDestinationFolder = string.Format("{0}\\{1}\\Season {2:D2}\\", _destinationFolder, episode.SeriesName, episode.Season);

            if (!Directory.Exists(seriesDestinationFolder)) return false;

            var episodeSearchPattern = string.Format("S{0:D2}E{1:D2}*", episode.Season, episode.EpisodeNumber);
            var files = Directory.GetFiles(seriesDestinationFolder, episodeSearchPattern, SearchOption.TopDirectoryOnly);

            return files.Length > 0;

        }

        public void DeleteSourceFile(string filePath)
        {

            try
            {

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.DirectoryName == _sourceFolder)
                {
                    fileInfo.Delete();
                    _logger.Info("Deleted file from source at: {0}\r\n", filePath);
                }
                else
                {
                    var folderParts = filePath.Substring(_sourceFolder.Length).Split('\\');
                    var rootFolder = _sourceFolder + folderParts[0];

                    if (!Directory.Exists(rootFolder)) return;

                    Directory.Delete(rootFolder, true);
                    _logger.Info("Deleted folder from source at: {0}\r\n", rootFolder);
                }

            }
            catch (UnauthorizedAccessException auth)
            {
                _logger.Error("Could not delete file at ");
            }
            catch (DirectoryNotFoundException dir)
            {
                //todo
                _logger.Error("Could not delete file at ");
            }
            catch (IOException io)
            {
                //TODO if the file can't be accessed, mark it for future delete and log exception
            }
        }
    }
}
