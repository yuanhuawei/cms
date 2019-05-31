using Microsoft.Owin;
using Owin;
using MaiDarServer.API;
[assembly: OwinStartup(typeof(Startup))]

namespace MaiDarServer.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}