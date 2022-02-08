using LNF;
using LNF.Data;
using LNF.Web;
using sselIndReports.AppCode;
using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public struct ReportButton
    {
        public ReportButton(Button button, ReportPage page)
        {
            Button = button;
            Page = page;
        }

        public Button Button { get; }
        public ReportPage Page { get; }
    }

    public partial class Index : LNF.Web.Content.OnlineServicesPage
    {
        private readonly List<ReportButton> appPages = new List<ReportButton>();

        public override ClientPrivilege AuthTypes
        {
            get { return 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // store the relationship between the buttons and pages
            // there should be one line per button, equals number of pages
            // adding makes the button visible

            appPages.Add(new ReportButton(btnIndDetUsage, new IndDetUsage()));
            //appPages.Add(new ReportButton(trIndSumUsage, btnIndSumUsage, new IndSumUsage()));
            appPages.Add(new ReportButton(btnIndAuthTools, new IndAuthTools()));
            appPages.Add(new ReportButton(btnAggSumUsage, new AggSumUsage()));
            appPages.Add(new ReportButton(btnAggDemographic, new AggDemographic()));
            //appPages.Add(new ReportButton(trAggNNIN, btnAggNNIN, new AggNNIN()));
            appPages.Add(new ReportButton(btnAggNNIN2, new AggNNIN2()));
            appPages.Add(new ReportButton(btnDatClient, new DatClient()));
            appPages.Add(new ReportButton(btnDatAccount, new DatAccount()));
            appPages.Add(new ReportButton(btnDatOrganization, new DatOrganization()));
            appPages.Add(new ReportButton(btnIndClientAccount, new IndClientAccount()));
            appPages.Add(new ReportButton(btnAggFeeComparison, new AggFeeComparison()));
            appPages.Add(new ReportButton(btnAggSubsidyByFacultyGroup, new AggSubsidyByFacultyGroup()));
            appPages.Add(new ReportButton(btnIndUserUsageSummary20100701, new IndUserUsageSummary20100701()));
            appPages.Add(new ReportButton(btnIndUserUsageSummary20110401, new IndUserUsageSummary20110401()));
            appPages.Add(new ReportButton(btnIndUserUsageSummary20111101, new IndUserUsageSummary20111101()));
            appPages.Add(new ReportButton(btnIndUserUsageSummaryAudit, new IndUserUsageSummaryAudit()));
            appPages.Add(new ReportButton(btnDatHistory, new DatHistory()));

            if (!Page.IsPostBack)
            {
                // check to see if session is valid
                if (Request.QueryString.Count > 0) // probably coming from sselOnLine
                {
                    string strClientID = Request.QueryString["ClientID"];
                    if (int.TryParse(strClientID.Trim(), out int clientId))
                    {
                        if (CurrentUser.ClientID != clientId)
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
                btnAggNNIN2.ToolTip = "Creates the spreadsheet required for monthly NNIN reporting";
                btnDatClient.ToolTip = "Database report for a client - shows affiliations, acocunts, etc";
                btnDatAccount.ToolTip = "Database report for accounts";
                btnDatOrganization.ToolTip = "Database report an organization - shows all accounts, clients, etc";
                btnIndClientAccount.ToolTip = "Clients and Accounts association report";
                btnIndUserUsageSummary20111101.ToolTip = "Displays summary of a users usage - total time in labs per day, total tool time, and store charges";
                btnIndUserUsageSummaryAudit.ToolTip = "Displays an audit-friendly version of the User Usage Summary report.";

                foreach (ReportButton a in appPages)
                {
                    string reportType = a.Button.ID.Substring(3, 3);

                    bool display = DisplayButton(a);

                    HtmlGenericControl cell = (HtmlGenericControl)FindControlRecursive("div" + reportType);
                    cell.Visible |= display;

                    Label lbl = (Label)FindControlRecursive("lbl" + reportType);
                    lbl.Visible |= display;
                }

                lblName.Text = CurrentUser.DisplayName;
            }
        }

        private bool DisplayButton(ReportButton repbtn)
        {
            bool visible = CurrentUser.HasPriv(repbtn.Page.AuthTypes) && repbtn.Page.ShowButton;
            repbtn.Button.Visible = visible;
            return visible;
        }

        // handles all button clicks
        protected void Button_Command(object sender, CommandEventArgs e)
        {
            string redirectPage;

            if (e.CommandName == "navigate")
                redirectPage = e.CommandArgument.ToString();
            else
                redirectPage = string.Format("{0}.aspx{1}", e.CommandName, e.CommandArgument);

            Response.Redirect(redirectPage);
        }

        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            ContextBase.RemoveCacheData();
            Session.Abandon();
            Response.Redirect(ServiceProvider.Current.LoginUrl() + "?Action=Blank");
        }
    }
}