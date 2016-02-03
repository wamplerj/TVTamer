using System.Linq;
using NUnit.Framework;

namespace TvTamer.Core.UnitTests
{
    [TestFixture]
    public class TvEpisodeFilterTests
    {

        [Test]
        public void ValidEpisodeFormats()
        {

            var source = new[] {
                "The.Mindy.Project.S03E18.720p.HDTV.x264-KILLERS[rarbg]",
                "The.Walking.Dead.S05E12.720p.HDTV.x264-KILLERS",
                "The.Simpsons.S26E15.REAL.REPACK.720p.HDTV.x264-KILLERS.mkv",
                "testing s1e2 file.avi",
                "testing s01e02 - proper - eztv.mkv",
                "c:\\test\\Undateable.2014.S02E02.HDTV.x264-LOL[rarbg]\\undateable.202.hdtv-lol.mp4"
            };

            var matches = TvEpisodeFileMatcher.GetTvEpisodesFiles(source).ToList();
            Assert.IsTrue(matches.Count() == 6);
        }

    }
}
