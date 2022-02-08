using LNF;
using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;

namespace sselIndReports
{
    public partial class ToolCharges : System.Web.UI.Page
    {
        private DateTime period;
        private int clientId;

        [Inject] public IProvider Provider { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            period = GetPeriod();
            clientId = GetClientID();
            LoadData();
        }

        private void LoadData()
        {
            var query = Provider.Billing.Tool.GetToolBilling(period, clientId);
            IEnumerable<IResource> resources = Provider.Scheduler.Resource.GetResources();
            IEnumerable<IAccount> accounts = Provider.Data.Account.GetAccounts();

            var toolBilling = new LNF.Reporting.Individual.ToolBilling(Provider);
            DataTable dtAggByTool = toolBilling.GetAggreateByTool(query, resources, accounts);
            DataTable dtToolCharges = toolBilling.GetToolCharges(dtAggByTool);
            rptToolCharges.DataSource = dtToolCharges;
            rptToolCharges.DataBind();
        }

        private DateTime GetPeriod()
        {
            if (string.IsNullOrEmpty(Request.QueryString["Period"]))
                throw new Exception("Missing required QueryString parameter: Period");

            if (DateTime.TryParse(Request.QueryString["Period"], out DateTime result))
                return result;
            else
                throw new Exception("Invalid DateTime QueryString parameter: Period");

        }

        private int GetClientID()
        {
            if (string.IsNullOrEmpty(Request.QueryString["ClientID"]))
                throw new Exception("Missing required QueryString parameter: ClientID");

            if (int.TryParse(Request.QueryString["ClientID"], out int result))
                return result;
            else
                throw new Exception("Invalid int QueryString parameter: Period");
        }
    }
}