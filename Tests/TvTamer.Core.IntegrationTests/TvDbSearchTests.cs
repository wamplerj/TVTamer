using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TvTamer.Core.Persistance;

namespace TvTamer.Core.IntegrationTests
{
    [TestFixture]
    public class TvDbSearchTests
    {

        [Test]
        public void Searching_For_Tv_Show_Full_Title_Returns_Results()
        {
            var service = new TvDbSearchService();

            var results = service.FindTvSeries("Friends");

            Assert.That(results.Count(), Is.GreaterThan(1));

        }

        [Test]
        public void Lookup_Tv_Show_By_Id_Returns_Full_Series_Info()
        {
            var service = new TvDbSearchService();

            var result = service.GetTvSeries("80348");

            Assert.That(result, Is.Not.Null);

            var season2Count = result.Episodes.Count(x => x.Season == 2);
            Assert.That(season2Count, Is.EqualTo(22));

        }
    }
}
