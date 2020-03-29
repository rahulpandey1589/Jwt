using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OAuthAuthentication.Startup))]
namespace OAuthAuthentication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
