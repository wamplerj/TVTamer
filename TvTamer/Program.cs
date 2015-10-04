using System;
using Autofac;
using NLog;
using Topshelf;
using Topshelf.Autofac;


namespace TvTamer
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetLogger("log");


        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += (o, ea) => _logger.Fatal("Unhandled exception: {0}", ea.ExceptionObject);

            var builder = new ContainerBuilder();
            builder.RegisterModule(new IocModule());
            var container = builder.Build();

            HostFactory.Run(hc =>                                 
            {
                hc.UseAutofacContainer(container);
                //hc.UseNLog();
                hc.Service<TvTamerService>(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                hc.RunAsLocalSystem();

                hc.SetDescription("TVTamer");
                hc.SetDisplayName("TVTamer");
                hc.SetServiceName("TVTamer");
            });

            #if DEBUG
            Console.ReadKey();
            #endif

        }

    }
}
