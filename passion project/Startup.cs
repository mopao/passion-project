using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(passion_project.Startup))]
namespace passion_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
