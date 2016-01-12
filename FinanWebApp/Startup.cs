using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinanWebApp.Startup))]
namespace FinanWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
