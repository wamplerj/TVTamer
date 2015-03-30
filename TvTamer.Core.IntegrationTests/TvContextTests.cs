using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TvTamer.Core.Persistance;

namespace TvTamer.Core.IntegrationTests
{
    [TestFixture]
    public class TvContextTests
    {

        [SetUp]
        public void Setup()
        {
            Database.SetInitializer(new IntegrationDbInitializer());
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""));
        }

        [Test]
        public void Get_First_Tv_Series()
        {
            var context = new TvContext();
            var result = context.TvSeries.First();

            Assert.That(result, Is.Not.Null);

        }
    }
}
