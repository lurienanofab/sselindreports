using LNF.Web;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(sselIndReports.PageInitializer), "Initialize")]

namespace sselIndReports
{
    public class PageInitializer : PageInitializerModule
    {
        public static void Initialize()
        {
            RegisterModule(typeof(PageInitializer));
        }
    }
}