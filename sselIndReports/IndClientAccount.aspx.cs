using LNF.CommonTools;
using LNF.Data;
using LNF.Data.ClientAccountMatrix;
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
            get { return ClientPrivilege.Administrator | ClientPrivilege.Executive; }
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

            var managerCount = ddlManager.Items.Count;

            if (managerCount > 0)
            {
                int managerOrgId = GetManagerOrgID();

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

                phCriteria.Visible = true;
            }
            else
            {
                litMatrix.Text = $@"<div class=""nodata"">You are currently not a manager in the {Provider.Data.GlobalSetting.GetGlobalSetting("CompanyName")} online system.</div>";
                phCriteria.Visible = false;
            }
        }

        private int GetManagerOrgID()
        {
            if (int.TryParse(ddlManager.SelectedValue, out int result))
                return result;

            return 0;
        }

        protected void LoadManagers()
        {
            var client = CurrentUser;

            IEnumerable<IClient> allClientOrgs = null;

            if (client.HasPriv(ClientPrivilege.Administrator))
                allClientOrgs = Provider.Data.Client.GetActiveManagers(true);
            else if (client.HasPriv(ClientPrivilege.Executive))
                allClientOrgs = Provider.Data.Client.GetActiveManagers(true).Where(x => x.ClientID == client.ClientID).ToList();

            var list = new List<ManagerItem>();

            foreach (var co in allClientOrgs)
            {
                var item = GetManagerItem(co, allClientOrgs);
                list.Add(item);
            }

            var dataSource = list.OrderBy(x => x.DisplayName).ToList();

            if (dataSource.Count > 1)
                dataSource.Insert(0, new ManagerItem { ClientOrgID = 0, DisplayName = "-- Select --" });

            ddlManager.DataSource = dataSource;
            ddlManager.DataBind();
            rdoAcctDisplayByName.Checked = true;
        }

        private ManagerItem GetManagerItem(IClient co, IEnumerable<IClient> allClientOrgs)
        {
            string displayName = co.DisplayName;
            int count = allClientOrgs.Where(x => x.ClientID == co.ClientID).Count();

            if (count > 1)
                displayName += string.Format(" ({0})", co.OrgName);

            var result = new ManagerItem
            {
                ClientOrgID = co.ClientOrgID,
                DisplayName = displayName
            };

            return result;
        }
    }

    public class ManagerItem
    {
        public int ClientOrgID { get; set; }
        public string DisplayName { get; set; }
    }
}