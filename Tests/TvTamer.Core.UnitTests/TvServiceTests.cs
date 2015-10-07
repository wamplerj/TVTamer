using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

namespace TvTamer.Core.UnitTests
{
    [TestFixture]
    public class TvServiceTests
    {

        [Test]
        public void GetEpisodeBySeriesName_ReturnsEpisode()
        {

            var episode = new TvEpisode() { Season = 5, EpisodeNumber = 12, Title = "Some Title" };
            var series = new TvSeries() { Name = "The Walking Dead" };
            series.Episodes.Add(episode);

            var episodeQueryable = new List<TvEpisode> { episode }.AsQueryable();
            var seriesList = new List<TvSeries> { series };

            var seriesSet = new Mock<MockableDbSetWithExtensions<TvSeries>>();
            seriesSet.Setup(ss => ss.SqlQuery(It.IsAny<string>(), It.IsAny<object[]>()))
                            .Returns<string, object[]>((sql, param) =>
                            {
                                var sqlQueryMock = new Mock<DbSqlQuery<TvSeries>>();
                                sqlQueryMock.Setup(m => m.AsNoTracking())
                                    .Returns(sqlQueryMock.Object);
                                sqlQueryMock.Setup(m => m.GetEnumerator())
                                    .Returns(seriesList.GetEnumerator());
                                return sqlQueryMock.Object;
                            });

            var episodeSet = new Mock<MockableDbSetWithExtensions<TvEpisode>>();
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.Provider).Returns(episodeQueryable.Provider);
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.Expression).Returns(episodeQueryable.Expression);
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.ElementType).Returns(episodeQueryable.ElementType);
            episodeSet.As<IQueryable<TvEpisode>>().Setup(m => m.GetEnumerator()).Returns(episodeQueryable.GetEnumerator());

            var context = new Mock<ITvContext>();
            context.Setup(c => c.Set<TvSeries>()).Returns(seriesSet.Object);
            context.Setup(c => c.TvEpisodes).Returns(episodeSet.Object);

            var tvService = new TvService(context.Object);

            var result = tvService.GetEpisodeBySeriesName("The Walking Dead", 5, 12, false);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Some Title"));

        }

    }
    
    public abstract class MockableDbSetWithExtensions<T> : DbSet<T> where T : class
    {
        public abstract void AddOrUpdate(params T[] entities);
        public abstract void AddOrUpdate(Expression<Func<T, object>> identifierExpression, params T[] entities);
    }
}
