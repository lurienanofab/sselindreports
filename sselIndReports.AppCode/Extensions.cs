using System;
using System.Web;

namespace sselIndReports.AppCode
{
    public static class HttpContextBaseExtensions
    {
        public static bool Updated(this HttpContextBase context)
        {
            if (context.Session["Updated"] != null)
                return Convert.ToBoolean(context.Session["Updated"]);
            return false;
        }

        public static void Updated(this HttpContextBase context, bool value)
        {
            context.Session["Updated"] = value;
        }
    }
}
