using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SiebertPdfGenerator.Startup))]
namespace SiebertPdfGenerator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
