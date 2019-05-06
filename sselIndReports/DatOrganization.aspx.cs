using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using sselIndReports.AppCode;
using sselIndReports.AppCode.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Diagnostics;
using LNF.Web;

namespace sselIndReports
{
    public partial class DatOrganization : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator | ClientPrivilege.Executive; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            IEnumerable<OrgListItem> dataSource = null;

            if (CurrentUser.HasPriv(ClientPrivilege.Administrator))
            {
                IList<Org> allOrgs = DA.Current.Query<Org>().ToList();
                dataSource = allOrgs.Select(x => new OrgListItem() { OrgID = x.OrgID, OrgName = x.OrgName }).OrderBy(x => x.OrgName);
            }
            else if (CurrentUser.HasPriv(ClientPrivilege.Executive))
            {
                var allClientOrgs = ContextBase.GetCurrentUserClientOrgs();
                dataSource = allClientOrgs.Select(x => new OrgListItem() { OrgID = x.OrgID, OrgName = x.OrgName }).OrderBy(x => x.OrgName);
            }

            if (dataSource.Count() > 1)
                ddlOrg.AppendDataBoundItems = true;

            ddlOrg.DataSource = dataSource;
            ddlOrg.DataTextField = "OrgName";
            ddlOrg.DataValueField = "OrgID";
            ddlOrg.DataBind();
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            var year = pp1.SelectedYear;
            var month = pp1.SelectedMonth;
            var orgId = int.Parse(ddlOrg.SelectedValue);

            var sw = Stopwatch.StartNew();
            var dt = AccountDA.GetAccountDetailsByOrgID(year, month, orgId);
            sw.Stop();

            litDebug.Text = $"<div class=\"debug\"><em>Query completed in {sw.Elapsed.TotalSeconds:0.00} seconds</em></div>";

            gvReport.DataSource = dt;
            gvReport.DataBind();
        }

        protected void GvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (ddlOrg.SelectedValue == "17")
            {
                gvReport.Columns[1].Visible = true;
                gvReport.Columns[2].Visible = true;
            }
            else
            {
                gvReport.Columns[1].Visible = false;
                gvReport.Columns[2].Visible = false;
            }
        }
    }
}