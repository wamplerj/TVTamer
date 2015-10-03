using Moq;
using NUnit.Framework;
using TvTamer.Core.Torrents;

namespace TvTamer.Core.IntegrationTests
{
    [TestFixture]
    public class SearchProviderTests
    {

        [Test]
        public void KickAssSearchProvider_Returns_First_Value()
        {
            var analyticService = new Mock<IAnalyticsService>();

            var searchProvider = new KickassSearchProvider(new NullSearchProvider(), new WebRequester(analyticService.Object), analyticService.Object);

            var result = searchProvider.GetTorrent("the big bang theory s01e01 720");

            Assert.That(result, Is.Not.Null);
        }


    }
}
