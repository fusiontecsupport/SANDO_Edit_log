using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(scfs_erp.Startup))]
namespace scfs_erp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
