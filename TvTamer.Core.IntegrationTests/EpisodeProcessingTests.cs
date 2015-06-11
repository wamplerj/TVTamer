using System.Linq;
using NUnit.Framework;
using TvTamer.Core.Configuration;
using TvTamer.Core.Persistance;

namespace TvTamer.Core.IntegrationTests
{
    [TestFixture]
    public class EpisodeProcessingTests
    {

        [Test]
        public void Test()
        {

            var settings = new EpisodeProcessorSettings() {DownloadFolder = @"c:\temp\TV_Source\", TvLibraryFolder = ""};
            var context = new TvContext();

            var processor = new EpisodeProcessor(settings, context);
            var files = processor.GetTvEpisodeFiles().ToList();

            Assert.That(files, Is.Not.Null);

        }


    }
}
