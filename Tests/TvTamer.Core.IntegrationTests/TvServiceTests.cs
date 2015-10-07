using NUnit.Framework;
using TvTamer.Core.Persistance;

namespace TvTamer.Core.IntegrationTests
{
    [TestFixture]
    public class TvServiceTests
    {
        [Test]
        public void GetSeriesWithSeriesNameReturnsSeries()
        {

            var service = new TvService(new TvContext());
            var result = service.GetEpisodeBySeriesName("Grey's Anatomy", 1, 1);

            Assert.That(result.Id, Is.EqualTo(1379));

        }

        [Test]
        public void GetSeriesWithAlternateSeriesNameReturnsSeries()
        {
            var service = new TvService(new TvContext());
            var result = service.GetEpisodeBySeriesName("Greys Anatomy", 1, 1, true);

            Assert.That(result.Id, Is.EqualTo(1379));
        }
    }
}