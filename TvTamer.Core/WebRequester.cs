using System.Net;
using System.Xml;
using NLog;

namespace TvTamer.Core
{
    public interface IWebRequester
    {
        XmlDocument GetXml(string url);
    }

    public class WebRequester : IWebRequester
    {

        private readonly Logger _logger = LogManager.GetLogger("log");

        public XmlDocument GetXml(string url)
        {

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






    }
}
