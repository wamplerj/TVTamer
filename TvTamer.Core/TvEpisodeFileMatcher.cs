using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TvTamer.Core.FileSystem;
using TvTamer.Core.Models;

namespace TvTamer.Core
{
    public static class TvEpisodeFileMatcher
    {

        private readonly static string episodePattern = @"[Ss](?<season>\d{1,2})[Ee](?<episode>\d{1,2})";
        private readonly static string episodePattern2 = @"(?:[^\\]\\)*(?<SeriesName>.+?) *S?(?<SeasonNumber>[0-9]+)(?:[ .XE]+(?<EpisodeNumber>[0-9]+))";

        public static TvEpisodeFile GetTvEpisodeFile(string fileName)
        {

            var fullFileName = fileName;
            var regex = new Regex(episodePattern2, RegexOptions.IgnoreCase);

            var fileNameParts = fileName.Split('\\');
            fileName = fileNameParts[fileNameParts.Length - 1];

            var match = regex.Match(fileName);

            if (!match.Success)
            {
                //If the file does not match the SXXExx naming pattern, then test its parent directory
                if (fileNameParts.Length >= 2)
                {
                    return GetTvEpisodeFile(fileNameParts[fileNameParts.Length - 2]);
                }

                return null;
            }

            var seriesName = match.Groups["SeriesName"].Value;
            seriesName = seriesName.Replace(".", " ").Replace("'", "").Trim();

            var seasonNumber = Convert.ToInt32(match.Groups["SeasonNumber"].Value);
            var episodeNumber = Convert.ToInt32(match.Groups["EpisodeNumber"].Captures[0].Value);

            return new TvEpisodeFile(fileName) {EpisodeNumber = episodeNumber, Season = seasonNumber, SeriesName = seriesName, FileName = fullFileName};

        }

        public static IEnumerable<TvEpisodeFile> GetTvEpisodesFiles(IEnumerable<string> sources)
        {
            return sources.Where(source => Regex.IsMatch(source, episodePattern)).Select(GetTvEpisodeFile).Where(file => file != null);
        }
    }
}
