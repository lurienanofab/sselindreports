using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using sselIndReports.AppCode;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class index : LNF.Web.Content.LNFPage
    {
        private Dictionary<Button, ReportPage> appPages = new Dictionary<Button, ReportPage>();

        public override ClientPrivilege AuthTypes
        {
            get { return 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // store the relationship between the buttons and pages
            // there should be one line per button, equals number of pages

            appPages.Add(btnIndDetUsage, new IndDetUsage());
            //appPages.Add(btnIndSumUsage, new IndSumUsage());
            appPages.Add(btnIndAuthTools, new IndAuthTools());
            appPages.Add(btnAggSumUsage, new AggSumUsage());
            appPages.Add(btnAggDemographic, new AggDemographic());
            appPages.Add(btnAggNNIN, new AggNNIN());
            appPages.Add(btnAggNNIN2, new AggNNIN2());
            appPages.Add(btnDatClient, new DatClient());
            appPages.Add(btnDatAccount, new DatAccount());
            appPages.Add(btnDatOrganization, new DatOrganization());
            appPages.Add(btnIndClientAccount, new IndClientAccount());
            appPages.Add(btnAggFeeComparison, new AggFeeComparison());
            appPages.Add(btnAggSubsidyByFacultyGroup, new AggSubsidyByFacultyGroup());
            appPages.Add(btnIndUserUsageSummary20100701, new IndUserUsageSummary20100701());
            appPages.Add(btnIndUserUsageSummary20110401, new IndUserUsageSummary20110401());
            appPages.Add(btnIndUserUsageSummary20111101, new IndUserUsageSummary20111101());

            if (!Page.IsPostBack)
            {
                // check to see if session is valid
                if (Request.QueryString.Count > 0) // probably coming from sselOnLine
                {
                    int clientId;
                    string strClientID = Request.QueryString["ClientID"];
                    if (int.TryParse(strClientID.Trim(), out clientId))
                    {
                        if (CacheManager.Current.ClientID != clientId)
                        {
                            Session.Abandon();
                            Response.Redirect("~");
                        }
                    }
                }

                btnIndDetUsage.ToolTip = "Displays all user activity - enter/exit lab and tool reservations";
                btnIndSumUsage.ToolTip = "Displays summary of a users usage - total time in labs per day, total tool time, and store charges";
                btnIndAuthTools.ToolTip = "Displays the list of tools the selected user is authorized to use";
                btnAggSumUsage.ToolTip = "Displays a listing of the number of entries and total time spent by active users in each of the passback controlled rooms";
                btnAggDemographic.ToolTip = "Displays a report showing the number of hours spent in each passback controlled room by demographic category";
                btnAggNNIN.ToolTip = "Creates the spreadsheet required for monthly NNIN reporting";
                btnDatClient.ToolTip = "Database report for a client - shows affiliations, acocunts, etc";
                btnDatAccount.ToolTip = "Database report for accounts";
                btnDatOrganization.ToolTip = "Database report an organization - shows all accounts, clients, etc";
                btnIndClientAccount.ToolTip = "Clients and Accounts association report";
                btnIndUserUsageSummary20111101.ToolTip = "Displays summary of a users usage - total time in labs per day, total tool time, and store charges";

                foreach (KeyValuePair<Button, ReportPage> kvp in appPages)
                {
                    string reportType = kvp.Key.ID.Substring(3, 3);
                    Label lbl = (Label)FindControlRecursive("lbl" + reportType);
                    lbl.Visible = lbl.Visible | DisplayButton(kvp.Key, kvp.Value);
                }

                btnDatHistory.Visible = CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Developer | ClientPrivilege.Administrator);

                lblName.Text = CacheManager.Current.CurrentUser.DisplayName;
            }
        }

        private bool DisplayButton(Button btn, ReportPage page)
        {
            btn.Visible = CurrentUser.HasPriv(page.AuthTypes) && page.ShowButton;
            return btn.Visible;
        }

        // handles all button clicks
        protected void Button_Command(object sender, CommandEventArgs e)
        {
            string redirectPage = string.Empty;

            if (e.CommandName == "navigate")
                redirectPage = e.CommandArgument.ToString();
            else
                redirectPage = string.Format("{0}.aspx{1}", e.CommandName, e.CommandArgument);

            Response.Redirect(redirectPage);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            CacheManager.Current.RemoveCacheData();
            Session.Abandon();
            Response.Redirect(CacheManager.Current.Logout + "?Action=Blank");
        }
    }
}