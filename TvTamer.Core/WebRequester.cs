using System;
using System.Net;
using System.Xml;
using NLog;

namespace TvTamer.Core
{
    public interface IWebRequester
    {
        XmlDocument GetXml(string url);
        void DownloadFileAsync(string url, string filePath);
    }

    public class WebRequester : IWebRequester
    {

        private readonly Logger _logger = LogManager.GetLogger("log");

        public XmlDocument GetXml(string url)
        {

            _logger.Info($"Getting XML from url: {url}");

            var client = WebRequest.Create(url);
            var xmlDoc = new XmlDocument();

            try
            {
                var response = client.GetResponse();
                xmlDoc.Load(response.GetResponseStream());
            }
            catch (WebException ex)
            {
                _logger.Error(ex);
            }

            return xmlDoc;

        }

        public void DownloadFileAsync(string url, string filePath)
        {
            var webclient = new GZipWebClient();

            try
            {
                _logger.Info($"Downloading file: {url} and saving to {filePath}");
                webclient.DownloadFileAsync(new Uri(url), filePath);
            }
            catch (WebException ex)
            {
                _logger.Error(ex);
            }
        }

        internal class GZipWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return request;
            }
        }
    }
}
