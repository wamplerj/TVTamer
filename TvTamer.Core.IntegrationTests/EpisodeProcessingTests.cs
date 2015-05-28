using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TvTamer.Core.Configuration;

namespace TvTamer.Core.IntegrationTests
{
    [TestFixture]
    public class EpisodeProcessingTests
    {

        [Test]
        public void Test()
        {

            var settings = new EpisodeProcessorSettings() {DownloadFolder = @"c:\temp\TV_Source\", TvLibraryFolder = ""};

            var processor = new EpisodeProcessor(settings);
            var files = processor.GetTvEpisodeFiles().ToList();

            Assert.That(files, Is.Not.Null);

        }


    }
}
