using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(POR.Startup))]
namespace POR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
