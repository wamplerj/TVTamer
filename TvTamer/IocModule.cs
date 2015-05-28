using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.Persistance;

namespace TvTamer
{
    internal class IocModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {

            //TvTamer.Core Types
            builder.Register(c => ConfigurationManager.GetSection("episodeProcessorSettings") as EpisodeProcessorSettings);
            builder.Register(c => ConfigurationManager.GetSection("scheduleSettings") as ScheduleSettings);
            builder.RegisterType<TvDbSearchService>().As<ITvSearchService>();
            builder.RegisterType<TvContext>();
            builder.RegisterType<DatabaseUpdater>().As<IDatabaseUpdater>();
            builder.RegisterType<EpisodeProcessor>().As<IEpisodeProcessor>();

            builder.RegisterType<TvTamerService>();
        }

    }
}
