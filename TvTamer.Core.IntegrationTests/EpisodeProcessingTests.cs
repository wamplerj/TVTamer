using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TvTamer.Core.IntegrationTests
{
    [TestFixture]
    public class EpisodeProcessingTests
    {

        [Test]
        public void Test()
        {

            var processingFolder = @"c:\temp\TV_Source\";

            var processor = new EpisodeProcessor(processingFolder, "");
            var files = processor.GetTvEpisodeFiles().ToList();

            Assert.That(files, Is.Not.Null);

        }


    }
}
