using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EPMS_Darpan.Startup))]
namespace EPMS_Darpan
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
