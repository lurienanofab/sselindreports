using LNF;
using LNF.Billing;
using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class ResourceDetail : System.Web.UI.Page
    {
        private readonly IDictionary<string, double> _totals = new Dictionary<string, double>();

        protected IBillingTypeManager BillingTypeManager => ServiceProvider.Current.BillingTypeManager;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var period = GetRequiredParamAsDateTime("Period");
                var resourceId = GetRequiredParamAsInt32("ResourceID");
                var clientId = GetRequiredParamAsInt32("ClientID");
                var accountId = GetRequiredParamAsInt32("AccountID");

                ResourceItem res = ServiceProvider.Current.Scheduler.GetResource(resourceId);
                litHeaderResource.Text = res.ToString();

                IAccount acct = ServiceProvider.Current.Data.GetAccount(accountId);
                litHeaderAccount.Text = acct.ToString();

                var data = GetData(resourceId, clientId, accountId, period);

                rptResourceDetail.DataSource = data;
                rptResourceDetail.DataBind();
            }
        }

        private IEnumerable<ResourceDetailItem> GetData(int resourceId, int clientId, int accountId, DateTime period)
        {
            IQueryable<IToolBilling> query;

            if (period == DateTime.Now.FirstOfMonth())
                query = DA.Current.Query<ToolBillingTemp>();
            else
                query = DA.Current.Query<ToolBilling>();

            var join = query
                .Where(x => x.Period == period && x.ClientID == clientId && x.ResourceID == resourceId && x.AccountID == accountId)
                .Join(DA.Current.Query<Account>(), o => o.AccountID, i => i.AccountID, (o, i) => new { ToolBilling = o, Account = i })
                .ToList();

            var result = join
                .GroupBy(x => new { x.ToolBilling.ReservationID, x.ToolBilling.IsStarted, x.ToolBilling.IsActive, x.ToolBilling.IsCancelledBeforeAllowedTime, x.ToolBilling.ResourceRate })
                .Select(x => new ResourceDetailItem
                {
                    ReservationID = x.Key.ReservationID,
                    ActDate = x.Min(g => g.ToolBilling.ActDate),
                    Started = x.Key.IsStarted ? "Yes" : "No",
                    Cancelled = x.Key.IsActive ? "No" : "Yes",
                    CancelledBeforeCutoff = x.Key.IsCancelledBeforeAllowedTime ? "Yes" : "No",
                    ActivatedUsed = x.Sum(g => g.ToolBilling.ActivatedUsed().TotalHours),
                    ActivatedUnused = x.Sum(g => g.ToolBilling.ActivatedUnused().TotalHours),
                    Overtime = Convert.ToDouble(x.Sum(g => g.ToolBilling.OverTime / 60M)),
                    OvertimeFee = x.Sum(g => g.ToolBilling.OverTimePenaltyFee),
                    UnstartedUnused = x.Sum(g => g.ToolBilling.UnstartedUnused().TotalHours),
                    BookingFee = x.Sum(g => g.ToolBilling.BookingFee),
                    Transferred = Convert.ToDouble(x.Sum(g => g.ToolBilling.TransferredDuration / 60M)),
                    Forgiven = Convert.ToDouble(x.Sum(g => g.ToolBilling.ForgivenDuration / 60M)),
                    ResourceRate = x.Key.ResourceRate,
                    LineTotal = x.Sum(g => BillingTypeManager.GetLineCost(g.ToolBilling)),
                })
                .OrderBy(x => x.ActDate)
                .ThenBy(x => x.ReservationID)
                .ToList();

            _totals.Add("ActivatedUsed", result.Sum(x => x.ActivatedUsed));
            _totals.Add("ActivatedUnused", result.Sum(x => x.ActivatedUnused));
            _totals.Add("Overtime", result.Sum(x => x.Overtime));
            _totals.Add("OvertimeFee", result.Sum(x => Convert.ToDouble(x.OvertimeFee)));
            _totals.Add("UnstartedUnused", result.Sum(x => x.UnstartedUnused));
            _totals.Add("BookingFee", result.Sum(x => Convert.ToDouble(x.BookingFee)));
            _totals.Add("Transferred", result.Sum(x => x.Transferred));
            _totals.Add("Forgiven", result.Sum(x => x.Forgiven));
            _totals.Add("LineTotal", result.Sum(x => Convert.ToDouble(x.LineTotal)));

            return result;
        }

        private string GetRequiredParamAsString(string key)
        {
            if (string.IsNullOrEmpty(Request.QueryString[key]))
                throw new Exception($"Missing required QueryString parameter: {key}");

            return Request.QueryString[key];
        }

        private int GetRequiredParamAsInt32(string key)
        {
            var value = GetRequiredParamAsString(key);

            if (!int.TryParse(value, out int result))
                throw new Exception($"Invalid Int32 value for QueryString parameter: {key}");

            return result;
        }

        private DateTime GetRequiredParamAsDateTime(string key)
        {
            var value = GetRequiredParamAsString(key);

            if (!DateTime.TryParse(value, out DateTime result))
                throw new Exception($"Invalid DateTime value for QueryString parameter: {key}");

            return result;
        }

        protected void RptResourceDetail_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                foreach (var kvp in _totals)
                {
                    var lit = (Literal)e.Item.FindControl($"litTotal{kvp.Key}");
                    lit.Text = kvp.Value.ToString("0.00");
                }
            }
        }
    }

    public class ResourceDetailItem
    {
        public int ReservationID { get; set; }
        public DateTime ActDate { get; set; }
        public string Started { get; set; }
        public string Cancelled { get; set; }
        public string CancelledBeforeCutoff { get; set; }
        public double ActivatedUsed { get; set; }
        public double ActivatedUnused { get; set; }
        public double Overtime { get; set; }
        public decimal OvertimeFee { get; set; }
        public double UnstartedUnused { get; set; }
        public decimal BookingFee { get; set; }
        public double Transferred { get; set; }
        public double Forgiven { get; set; }
        public decimal ResourceRate { get; set; }
        public decimal LineTotal { get; set; }

        public override string ToString() => ReservationID.ToString();
    }
}