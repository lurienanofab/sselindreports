﻿using LNF;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace sselIndReports
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            if (Providers.IsProduction())
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
