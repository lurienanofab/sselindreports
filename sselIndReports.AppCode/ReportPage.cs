﻿using LNF;
using LNF.Billing;
using LNF.Data;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scheduler;
using LNF.Web;
using StructureMap.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselIndReports.AppCode
{
    public abstract class ReportPage : LNF.Web.Content.LNFPage
    {
        [SetterProperty]
        public IOrgManager OrgManager { get; set; }

        [SetterProperty]
        public IBillingTypeManager BillingTypeManager { get; set; }

        [SetterProperty]
        public IClientManager ClientManager { get; set; }

        [SetterProperty]
        public IReservationManager ReservationManager { get; set; }

        protected IEnumerable<ClientItem> GetClientDataSource(DateTime period, bool displayAllUsersToStaff = false)
        {
            var allClientAccountsActiveInPeriod = GetClientSelectItems(period);
            return FilterUsers(CurrentUser, allClientAccountsActiveInPeriod, displayAllUsersToStaff); //checks for administrator, manager, or normal user
        }

        public ReportPage()
        {
            ServiceProvider.Current.BuildUp(this);
        }

        public virtual bool ShowButton
        {
            get { return true; }
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            ContextBase.Session.Remove("Updated");
            ContextBase.RemoveCacheData(); // remove anything left in cache
            Response.Redirect("~");
        }

        protected void FillYearSelect(DropDownList ddl, int startYear = 2003)
        {
            WebUtility.BindYearData(ddl, startYear);
        }

        protected UserClientSelectItem[] GetClientSelectItems(DateTime period)
        {
            // clear the session variable whenever the period changes
            if (Session["ClientSelectPeriod"] == null || Convert.ToDateTime(Session["ClientSelectPeriod"]) != period)
                Session.Remove("ClientSelectItems");

            Session["ClientSelectPeriod"] = period;

            if (Session["ClientSelectItems"] == null)
            {
                DateTime sd = period;
                DateTime ed = sd.AddMonths(1);

                Session["ClientSelectItems"] = DA.Current.Query<ActiveLogClientAccount>()
                    .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd))
                    .Select(x => new UserClientSelectItem()
                    {
                        ClientID = x.ClientID,
                        LName = x.LName,
                        FName = x.FName,
                        AccountID = x.AccountID,
                        Manager = x.Manager
                    })
                    .ToArray();
            }

            return (UserClientSelectItem[])Session["ClientSelectItems"];
        }

        protected void PopulateUserDropDownList(DropDownList ddl, DateTime period, Button btnRetrieveData, bool displayAllUsersToStaff = false)
        {
            ddl.Enabled = true;
            btnRetrieveData.Enabled = true;

            //used to store temporary selected user because we want to keep the same user when date has been changed
            string origSelectedValue = ddl.SelectedValue;

            //populate the user dropdown list
            var dataSource = GetClientDataSource(period, displayAllUsersToStaff);
            ddl.DataSource = dataSource;
            ddl.DataBind();

            if (dataSource.Count() == 0)
            {
                ddl.Items.Insert(0, new ListItem("-- You were not active in this period --", "0"));
                ddl.Enabled = false;
                btnRetrieveData.Enabled = false;
            }
            else if (dataSource.Count() > 1)
            {
                ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
            }

            //make sure the original user is still selected after the update of user dropdownlist
            if (!string.IsNullOrEmpty(origSelectedValue) && ddl.Items.FindByValue(origSelectedValue) != null)
                ddl.SelectedValue = origSelectedValue;
        }

        private static ClientItem[] FilterUsers(IClient client, UserClientSelectItem[] items, bool displayAllUsersToStaff)
        {
            List<ClientItem> dataSource = new List<ClientItem>();
            bool isUserStaffAndAllowedAllUserInfo = client.HasPriv(ClientPrivilege.Staff) && displayAllUsersToStaff;

            if (client.HasPriv(ClientPrivilege.Administrator) || isUserStaffAndAllowedAllUserInfo)
            {
                //administrators see everyone
                dataSource.AddRange(items.Select(x => new ClientItem() { ClientID = x.ClientID, FName = x.FName, LName = x.LName }));
            }
            else
            {
                //not an admin - get all of this user's ClientAccounts
                var clientAccts = items.Where(x => x.ClientID == client.ClientID).ToArray();

                foreach (var ca in clientAccts)
                {
                    //check to see if they are a manager
                    if (ca.Manager)
                    {
                        var otherClientAccts = items.Where(x => x.AccountID == ca.AccountID);
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

    public class UserClientSelectItem
    {
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public int AccountID { get; set; }
        public bool Manager { get; set; }
        public string DisplayName => LNF.Models.Data.ClientItem.GetDisplayName(LName, FName);
    }
}
