using LNF.Web;
using Owin;

namespace sselIndReports
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDataAccess(Global.WebApp.Context);
        }
    }
}