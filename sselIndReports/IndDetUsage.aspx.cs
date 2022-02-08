using LNF.Data;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
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

                    var command = DataCommand()
                        .Param("sDate", sDate)
                        .Param("eDate", eDate)
                        .Param("ClientID", CurrentUser.ClientID);

                    //Depending on the user prives, we need to retrieve different set of data
                    if (CurrentUser.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Staff))
                    {
                        command
                            .Param("Privs", (int)(ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.StoreUser))
                            .Param("Action", "All");
                    }
                    else if (CurrentUser.HasPriv(ClientPrivilege.Executive))
                    {
                        //for executive-only person, we only show the people he/she manages
                        command.Param("Action", "ByMgr");
                    }

                    using (var reader = command.ExecuteReader("dbo.Client_Select"))
                    {
                        ddlUser.DataSource = reader;
                        ddlUser.DataTextField = "DisplayName";
                        ddlUser.DataValueField = "ClientID";
                        ddlUser.DataBind();
                    }
                }

                SetCurrentUser(CurrentUser.ClientID.ToString());

                DisplayUsage();
            }
        }

        private IEnumerable<ActivityDate> GetActivityDates(int clientId, DateTime sd, DateTime ed, IDictionary<string, double> summary)
        {
            List<ActivityDate> result = new List<ActivityDate>();

            string desc = string.Empty;

            // RoomDataClean contains prior months and same month data

            var roomEvents = new List<RoomEvent>();
            var cleanData = DataSession.Query<RoomDataClean>().Where(x => (x.EntryDT >= sd && x.EntryDT < ed) && x.ClientID == clientId).ToList();

            var rooms = DataSession.Query<Room>().ToList();

            foreach (var item in cleanData)
            {
                var room = rooms.First(x => x.RoomID == item.RoomID);

                if (room.PassbackRoom)
                {
                    // add entry
                    roomEvents.Add(new RoomEvent
                    {
                        ClientID = item.ClientID,
                        RoomDisplayName = room.DisplayName,
                        RoomBillable = room.Billable,
                        RoomActive = room.Active,
                        EventDate = item.EntryDT,
                        EventDescription = EVENT_ANTIPASSBACK_IN
                    });

                    if (!item.ExitDT.HasValue)
                        throw new Exception($"No ExitDT for RoomDataID = {item.RoomDataID} in RoomDataClean");

                    // add exit
                    roomEvents.Add(new RoomEvent
                    {
                        ClientID = item.ClientID,
                        RoomDisplayName = room.DisplayName,
                        RoomBillable = room.Billable,
                        RoomActive = room.Active,
                        EventDate = item.ExitDT.Value,
                        EventDescription = EVENT_ANTIPASSBACK_OUT
                    });

                    if (!summary.ContainsKey(room.DisplayName))
                        summary.Add(room.DisplayName, 0);

                    summary[room.DisplayName] += (item.ExitDT.Value - item.EntryDT).TotalHours;
                }
                else
                {
                    // add entry only, because not antipassback room
                    roomEvents.Add(new RoomEvent
                    {
                        ClientID = item.ClientID,
                        RoomDisplayName = room.RoomName,
                        RoomBillable = room.Billable,
                        RoomActive = room.Active,
                        EventDate = item.EntryDT,
                        EventDescription = EVENT_NO_ANTIPASSBACK
                    });
                }
            }

            foreach (var item in roomEvents)
            {
                if (item.RoomActive && item.RoomBillable)
                {
                    if (item.EventDescription == EVENT_ANTIPASSBACK_IN)
                        desc = $"Entered {item.RoomDisplayName}";
                    else if (item.EventDescription == EVENT_ANTIPASSBACK_OUT)
                        desc = $"Exited {item.RoomDisplayName}";
                    else if (item.EventDescription == EVENT_NO_ANTIPASSBACK)
                        desc = $"Opened {item.RoomDisplayName} door";

                    if (result.FirstOrDefault(x => x.Date == item.EventDate.Date) == null)
                        result.Add(new ActivityDate() { Date = item.EventDate.Date, Items = new List<ActivityItem>() });

                    result.First(x => x.Date == item.EventDate.Date).Items.Add(new ActivityItem
                    {
                        ActivityDateTime = item.EventDate,
                        Description = desc
                    });
                }
            }

            var toolData = Provider.Scheduler.Reservation.SelectByClient(clientId, sd, ed, true);
            var filteredToolData = Provider.Scheduler.Reservation.FilterCancelledReservations(toolData, false);

            DateTime startTime;
            double duration;
            string reservationUsage = string.Empty;

            foreach (var item in filteredToolData)
            {
                if (item.Reservation.IsStarted)
                {
                    startTime = item.Reservation.ActualBeginDateTime.Value;
                    duration = item.Reservation.GetActualDuration().TotalMinutes;
                    reservationUsage = "Used ";
                }
                else
                {
                    startTime = item.Reservation.BeginDateTime;
                    duration = item.Reservation.GetReservedDuration().TotalMinutes;
                    reservationUsage = "Reserved (but did not use) ";
                }

                string account = item.Reservation.AccountName;
                string resource = ((LNF.Scheduler.IReservationItem)item.Reservation).ResourceName;

                double hours = duration / 60D;

                if (startTime < DateTime.Now)
                    desc = reservationUsage + resource + $" for {hours:0.00} hours on account {account}";
                else
                    desc = $"Reserved {resource} for {hours:0.00} hours on account {account}";

                if (result.FirstOrDefault(x => x.Date == startTime.Date) == null)
                    result.Add(new ActivityDate() { Date = startTime.Date, Items = new List<ActivityItem>() });

                result.First(x => x.Date == startTime.Date).Items.Add(new ActivityItem()
                {
                    ActivityDateTime = startTime,
                    Description = desc
                });
            }

            return result;
        }

        private void DisplayUsage()
        {
            litNoData.Text = string.Empty;
            litSummaryNoData.Text = string.Empty;

            int clientId = Convert.ToInt32(ddlUser.SelectedValue);
            DateTime sd = pp1.SelectedPeriod;
            DateTime ed = sd.AddMonths(1);

            litMessage.Text = GetDisplayName(clientId);

            var summary = new Dictionary<string, double>();
            var data = GetActivityDates(clientId, sd, ed, summary);

            phAntipassbackSummary.Visible = false;
            rptAntipassbackSummary.Visible = false;

            if (data.Count() > 0)
            {
                dgActDate.DataSource = data.OrderBy(x => x.Date);
                dgActDate.DataBind();
                dgActDate.Visible = true;

                if (summary.Count > 0)
                {
                    rptAntipassbackSummary.DataSource = summary.OrderBy(x => x.Key);
                    rptAntipassbackSummary.DataBind();
                    rptAntipassbackSummary.Visible = true;
                }
                else
                {
                    rptAntipassbackSummary.Visible = false;
                    litSummaryNoData.Text = "<div class=\"nodata\">No antipassback rooms found.</div>";
                }

                phAntipassbackSummary.Visible = true;
            }
            else
            {
                dgActDate.Visible = false;
                rptAntipassbackSummary.Visible = false;
                litNoData.Text = "<div class=\"nodata\">No activity found.</div>";
            }
        }

        private string GetDisplayName(int clientId)
        {
            string name = DataSession.Query<Client>().First(x => x.ClientID == clientId).DisplayName;
            return string.Format("<div class=\"display-name\">{0}</div>", name);
        }

        protected void DgActDate_ItemDataBound(object sender, DataGridItemEventArgs e)
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

        protected void Pp1_SelectedPeriodChanged(object sender, PeriodChangedEventArgs e)
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
            var item = ddlUser.Items.FindByValue(CurrentUser.ClientID.ToString());
            if (item != null) ddlUser.Items.Remove(item);

            ddlUser.Items.Insert(0, new ListItem(CurrentUser.DisplayName, CurrentUser.ClientID.ToString()));

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

    public class RoomEvent
    {
        public int ClientID { get; set; }
        public string RoomDisplayName { get; set; }
        public bool RoomBillable { get; set; }
        public bool RoomActive { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
    }
}