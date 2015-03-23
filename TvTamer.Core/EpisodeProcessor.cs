using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace TvTamer.Core
{
    public class EpisodeProcessor
    {
        private Logger _logger = LogManager.GetLogger("log");

        private readonly string tvLibraryFolder = @"c:\temp\library\";
        private readonly string[] extensions = { ".mkv", ".mp4", ".m4v", ".avi", ".mpg", ".mpeg", ".wmv" };
        
        private readonly bool dryRun = true;

        public IEnumerable<TvEpisode> GetTvEpisodeFiles(string folder)
        {

            var files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).Where(f => extensions.Contains(new FileInfo(f).Extension.ToLower()));
            return TvEpisodeFilter.GetEpisodes(files);
        }

        public bool DestinationFileExists(TvEpisode episode, string folder)
        {

            var destinationFolder = string.Format("{0}\\{1}\\Season {2:D2}\\", folder, episode.SeriesName, episode.Season);

            if (!Directory.Exists(destinationFolder)) return false;

            var episodeSearchPattern = string.Format("S{0:D2}E{1:D2}", episode.Season, episode.EpisodeNumber);
            var files = Directory.GetFiles(destinationFolder, episodeSearchPattern, SearchOption.TopDirectoryOnly);

            return files.Length > 0;

        }

        public void DeleteSourceFile(string filePath)
        {
            Console.WriteLine("Deleting file from source at : {0}", filePath);

            if (dryRun) return;

            try
            {
                File.Delete(filePath);
                _logger.Info("Deleted file from source at : {0}", filePath);
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
