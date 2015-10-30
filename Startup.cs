using CallApp;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace CallApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Data.Load(app); // load contacts and numbers data
            Catapult.Configure(app); // initialize catapult client
        }
    }
}
