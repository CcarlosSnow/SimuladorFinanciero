using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimuladorFinanciero.Startup))]
namespace SimuladorFinanciero
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
