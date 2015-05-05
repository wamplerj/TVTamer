﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TvTamer.Core.Models;
using System.IO.Compression;
using System.Xml.Linq;

namespace TvTamer.Core
{
    public interface ITvSearchService
    {
        IEnumerable<TvSeries> FindTvShow(string name);
        TvSeries GetTvSeries(string id);
        bool HasUpdates(string seriesId, DateTime lastUpdated);
    }

    public class TvDbSearchService : ITvSearchService
    {

        private const string _TvDbApiKey = "606A01BB48D22619";

        public IEnumerable<TvSeries> FindTvShow(string name)
        {
            var series = new List<TvSeries>();
            var url = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}", name);

            var client = WebRequest.Create(url);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(client.GetResponse().GetResponseStream());

            foreach (XmlNode node in xmlDoc.SelectNodes("Data/Series"))
            {
                if (node["FirstAired"] == null) continue;

                var overview = node["Overview"]?.InnerText ?? string.Empty;

                var tvSeries = new TvSeries()
                {
                    SeriesId = node["id"].InnerText,
                    Name = node["SeriesName"].InnerText,
                    FirstAired = Convert.ToDateTime(node["FirstAired"].InnerText),
                    Summary = overview
                };
                series.Add(tvSeries);
            }

            return series;

        }

        private List<string> GetGenres(string delmitedGenres)
        {
            return delmitedGenres.Split('|').ToList();
        }

        public TvSeries GetTvSeries(string id)
        {
            var url = $"http://thetvdb.com/api/{_TvDbApiKey}/series/{id}/all/en.zip";
            var temporaryFilePath = Path.GetTempFileName();

            TvSeries tvSeries = null;

            var client = new WebClient();
            client.DownloadFile(url, temporaryFilePath);

            if (!File.Exists(temporaryFilePath)) return tvSeries;

            var extractFolder = temporaryFilePath + "_extract\\";
            ZipFile.ExtractToDirectory(temporaryFilePath, extractFolder);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(extractFolder + "en.xml");

            var seriesNode = xmlDoc.SelectSingleNode("Data/Series");

            var overview = seriesNode["Overview"]?.InnerText ?? string.Empty;

            tvSeries = new TvSeries()
            {
                SeriesId = seriesNode["id"].InnerText,
                Name = seriesNode["SeriesName"].InnerText,
                FirstAired = Convert.ToDateTime(seriesNode["FirstAired"].InnerText),
                Status = seriesNode["Status"].InnerText,
                Summary = overview,
                AirsTimeOfDay = seriesNode["Airs_Time"]?.InnerText ?? string.Empty,
                Network = seriesNode["Network"]?.InnerText ?? string.Empty,
                Rating = seriesNode["ContentRating"]?.InnerText ?? string.Empty,
            };

            if (seriesNode["Airs_DayOfWeek"] != null)
            {
                DayOfWeek airDate;
                Enum.TryParse<DayOfWeek>(seriesNode["Airs_DayOfWeek"].InnerText, out airDate);
                tvSeries.AirsDayOfWeek = airDate;

            }

            tvSeries.LastUpdated = seriesNode["LastUpdated"] != null ? Convert.ToInt64(seriesNode["LastUpdated"]?.InnerText).FromEpochTime() : DateTime.Now;

            if (seriesNode["Genre"] != null)
                tvSeries.Genres = GetGenres(seriesNode["Genre"].InnerText);


            var episodeNodes = xmlDoc.SelectNodes("Data/Episode");

            if (episodeNodes != null)
            {

                foreach (XmlNode node in episodeNodes)
                {

                    if (string.IsNullOrEmpty(node["FirstAired"]?.InnerText)) continue;

                    var episode = new TvEpisode();
                    episode.EpisodeNumber = Convert.ToInt32(node["EpisodeNumber"].InnerText);
                    episode.Season = Convert.ToInt32(node["SeasonNumber"].InnerText);
                    episode.Title = node["EpisodeName"].InnerText;
                    episode.FirstAired = Convert.ToDateTime(node["FirstAired"].InnerText);

                    overview = node["Overview"]?.InnerText ?? string.Empty;
                    episode.Summary = overview;

                    tvSeries.Episodes.Add(episode);
                }
            }

            File.Delete(temporaryFilePath);
            Directory.Delete(extractFolder, recursive: true);

            return tvSeries;
        }

        public bool HasUpdates(string seriesId, DateTime lastUpdated)
        {
            var url = $"http://thetvdb.com/api/{_TvDbApiKey}/series/{seriesId}/en.xml";
            var epochtime = lastUpdated.ToEpochTime();

            var client = WebRequest.Create(url);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(client.GetResponse().GetResponseStream());

            var tvdbLastUpdated = xmlDoc.SelectSingleNode("Data/Series/lastupdated")?.InnerText;

            if (string.IsNullOrEmpty(tvdbLastUpdated)) return true;

            return Convert.ToInt64(tvdbLastUpdated) > epochtime;
        }
    }
}
