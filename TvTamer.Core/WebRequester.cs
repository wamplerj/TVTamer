using System;
using System.Net;
using System.Xml;
using NLog;

namespace TvTamer.Core
{
    public interface IWebRequester
    {
        XmlDocument GetXml(string url, string referer = null);
        void DownloadFileAsync(string url, string filePath, string referer = null);
    }

    public class WebRequester : IWebRequester
    {

        private readonly Logger _logger = LogManager.GetLogger("log");

        public XmlDocument GetXml(string url, string referer = null)
        {

            _logger.Info($"Getting XML from url: {url}");

            var client = new GZipWebClient();

            if (!string.IsNullOrEmpty(referer))
                client.Headers.Add("Referer", referer);

            var xmlDoc = new XmlDocument();

            try
            {
                var xml = client.DownloadString(url);
                xmlDoc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return xmlDoc;

        }

        public void DownloadFileAsync(string url, string filePath, string referer = null)
        {
            var webclient = new GZipWebClient();

            if(!string.IsNullOrEmpty(referer))
                webclient.Headers.Add("Referer", referer);

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
                var request = (HttpWebRequest)base.GetWebRequest(address);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return request;
            }
        }
    }
}
