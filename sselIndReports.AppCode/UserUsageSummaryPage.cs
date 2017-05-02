using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Web.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace sselIndReports.AppCode
{
    public abstract class UserUsageSummaryPage : ReportPage
    {
        protected DataTable dtRoom;
        protected DataTable dtToolActivated;
        protected DataTable dtToolUncancelled;
        protected DataTable dtToolForgiven;
        protected DataTable dtStore;

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.Executive | ClientPrivilege.Administrator; }
        }

        protected abstract DateTime SelectedPeriod { get; set; }

        protected abstract void RunReport(DateTime period, int clientId);

        protected abstract Label SummaryApproximateLabel { get; }

        protected abstract DropDownList ClientDropDownList { get; }

        protected abstract Button RetrieveDataButton { get; }

        protected int SelectedClientID
        {
            get { return Convert.ToInt32(ClientDropDownList.SelectedValue); }
            set { ClientDropDownList.SelectedValue = value.ToString(); }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PopulateUserDropDownList(ClientDropDownList, SelectedPeriod, RetrieveDataButton);
                ShowEstimateMessage();
                RunReportOnLoad();
            }

            base.OnLoad(e);
        }

        private int GetClientIDFromQueryString()
        {
            int result = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["cid"]))
                int.TryParse(Request.QueryString["cid"], out result);
            return result;
        }

        private DateTime GetPeriodFromQueryString()
        {
            DateTime result = DateTime.MinValue;
            if (!string.IsNullOrEmpty(Request.QueryString["p"]))
                DateTime.TryParse(Request.QueryString["p"], out result);
            return result;
        }

        protected void RunReportOnLoad()
        {
            DateTime period = GetPeriodFromQueryString();
            int clientId = GetClientIDFromQueryString();
            if (clientId > 0 && period != DateTime.MinValue)
            {
                SelectedPeriod = period;
                SelectedClientID = clientId;
                RunReport(period, clientId);
            }
        }

        protected void UpdateRoomSums(DataTable dt, Label lbl)
        {
            //2009-12-11 Used to calculate differen room charges, because users want to see those differentiated
            object lnfSum, cleanRoomSum, wetChemSum, testLabSum;
            GetRoomSums(dt, out lnfSum, out cleanRoomSum, out wetChemSum, out testLabSum);
            UpdateSumLabel(lnfSum, lbl, "LNF");
            UpdateSumLabel(cleanRoomSum, lbl, "Clean Room");
            UpdateSumLabel(wetChemSum, lbl, "Wet Chemistry");
            UpdateSumLabel(testLabSum, lbl, "DC Lab");
        }

        private void GetRoomSums(DataTable dt, out object lnfSum, out object cleanRoomSum, out object wetChemSum, out object testLabSum)
        {
            lnfSum = dt.Compute("SUM(LineCost)", "RoomID = 154");
            cleanRoomSum = dt.Compute("SUM(LineCost)", "RoomID = 6");
            wetChemSum = dt.Compute("SUM(LineCost)", "RoomID = 25");
            testLabSum = dt.Compute("SUM(LineCost)", "RoomID = 2");
        }

        private void UpdateSumLabel(object sum, Label lbl, string roomName)
        {
            double temp;
            if (RepositoryUtility.TryConvertTo(sum, out temp, 0.0))
            {
                if (!string.IsNullOrEmpty(lbl.Text)) lbl.Text += " | ";
                lbl.Text += string.Format("{0}: {1:$#,##0.00}", roomName, temp);
                lbl.Visible = true;
            }
        }

        protected bool IsPrimaryOrg(int orgId)
        {
            return OrgUtility.GetPrimaryOrg().OrgID == orgId;
        }

        protected void ShowEstimateMessage()
        {
            if (SummaryApproximateLabel != null)
            {
                if (Utility.IsBeforeNextBusinessDay(DateTime.Now) && DateTime.Now.AddMonths(-1).Year == SelectedPeriod.Year && DateTime.Now.AddMonths(-1).Month == SelectedPeriod.Month)
                    SummaryApproximateLabel.Visible = true;
                else
                    SummaryApproximateLabel.Visible = false;
            }
        }

        /*protected void PopulateUserDropDownList()
        {
            ClientDropDownList.Enabled = true;
            RetrieveDataButton.Enabled = true;

            //used to store temporary selected user because we want to keep the same user when date has been changed
            string origSelectedValue = ClientDropDownList.SelectedValue;

            //populate the user dropdown list
            //int privs = AccessPrivs.GetPrivVal("Lab User") | AccessPrivs.GetPrivVal("Staff") | AccessPrivs.GetPrivVal("Remote User");
            DateTime sDate = SelectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            ActiveLogClientAccount[] allClientAccountsActiveInPeriod = DA.Current.Query<ActiveLogClientAccount>().Where(x => x.EnableDate < eDate && (x.DisableDate == null || x.DisableDate > sDate)).ToArray(); //ActiveLog.GetActive<ClientAccount>(x => x.ClientAccountID, sDate, eDate).ToArray();
            ClientItem[] filtered = FilterUsers(Client.Current, allClientAccountsActiveInPeriod); //checks for administrator, manager, or normal user
            ClientDropDownList.DataSource = filtered;
            ClientDropDownList.DataBind();

            if (filtered.Length == 0)
            {
                ClientDropDownList.Items.Insert(0, new ListItem("-- You were not active in this period --", "0"));
                ClientDropDownList.Enabled = false;
                RetrieveDataButton.Enabled = false;
            }
            else if (filtered.Length > 1)
            { 
                ClientDropDownList.Items.Insert(0, new ListItem("-- Select --", "0"));
            }

            //make sure the original user is still selected after the update of user dropdownlist
            if (!string.IsNullOrEmpty(origSelectedValue) && ClientDropDownList.Items.FindByValue(origSelectedValue) != null)
                ClientDropDownList.SelectedValue = origSelectedValue;
        }

        private ClientItem[] FilterUsers(Client client, ActiveLogClientAccount[] baseQuery)
        {
            List<ClientItem> dataSource = new List<ClientItem>();

            if (client.HasPriv(ClientPrivilege.Administrator))
            {
                //administrators see everyone
                dataSource.AddRange(baseQuery.Select(x => new ClientItem() { ClientID = x.ClientID, FName = x.FName, LName = x.LName}));
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
        }*/

        private Client[] FilterForExecutive(ClientAccount[] baseQuery)
        {
            // get all accounts of this manager Client.Current
            // get all users in each of these accounts
            ClientModel client = CacheManager.Current.CurrentUser;
            //if (client.HasPriv(ClientPrivilege.Executive))
            //return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId).ToArray();

            //var allClientAccountsActiveInPeriod = ActiveLog.GetActive<ClientAccount>(x => x.ClientAccountID, SelectedPeriod, SelectedPeriod.AddMonths(1)).ToArray();

            ClientAccount[] managerClientAccounts = baseQuery.Where(x => x.ClientOrg.Client.ClientID == client.ClientID).ToArray();

            List<Client> dataSource = new List<Client>();

            foreach (ClientAccount ca in managerClientAccounts)
            {
                //ClientOrg[] clientOrgs = DA.Current.Query<ClientOrg>().Where(x => x.Org == ca.ClientOrg.Org).ToArray();
                //dataSource.AddRange(clientOrgs.Select(x => x.Client));

                ClientAccount[] clientAccts = baseQuery.Where(x => x.Account == ca.Account).ToArray();
                dataSource.AddRange(clientAccts.Select(x => x.ClientOrg.Client));
            }

            //managerClientAccounts.SelectMany(x => DA.Current.Query<Client>().Where(y => y.ClientID == x.ClientOrg.Client.ClientID && y.ClientID != client.ClientID)).Distinct().ToArray();

            return dataSource.Distinct().ToArray();
        }
        private static DataTable GetAllClient(DateTime sDate, DateTime eDate, int privs)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "AllUniqueName");
                dba.AddParameter("@sDate", sDate);
                dba.AddParameter("@eDate", eDate);
                dba.AddParameter("@Privs", privs);
                DataTable dt = dba.FillDataTable("Client_Select");

                //Must set primary key because the client code need to find data in this table
                //should this code below to business logic or data access?
                dt.PrimaryKey = new DataColumn[] { dt.Columns["ClientID"] };

                return dt;
            }
        }

        protected void ReportButton_Click(object sender, EventArgs e)
        {
            RunReport(SelectedPeriod, SelectedClientID);
        }

        protected void pp1_SelectedPeriodChanged(object sender, PeriodChangedEventArgs e)
        {
            PopulateUserDropDownList(ClientDropDownList, SelectedPeriod, RetrieveDataButton);
            ShowEstimateMessage();
            OnSelectedPeriodChanged(e);
        }

        protected void SetTotalPerOrgCharge(DataTable dt, DataRow dr, string column)
        {
            object totalPerOrgCharge = DBNull.Value;

            if (dt.Rows.Count > 0)
                totalPerOrgCharge = dtRoom.Compute("SUM(LineCost)", string.Format("OrgID = {0}", dr["OrgID"]));

            if (totalPerOrgCharge != DBNull.Value)
                dr[column] = dr.Field<double>(column) + Convert.ToDouble(totalPerOrgCharge);
        }

        protected void PopulateReportInfo(HtmlControl div, int clientId, DateTime period)
        {
            Client client = DA.Current.Single<Client>(clientId);
            if (client != null)
            {
                ArrayList data = new ArrayList();
                data.Add(new { Label = "User", Value = client.DisplayName });
                data.Add(new { Label = "Period", Value = period.ToString("M/d/yyyy") });
                data.Add(new { Label = "Created", Value = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt") });
                Repeater rpt = (Repeater)div.FindControl("rptReportInfo");
                if (rpt != null)
                {
                    rpt.DataSource = data;
                    rpt.DataBind();
                    div.Visible = true;
                }
            }
        }

        protected virtual void OnSelectedPeriodChanged(PeriodChangedEventArgs e) { }
    }
}
