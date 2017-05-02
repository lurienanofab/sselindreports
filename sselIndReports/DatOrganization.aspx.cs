using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using sselIndReports.AppCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

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
            List<OrgItem> dataSource = null;

            if (CurrentUser.HasPriv(ClientPrivilege.Administrator))
            {
                IList<Org> allOrgs = DA.Current.Query<Org>().ToList();
                dataSource = allOrgs.Select(x => new OrgItem() { OrgID = x.OrgID, OrgName = x.OrgName }).OrderBy(x => x.OrgName).ToList();
            }
            else if (CurrentUser.HasPriv(ClientPrivilege.Executive))
            {
                IList<ClientModel> allClientOrgs = CacheManager.Current.CurrentUserActiveClientOrgs();
                dataSource = allClientOrgs.Select(x => new OrgItem() { OrgID = x.OrgID, OrgName = x.OrgName }).OrderBy(x => x.OrgName).ToList();
            }

            if (dataSource.Count > 1)
                ddlOrg.AppendDataBoundItems = true;

            ddlOrg.DataSource = dataSource;
            ddlOrg.DataTextField = "OrgName";
            ddlOrg.DataValueField = "OrgID";
            ddlOrg.DataBind();
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            gv.DataSourceID = "odsGrid";
            gv.DataBind();
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (ddlOrg.SelectedValue == "17")
            {
                gv.Columns[1].Visible = true;
                gv.Columns[2].Visible = true;
            }
            else
            {
                gv.Columns[1].Visible = false;
                gv.Columns[2].Visible = false;
            }
        }
    }
}