using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TvTamer.Core.Models;

namespace TvTamer.Core
{
    public static class TvEpisodeFilter
    {

        private readonly static string episodePattern = @"[Ss](?<season>\d{1,2})[Ee](?<episode>\d{1,2})";
        private readonly static string episodePattern2 = @"(?:[^\\]\\)*(?<SeriesName>.+?) *S?(?<SeasonNumber>[0-9]+)(?:[ .XE]+(?<EpisodeNumber>[0-9]+))";

        public static string GetSeriesName(string fileName)
        {
            return GetTvEpisode(fileName).SeriesName;
        }

        public static TvEpisode GetTvEpisode(string fileName)
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
                    return GetTvEpisode(fileNameParts[fileNameParts.Length - 2]);
                }

                return null;
            }

            var seriesName = match.Groups["SeriesName"].Value;
            seriesName = seriesName.Replace(".", " ").Replace("'", "").Trim();

            var seasonNumber = Convert.ToInt32(match.Groups["SeasonNumber"].Value);
            var episodeNumber = Convert.ToInt32(match.Groups["EpisodeNumber"].Captures[0].Value);

            return new TvEpisode() {EpisodeNumber = episodeNumber, Season = seasonNumber, SeriesName = seriesName, FileName = fullFileName};

        }

        public static IEnumerable<TvEpisode> GetEpisodes(IEnumerable<string> sources)
        {
            foreach(var source in sources)
            {
                if (!Regex.IsMatch(source, episodePattern)) continue;

                var episode = GetTvEpisode(source);
                if (episode != null)
                    yield return episode;
            }
        }

    }
}
