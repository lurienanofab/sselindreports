using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselIndReports.AppCode
{
    public abstract class ReportPage : LNF.Web.Content.LNFPage
    {
        public virtual bool ShowButton
        {
            get { return true; }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            CacheManager.Current.RemoveSessionValue("Updated");
            CacheManager.Current.RemoveCacheData(); // remove anything left in cache
            Response.Redirect("~");
        }

        protected void FillYearSelect(DropDownList ddl, int startYear = 2003)
        {
            WebUtility.BindYearData(ddl, startYear);
        }

        protected static void PopulateUserDropDownList(DropDownList clientDDList, DateTime selectedPeriod, Button btnRetrieveData, bool displayAllUsersToStaff = false)
        {
            clientDDList.Enabled = true;
            btnRetrieveData.Enabled = true;

            //used to store temporary selected user because we want to keep the same user when date has been changed
            string origSelectedValue = clientDDList.SelectedValue;

            //populate the user dropdown list
            //int privs = AccessPrivs.GetPrivVal("Lab User") | AccessPrivs.GetPrivVal("Staff") | AccessPrivs.GetPrivVal("Remote User");
            DateTime sDate = selectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            ActiveLogClientAccount[] allClientAccountsActiveInPeriod = DA.Current.Query<ActiveLogClientAccount>().Where(x => x.EnableDate < eDate && (x.DisableDate == null || x.DisableDate > sDate)).ToArray();
            //ActiveLog.GetActive<ClientAccount>(x => x.ClientAccountID, sDate, eDate).ToArray();
            ClientItem[] filtered = FilterUsers(CacheManager.Current.CurrentUser, allClientAccountsActiveInPeriod, displayAllUsersToStaff); //checks for administrator, manager, or normal user
            clientDDList.DataSource = filtered;
            clientDDList.DataBind();

            if (filtered.Length == 0)
            {
                clientDDList.Items.Insert(0, new ListItem("-- You were not active in this period --", "0"));
                clientDDList.Enabled = false;
                btnRetrieveData.Enabled = false;
            }
            else if (filtered.Length > 1)
            {
                clientDDList.Items.Insert(0, new ListItem("-- Select --", "0"));
            }

            //make sure the original user is still selected after the update of user dropdownlist
            if (!string.IsNullOrEmpty(origSelectedValue) && clientDDList.Items.FindByValue(origSelectedValue) != null)
                clientDDList.SelectedValue = origSelectedValue;
        }

        private static ClientItem[] FilterUsers(ClientModel client, ActiveLogClientAccount[] baseQuery, bool displayAllUsersToStaff)
        {
            List<ClientItem> dataSource = new List<ClientItem>();
            bool isUserStaffAndAllowedAllUserInfo = client.HasPriv(ClientPrivilege.Staff) && displayAllUsersToStaff;

            if (client.HasPriv(ClientPrivilege.Administrator) || isUserStaffAndAllowedAllUserInfo)
            {
                //administrators see everyone
                dataSource.AddRange(baseQuery.Select(x => new ClientItem() { ClientID = x.ClientID, FName = x.FName, LName = x.LName }));
            }
            else
            {
                //not an admin - get all of this user's ClientAccounts
                var clientAccts = baseQuery.Where(x => x.ClientID == client.ClientID).ToArray();

                foreach (ActiveLogClientAccount ca in clientAccts)
                {
                    //check to see if they are a manager
                    if (ca.Manager)
                    {
                        var otherClientAccts = baseQuery.Where(x => x.AccountID == ca.AccountID);
                        dataSource.AddRange(otherClientAccts.Select(x => new ClientItem() { ClientID = x.ClientID, FName = x.FName, LName = x.LName }).ToArray());
                    }
                    else
                    {
                        dataSource.Add(new ClientItem() { ClientID = client.ClientID, FName = client.FName, LName = client.LName });
                    }
                }
            }

            return dataSource.Distinct(new ClientItemEqualityComparer()).OrderBy(x => x.DisplayName).ToArray();
        }
    }
}
