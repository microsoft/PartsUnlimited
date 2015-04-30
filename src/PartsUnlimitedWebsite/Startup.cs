using Microsoft.Owin;
using Owin;
using PartsUnlimited;
[assembly: OwinStartup(typeof(Startup))]

namespace PartsUnlimited
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
