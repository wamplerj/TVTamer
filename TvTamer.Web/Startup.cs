using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TvTamer.Web.Startup))]
namespace TvTamer.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
