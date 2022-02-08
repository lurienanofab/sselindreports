using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.Billing;
using sselIndReports.AppCode;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndUserUsageSummaryAudit : UserUsageSummaryPage
    {
        // this report assumes all rates are hourly

        public bool ShowChargeMultiplierColumn { get; set; } = false;
        public bool ShowForgivenPercentageColumn { get; set; } = true;
        public bool ShowIsCancelledBeforeAllowedTimeColumn { get; set; } = true;

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator | ClientPrivilege.Developer; }
        }

        protected override DropDownList ClientDropDownList => ddlUser;

        protected override DateTime SelectedPeriod
        {
            get => pp1.SelectedPeriod;
            set => pp1.SelectedPeriod = value;
        }

        protected override void RunReport(DateTime period, int clientId)
        {
            Response.Redirect($"~/IndUserUsageSummaryAudit.aspx?ClientID={clientId}&Period={period:yyyy-MM-dd}");
        }

        protected override Button RetrieveDataButton => btnReport;

        protected override Label SummaryApproximateLabel => lblSummaryApproximate;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var clientId = GetClientIDFromQueryString();
                if (clientId > 0)
                {
                    var period = GetPeriodFromQueryString();
                    var resourceId = GetResourceIDFromQueryString();
                    LoadReport(clientId, period, resourceId);
                    SelectedClientID = clientId;
                    SelectedPeriod = period;
                }
            }
        }

        private int GetClientIDFromQueryString()
        {
            if (!int.TryParse(Request.QueryString["ClientID"], out int clientId))
                clientId = 0;
            return clientId;
        }

        private int GetResourceIDFromQueryString()
        {
            if (!int.TryParse(Request.QueryString["ResourceID"], out int resourceId))
                resourceId = 0;
            return resourceId;
        }

        DateTime GetPeriodFromQueryString()
        {
            if (!DateTime.TryParse(Request.QueryString["Period"], out DateTime period))
                period = DateTime.Now.FirstOfMonth().AddMonths(-1);
            return period;
        }

        private void LoadReport(int clientId, DateTime period, int resourceId)
        {
            var audit = new UserUsageAudit(Provider);
            audit.GetAuditData(period, clientId);

            var dtClient = audit.GetClientTable();

            phReport.Visible = dtClient.Rows.Count > 0;
            if (!phReport.Visible) return;

            DataTable dataSource;

            var drc = dtClient.Rows[0];

            var dtReportInfo = new DataTable();
            dtReportInfo.Columns.Add("Key", typeof(string));
            dtReportInfo.Columns.Add("Value", typeof(string));

            dtReportInfo.Rows.Add("Name", $"{drc["LName"]}, {drc["FName"]}");

            if (resourceId > 0)
            {
                // show individual reservations for the specified resource
                hypToolBackLink.NavigateUrl = $"?ClientID={clientId}&Period={period:yyyy-MM-dd}";
                phToolBackLink.Visible = true;
                dataSource = audit.GetDetailTable(resourceId);
                divToolBilling.Attributes["class"] = "tool-billing detail";

                // if there is any billing data add a line to report info
                if (dataSource.Rows.Count > 0)
                {
                    var rname = dataSource.Rows[0]["ResourceDisplayName"].ToString();
                    litSubtitle.Text = $"Reservation Details for {rname}";
                    dtReportInfo.Rows.Add("Tool", rname);
                }
            }
            else
            {
                // aggreate by resource
                litSubtitle.Text = "Tool Aggregate by Resource and Account";
                phToolBackLink.Visible = false;
                dataSource = audit.GetAggregateTable();
                divToolBilling.Attributes["class"] = "tool-billing aggregate";
            }

            dtReportInfo.Rows.Add("Period", period.ToString("MMMM yyyy"));
            dtReportInfo.Rows.Add("Created", DateTime.Now.ToString("M/d/yyyy h:mm:ss tt"));

            rptReportInfo.DataSource = dtReportInfo;
            rptReportInfo.DataBind();

            if (dataSource.Rows.Count > 0)
            {
                phTool.Visible = true;
                phToolNoData.Visible = false;
                rptToolBilling.DataSource = dataSource;
                rptToolBilling.DataBind();
            }
            else
            {
                phTool.Visible = false;
                phToolNoData.Visible = true;
            }
        }

        protected string GetResourceLink(object obj)
        {
            var drv = (DataRowView)obj;
            return $"<a href=\"?ClientID={GetClientIDFromQueryString()}&Period={GetPeriodFromQueryString():yyyy-MM-dd}&ResourceID={drv["ResourceID"]}\">{drv["ResourceDisplayName"]}</a>";
        }

        protected void DateSelect_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                var dir = e.CommandArgument.ToString();

                var currentPeriod = SelectedPeriod;

                if (dir == "next")
                    SelectedPeriod = currentPeriod.FirstOfMonth().AddMonths(1);
                else if (dir == "prev")
                    SelectedPeriod = currentPeriod.FirstOfMonth().AddMonths(-1);
                else
                    throw new Exception($"Unexpected CommandArgument: {dir}");

                // the only way ClientID is in the QueryString is if the Retrieve Data button was clicked
                // if this has happened at least once then load the data, but use the currently selected user in case it was changed
                if (GetClientIDFromQueryString() > 0)
                {
                    var resourceId = GetResourceIDFromQueryString();
                    if (resourceId == 0)
                        Response.Redirect($"~/IndUserUsageSummaryAudit.aspx?ClientID={SelectedClientID}&Period={SelectedPeriod:yyyy-MM-dd}");
                    else
                        Response.Redirect($"~/IndUserUsageSummaryAudit.aspx?ClientID={SelectedClientID}&ResourceID={resourceId}&Period={SelectedPeriod:yyyy-MM-dd}");
                }
            }
        }
    }
}