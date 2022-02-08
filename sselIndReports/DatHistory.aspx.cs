using LNF.Data;
using sselIndReports.AppCode;
using System;

namespace sselIndReports
{
    public partial class DatHistory : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Developer | ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string redirectUrl = "/data/dispatch/historical-database-report?returnTo=" + Server.UrlEncode("/sselindreports");
            hypRedirect.NavigateUrl = redirectUrl;
            Response.Redirect(redirectUrl);
        }
    }
}