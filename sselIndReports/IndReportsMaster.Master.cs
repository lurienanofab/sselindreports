using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace sselIndReports
{
    public partial class IndReportsMaster : LNF.Web.Content.LNFMasterPage
    {
        public override bool ShowMenu
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["ShowMenu"]) || Request.QueryString["menu"] == "1";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litCommonToolsVersion.Text = string.Format("<!-- CommonTools Version: {0} -->", LNF.CommonTools.Utility.Version());
        }
    }
}