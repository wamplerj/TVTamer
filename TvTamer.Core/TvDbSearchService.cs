using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TvTamer.Core.Models;
using System.IO.Compression;

namespace TvTamer.Core
{
    public class TvDbSearchService
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

        public TvSeries GetTvSeries(string id)
        {
            var url = string.Format("http://thetvdb.com/api/{0}/series/{1}/all/en.zip", _TvDbApiKey, id);
            var temporaryFilePath = Path.GetTempFileName();

            TvSeries tvSeries = null;

            var client = new WebClient();
            client.DownloadFile(url, temporaryFilePath);

            if (File.Exists(temporaryFilePath))
            {
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
                    Summary = overview
                };

                var episodeNodes = xmlDoc.SelectNodes("Data/Episode");

                if (episodeNodes != null)
                {

                    foreach (XmlNode node in episodeNodes)
                    {

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
                Directory.Delete(extractFolder, recursive:true);
            }

            return tvSeries;
        }


    }
}
