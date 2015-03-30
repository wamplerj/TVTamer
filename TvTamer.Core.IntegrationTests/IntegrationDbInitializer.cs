using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using TvTamer.Core.Models;
using TvTamer.Core.Persistance;

namespace TvTamer.Core.IntegrationTests
{
    public class IntegrationDbInitializer : DropCreateDatabaseAlways<TvContext>
    {
        protected override void Seed(TvContext context)
        {

            var tvSeries = new TvSeries
            {
                FirstAired = Convert.ToDateTime("1/1/2015"),
                Name = "Some Series Name",
                SeriesId = "123456",
                Status = "Ended",
                Summary =
                    "Some string goes here.  This could be really big or small.  Who know?  I am just going to keep typing until I feel tired."
            };

            var episodes = new List<TvEpisode>()
            {
                new TvEpisode()
                {
                    Season = 1,
                    EpisodeNumber = 1,
                    Title = "First Episode",
                    FirstAired = Convert.ToDateTime("1/1/2015")
                },
                new TvEpisode()
                {
                    Season = 1,
                    EpisodeNumber = 2,
                    Title = "SecondEpisode",
                    FirstAired = Convert.ToDateTime("1/8/2015")
                },
            };

            tvSeries.Episodes = episodes;

            context.TvSeries.Add(tvSeries);
            context.SaveChanges();
        }
    }
}
