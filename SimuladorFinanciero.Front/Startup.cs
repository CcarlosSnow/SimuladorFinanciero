using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimuladorFinanciero.Front.Startup))]
namespace SimuladorFinanciero.Front
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
