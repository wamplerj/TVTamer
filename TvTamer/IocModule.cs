using System;
using System.Configuration;
using Autofac;
using TvTamer.Core;
using TvTamer.Core.Configuration;
using TvTamer.Core.Persistance;
using TvTamer.Core.Torrents;

namespace TvTamer
{
    internal class IocModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {

            //TvTamer.Core Types
            builder.Register(c => ConfigurationManager.GetSection("episodeProcessorSettings") as EpisodeProcessorSettings);
            builder.Register(c => ConfigurationManager.GetSection("scheduleSettings") as ScheduleSettings);
            builder.Register(c => ConfigurationManager.GetSection("torrentSearchSettings") as TorrentSearchSettings);

            builder.RegisterType<TvDbSearchService>().As<ITvSearchService>();
            builder.RegisterType<TvContext>();
            builder.RegisterType<DatabaseUpdater>().As<IDatabaseUpdater>();
            builder.RegisterType<EpisodeProcessor>().As<IEpisodeProcessor>();
            builder.RegisterType<EpisodeDownloader>().As<IEpisodeDownloader>();
            builder.RegisterType<WebRequester>().As<IWebRequester>();

            //Search Provider Registration
            builder.RegisterType<NullSearchProvider>().Named<ISearchProvider>("searchProvider");

            builder.RegisterDecorator<ISearchProvider>((c, inner) => new KickassSearchProvider(inner, c.Resolve<IWebRequester>()),
                fromKey: "searchProvider");

            builder.RegisterType<TvTamerService>();
        }

    }
}
