using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
            var regex = new Regex(episodePattern2, RegexOptions.IgnoreCase);

            var fileNameParts = fileName.Split('\\');
            fileName = fileNameParts[fileNameParts.Length - 1];

            var match = regex.Match(fileName);

            if (!match.Success) return null;

            var seriesName = match.Groups["SeriesName"].Value;
            seriesName = seriesName.Replace(".", " ");
            seriesName = seriesName.Replace("'", "");
            seriesName = seriesName.Trim();

            var seasonNumber = Convert.ToInt32(match.Groups["SeasonNumber"].Value);

            if (match.Groups["EpisodeNumber"].Captures.Count > 1)
            {
                //TODO Something weird
            }

            var episodeNumber = Convert.ToInt32(match.Groups["EpisodeNumber"].Captures[0].Value);

            return new TvEpisode() {EpisodeNumber = episodeNumber, Season = seasonNumber, SeriesName = seriesName, FileName = fileName};

        }

        public static IEnumerable<TvEpisode> GetEpisodes(IEnumerable<string> sources)
        {
            foreach(var source in sources)
            {
                if (Regex.IsMatch(source, episodePattern))
                    yield return GetTvEpisode(source);
            }
        }

    }
}
