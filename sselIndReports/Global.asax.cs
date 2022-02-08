using LNF.Impl.DependencyInjection;
using LNF.Web;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Compilation;
using System.Web.Security;

namespace sselIndReports
{
    public class Global : HttpApplication
    {
        public static WebApp WebApp { get; private set; }

        void Application_Start(object sender, EventArgs e)
        {
            WebApp = new WebApp();

            // setup up dependency injection container
            WebApp.Context.EnablePropertyInjection();

            var wcc = new WebContainerConfiguration(WebApp.Context);
            wcc.RegisterAllTypes();

            // setup web dependency injection
            Assembly[] assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            WebApp.Bootstrap(assemblies);

            if (LNF.Configuration.Current.Production)
                Application["AppServer"] = "http://" + Environment.MachineName + ".eecs.umich.edu/";
            else
                Application["AppServer"] = "/";
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                FormsIdentity ident = (FormsIdentity)User.Identity;
                string[] roles = ident.Ticket.UserData.Split('|');
                Context.User = new GenericPrincipal(ident, roles);
            }
        }

        void Session_Start(object sender, EventArgs e)
        {

        }
    }
}
