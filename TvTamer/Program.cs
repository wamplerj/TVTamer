using System;
using Autofac;
using Topshelf;
using Topshelf.Autofac;


namespace TvTamer
{
    class Program
    {
        static void Main(string[] args)
        {

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
