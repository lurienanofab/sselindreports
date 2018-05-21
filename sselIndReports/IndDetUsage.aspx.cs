using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Web.Controls;
using sselIndReports.AppCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndDetUsage : ReportPage
    {
        //private DataSet dsUsage;
        //private DataTable dtActivity; // inner table - contians date/time and activity

        private const string EVENT_NO_ANTIPASSBACK = "Local Grant";
        private const string EVENT_ANTIPASSBACK_IN = "Local Grant - IN";
        private const string EVENT_ANTIPASSBACK_OUT = "Local Grant - OUT";

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.Executive | ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //2007-02-1 add the button back, disable all postback of dropdown list
                if (CurrentUser.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Staff | ClientPrivilege.Executive))
                {
                    DateTime sDate = pp1.SelectedPeriod;
                    DateTime eDate = sDate.AddMonths(1);

                    using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                    {
                        dba.AddParameter("@sDate", sDate);
                        dba.AddParameter("@eDate", eDate);
                        dba.AddParameter("@ClientID", CacheManager.Current.ClientID);

                        //Depending on the user prives, we need to retrieve different set of data
                        if (CurrentUser.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Staff))
                        {
                            dba.AddParameter("@Privs", (int)(ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.StoreUser));
                            dba.AddParameter("@Action", "All");
                        }
                        else if (CurrentUser.HasPriv(ClientPrivilege.Executive))
                        {
                            //for executive-only person, we only show the people he/she manages
                            dba.AddParameter("@Action", "ByMgr");
                        }

                        using (var reader = dba.ExecuteReader("Client_Select"))
                        {
                            ddlUser.DataSource = reader;
                            ddlUser.DataTextField = "DisplayName";
                            ddlUser.DataValueField = "ClientID";
                            ddlUser.DataBind();
                        }
                    }
                }

                SetCurrentUser(CurrentUser.ClientID.ToString());

                DisplayUsage();
            }
        }

        private ActivityDate[] GetActivityData(int clientId, DateTime startDate, DateTime endDate)
        {
            List<ActivityDate> result = new List<ActivityDate>();

            string desc = string.Empty;

            //first fill the table with room events
            IList<RoomDataImport> roomData;

            if (startDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                //get this month data from RoomDataImport
                roomData = DA.Current.Query<RoomDataImport>().Where(x => x.ClientID == clientId && x.EventDate >= startDate && x.EventDate < endDate).ToList();
            else
            {
                //get prior month data from RoomDataClean but make it look like RoomDataImport
                roomData = new List<RoomDataImport>();
                var cleanData = DA.Current.Query<RoomDataClean>().Where(x => (x.EntryDT >= startDate && x.EntryDT < endDate) && x.Client.ClientID == clientId).ToList();
                foreach (var item in cleanData)
                {
                    if (item.Room.PassbackRoom)
                    {
                        RoomDataImport entry = new RoomDataImport
                        {
                            ClientID = item.Client.ClientID,
                            RoomName = item.Room.RoomName,
                            EventDate = item.EntryDT,
                            EventDescription = EVENT_ANTIPASSBACK_IN
                        };

                        roomData.Add(entry);

                        RoomDataImport exit = new RoomDataImport
                        {
                            ClientID = item.Client.ClientID,
                            RoomName = item.Room.RoomName,
                            EventDate = item.ExitDT.GetValueOrDefault(),
                            EventDescription = EVENT_ANTIPASSBACK_OUT
                        };

                        roomData.Add(exit);
                    }
                    else
                    {
                        RoomDataImport entry = new RoomDataImport
                        {
                            ClientID = item.Client.ClientID,
                            RoomName = item.Room.RoomName,
                            EventDate = item.EntryDT,
                            EventDescription = EVENT_NO_ANTIPASSBACK
                        };

                        roomData.Add(entry);
                    }
                }
            }

            foreach (var item in roomData)
            {
                Room r = DA.Current.Query<Room>().FirstOrDefault(x => x.RoomName == item.RoomName && x.Active && x.Billable);

                if (r != null)
                {
                    if (item.EventDescription == EVENT_ANTIPASSBACK_IN)
                        desc = "Entered the " + item.RoomName;
                    else if (item.EventDescription == EVENT_ANTIPASSBACK_OUT)
                        desc = "Exited the " + item.RoomName;
                    else if (item.EventDescription == EVENT_NO_ANTIPASSBACK)
                        desc = "Opened the " + item.RoomName + " door";

                    if (result.FirstOrDefault(x => x.Date == item.EventDate.Date) == null)
                        result.Add(new ActivityDate() { Date = item.EventDate.Date, Items = new List<ActivityItem>() });

                    result.First(x => x.Date == item.EventDate.Date).Items.Add(new ActivityItem()
                    {
                        ActivityDateTime = item.EventDate,
                        Description = desc
                    });
                }
            }

            var toolData = ReservationManager.SelectByDateRange(startDate, endDate, clientId);
            var filteredToolData = ReservationManager.FilterCancelledReservations(toolData, false);

            DateTime startTime;
            double duration;
            string reservationUsage = string.Empty;

            foreach (var item in filteredToolData)
            {
                if (item.Reservation.IsStarted)
                {
                    startTime = item.Reservation.ActualBeginDateTime.Value;
                    duration = item.Reservation.ActualDuration();
                    reservationUsage = "Used the ";
                }
                else
                {
                    startTime = item.Reservation.BeginDateTime;
                    duration = item.Reservation.ReservedDuration();
                    reservationUsage = "Reserved (but did not use) the ";
                }

                string account = item.Reservation.Account.Name;
                string resource = item.Reservation.Resource.ResourceName;

                if (startTime < DateTime.Now)
                    desc = reservationUsage + resource + " for " + (duration / 60.0).ToString("#.00") + " hours on account " + account;
                else
                    desc = "Reserved the " + resource + " for " + (duration / 60.0).ToString("#.00") + " hours on account " + account;

                if (result.FirstOrDefault(x => x.Date == startTime.Date) == null)
                    result.Add(new ActivityDate() { Date = startTime.Date, Items = new List<ActivityItem>() });

                result.First(x => x.Date == startTime.Date).Items.Add(new ActivityItem()
                {
                    ActivityDateTime = startTime,
                    Description = desc
                });
            }

            return result.ToArray();
        }

        private void DisplayUsage()
        {
            litNoData.Text = "";

            int clientId = Convert.ToInt32(ddlUser.SelectedValue);
            DateTime sDate = pp1.SelectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            litMessage.Text = GetDisplayName(clientId);
            var data = GetActivityData(clientId, sDate, eDate);
            if (data.Count() > 0)
            {
                dgActDate.DataSource = data.OrderBy(x => x.Date);
                dgActDate.DataBind();
                dgActDate.Visible = true;
            }
            else
            {
                dgActDate.Visible = false;
                litNoData.Text = "<div class=\"nodata\">No activity found</div>";
            }
        }

        private string GetDisplayName(int clientId)
        {
            string name = DA.Current.Query<Client>().First(x => x.ClientID == clientId).DisplayName;
            return string.Format("<div class=\"display-name\">{0}</div>", name);
        }

        protected void dgActDate_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DataGrid dgActivity = (DataGrid)e.Item.FindControl("dgActivity");
                ActivityDate actDate = (ActivityDate)e.Item.DataItem;

                dgActivity.DataSource = actDate.Items.OrderBy(x => x.ActivityDateTime);
                dgActivity.DataBind();
            }
        }

        protected void ReportButton_Click(object sender, EventArgs e)
        {
            DisplayUsage();
        }

        protected void pp1_SelectedPeriodChanged(object sender, PeriodChangedEventArgs e)
        {
            PopulateUserDropDownList(ddlUser, pp1.SelectedPeriod, btnReport);
        }

        private void SetCurrentUser(string selectedValue)
        {
            //2007-2-02 Two possible scenario 
            //- if the current user is user himself, we have to add the empty ddlUser with only one entry
            //else, it's a good habit to include the user him/herself as well, since the above ddlUser data code return only a group of people
            //but sometimes the user him/herself is not belong to this group of people

            //remove first so we don't show the same user twice in the list
            var item = ddlUser.Items.FindByValue(CacheManager.Current.ClientID.ToString());
            if (item != null) ddlUser.Items.Remove(item);

            ddlUser.Items.Insert(0, new ListItem(CacheManager.Current.CurrentUser.DisplayName, CacheManager.Current.ClientID.ToString()));

            if (ddlUser.Items.FindByValue(selectedValue) != null)
                ddlUser.SelectedValue = selectedValue;
        }

        /*protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayUsage();
        }*/
    }

    public class ActivityDate
    {
        public DateTime Date { get; set; }
        public List<ActivityItem> Items { get; set; }
    }

    public class ActivityItem
    {
        public DateTime ActivityDateTime { get; set; }

        public DateTime ActivityDate
        {
            get { return ActivityDateTime.Date; }
        }

        public string ActivityTime
        {
            get { return ActivityDateTime.ToString("HH:mm"); }
        }

        public string Description { get; set; }
    }
}