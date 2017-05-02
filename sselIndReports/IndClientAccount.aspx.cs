using LNF.Cache;
using LNF.Data;
using LNF.Data.ClientAccountMatrix;
using LNF.Models.Data;
using LNF.Repository.Data;
using sselIndReports.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndClientAccount : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return  ClientPrivilege.Administrator | ClientPrivilege.Executive; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session["ClientAccount_ManagerOrgID"] = 0;
                LoadManagers();
            }

            litMessage.Text = string.Empty;
            litMatrix.Text = string.Empty;
            int managerOrgId = Convert.ToInt32(ddlManager.SelectedValue);
            if (managerOrgId > 0)
            {
                Matrix m = new Matrix(managerOrgId, true);
                if (m.EmployeeCount > 0)
                {
                    if (m.EmployeeCount == 1)
                        litMessage.Text = @"<div style=""color: #aa0000;"">There is 1 client associated with this manager.</div>";
                    else
                        litMessage.Text = string.Format(@"<div style=""color: #aa0000;"">There are {0} clients associated with this manager.</div>", m.EmployeeCount);

                    litMatrix.Text = m.MatrixHtml.ToString();
                }
                else
                    litMatrix.Text = @"<div class=""nodata"">No clients have been assigned to this manager.</div>";
            }
        }

        protected void LoadManagers()
        {
            ClientModel client = CacheManager.Current.CurrentUser;

            IEnumerable<ClientOrg> allClientOrgs = null;

            if (client.HasPriv(ClientPrivilege.Administrator))
                allClientOrgs = ClientOrgUtility.AllActiveManagers();
            else if (client.HasPriv(ClientPrivilege.Executive))
                allClientOrgs = ClientOrgUtility.AllActiveManagers().Where(x=> x.Client.ClientID == client.ClientID).ToList();

            var dataSource = allClientOrgs.Select(x=>GetManagerItem(x, allClientOrgs)).OrderBy(x=> x.DisplayName).ToList();

            if (dataSource.Count > 1)
                dataSource.Insert(0, new {ClientOrgID = 0, DisplayName = "-- Select --"});

            ddlManager.DataSource = dataSource;
            ddlManager.DataBind();
            rdoAcctDisplayByName.Checked = true;
        }

        private dynamic GetManagerItem(ClientOrg co, IEnumerable<ClientOrg> allClientOrgs)
        {
            string displayName = co.Client.DisplayName;
            int count = allClientOrgs.Where(x => x.Client == co.Client).Count();

            if (count > 1)
                displayName += string.Format(" ({0})", co.Org.OrgName);

            var result = new
            {
                ClientOrgID = co.ClientOrgID,
                DisplayName = displayName
            };

            return result;
        }
    }
}