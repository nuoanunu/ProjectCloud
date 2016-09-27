using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UniversityDbWeb.Startup))]
namespace UniversityDbWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
