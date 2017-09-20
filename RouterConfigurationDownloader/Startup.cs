using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RouterConfigurationDownloader.Startup))]
namespace RouterConfigurationDownloader
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
