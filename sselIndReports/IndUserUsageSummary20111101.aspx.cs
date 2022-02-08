using LNF.Billing.Reports;
using LNF.Cache;
using LNF.Data;
using LNF.Impl.Repository.Data;
using LNF.Web.Controls;
using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndUserUsageSummary20111101 : UserUsageSummaryPage
    {
        private GlobalSettings _ShowDisclaimerSetting;

        protected override DateTime SelectedPeriod
        {
            get { return pp1.SelectedPeriod; }
            set { pp1.SelectedPeriod = value; }
        }

        protected override DropDownList ClientDropDownList
        {
            get { return ddlUser; }
        }

        protected override Button RetrieveDataButton
        {
            get { return btnReport; }
        }

        protected override Label SummaryApproximateLabel
        {
            get { return lblSummaryApproximate; }
        }

        protected override void OnLoad(EventArgs e)
        {
            hidAjaxUrl.Value = VirtualPathUtility.ToAbsolute("~/ajax/index.ashx");

            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                ShowDisclaimer();
                Session.Remove("UserUsageSummaryTables");
                Session.Remove("UserUsageSummaryTables20110701");
            }
            else
            {
                lblGlobalMsg.Text = string.Empty;
            }
        }

        public GlobalSettings ShowDisclaimerSetting
        {
            get
            {
                if (_ShowDisclaimerSetting == null)
                {
                    _ShowDisclaimerSetting = DataSession.Query<GlobalSettings>().FirstOrDefault(x => x.SettingName == "ShowUserUsageSummaryDisclaimer");

                    if (_ShowDisclaimerSetting == null)
                    {
                        _ShowDisclaimerSetting = new GlobalSettings() { SettingName = "ShowUserUsageSummaryDisclaimer", SettingValue = "false" };
                        DataSession.SaveOrUpdate(_ShowDisclaimerSetting);
                    }
                }

                return _ShowDisclaimerSetting;
            }
        }

        private bool GetShowDisclaimerValue()
        {
            return bool.Parse(ShowDisclaimerSetting.SettingValue);
        }

        private void SetShowDisclaimerSetting(bool value)
        {
            ShowDisclaimerSetting.SettingValue = value ? "true" : "false";
            DataSession.SaveOrUpdate(ShowDisclaimerSetting);
        }

        private bool IsPreviousPeriod(DateTime value)
        {
            DateTime today = DateTime.Now.Date;
            DateTime prevPeriod = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1);
            return prevPeriod == value;
        }

        private void ShowDisclaimer()
        {
            bool show = GetShowDisclaimerValue();

            if (CurrentUser.HasPriv(ClientPrivilege.Developer))
            {
                panDisclaimerConfig.Visible = true;
                chkShowDisclaimer.Checked = show;
            }

            string text = ConfigurationManager.AppSettings["DisclaimerText"];
            text = text.Replace("{CurrentMonth}", DateTime.Now.Date.ToString("MMMM"));
            text = text.Replace("{PreviousMonth}", DateTime.Now.Date.AddMonths(-1).ToString("MMMM"));
            bool visible = show && IsPreviousPeriod(SelectedPeriod);
            panDisclaimer.Visible = visible;
            litDisclaimerText.Text = string.Empty;
            litSummaryDisclaimerText.Text = string.Empty;
            if (visible)
            {
                litDisclaimerText.Text = text;
                litSummaryDisclaimerText.Text = string.Format("<div style=\"padding: 10px; font-weight: bold; color: #ff0000;\">{0}</div>", text);
            }
        }

        private void PopulateRoomDetailData(DateTime period, int clientId)
        {
            // gets either current or prior period data
            DataTable dtRoom = RoomBillingBL.GetRoomBillingDataByClientID(ContextBase, period, clientId);

            if (!dtRoom.Columns.Contains("IsParent"))
                dtRoom.Columns.Add("IsParent", typeof(bool));

            if (!dtRoom.Columns.Contains("ParentID"))
                dtRoom.Columns.Add("ParentID", typeof(int));

            if (!dtRoom.Columns.Contains("RowCssClass"))
                dtRoom.Columns.Add("RowCssClass", typeof(string));

            var rooms = CacheManager.Current.Rooms();

            foreach (DataRow dr in dtRoom.Rows)
            {
                var r = rooms.FirstOrDefault(x => x.RoomID == dr.Field<int>("RoomID"));

                if (r.ParentID.HasValue)
                {
                    dr.SetField("IsParent", false);
                    dr.SetField("ParentID", r.ParentID.Value);
                    dr.SetField("RowCssClass", "child");
                }
                else
                {
                    dr.SetField("IsParent", true);
                    dr.SetField("ParentID", r.RoomID);
                    dr.SetField("RowCssClass", "parent");
                }
            }

            dtRoom.DefaultView.Sort = "ParentID ASC, IsParent DESC, Room ASC";

            rptRoomDetail.DataSource = dtRoom.DefaultView;
            rptRoomDetail.DataBind();

            decimal totalCleanRoomHours = 0, totalWetChemHours = 0, totalTestLabHours = 0, totalOrganicsHours = 0, totalLnfHours = 0;

            foreach (DataRow dr in dtRoom.Rows)
            {
                LabRoom room = Rooms.GetRoom(dr.Field<int>("RoomID"));

                // Using Convert.ToDecimal because value might be decimal or double depending no if it is from RoomBilling or RoomBillingTemp
                decimal hours = Convert.ToDecimal(dr["Hours"]);

                if (room == LabRoom.CleanRoom)
                    totalCleanRoomHours += hours;
                else if (room == LabRoom.ChemRoom)
                    totalWetChemHours += hours;
                else if (room == LabRoom.TestLab)
                    totalTestLabHours += hours;
                else if (room == LabRoom.OrganicsBay)
                    totalOrganicsHours += hours;
                else if (room == LabRoom.LNF)
                    totalLnfHours += hours;
            }

            string lnfName = "LNF";
            string cleanRoomName = "Clean Room";
            string wetChemName = "ROBIN";

            lblRoomHours.Text = $"|  {lnfName}: {totalLnfHours:#0.00} hours, {cleanRoomName}: {totalCleanRoomHours:#0.00} hours, {wetChemName}: {totalWetChemHours:#0.00} hours";

            lblRoomsSum.Text = string.Empty;
            if (dtRoom.Rows.Count > 0)
            {
                double totalRoomCharge = Convert.ToDouble(dtRoom.Compute("SUM(LineCost)", string.Empty));
                lblRoom.Text = string.Format("Total room usage fees: {0:$#,##0.00}", totalRoomCharge);
                UpdateRoomSums(dtRoom, lblRoomsSum);
            }
            else
                lblRoom.Text = "No room usage in this period";

            lblRoom.Visible = true;
        }

        protected string GetChargeDays(object dataItem)
        {
            DataRowView drv = (DataRowView)dataItem;

            bool isParent = Convert.ToBoolean(drv["IsParent"]);
            object obj = drv["ChargeDays"];

            decimal chargeDays = 0;

            if (obj != null && obj != DBNull.Value)
                chargeDays = Convert.ToDecimal(obj);

            if (isParent)
                return chargeDays.ToString("#0.00");
            else
                return string.Empty;
        }

        protected string GetDailyFee(object dataItem)
        {
            DataRowView drv = (DataRowView)dataItem;

            bool isParent = Convert.ToBoolean(drv["IsParent"]);
            object obj = drv["DailyFee"];

            decimal dailyFee = 0;

            if (obj != null && obj != DBNull.Value)
                dailyFee = Convert.ToDecimal(obj);

            if (isParent)
                return dailyFee.ToString("C");
            else
                return string.Empty;
        }

        protected string GetEntries(object dataItem)
        {
            DataRowView drv = (DataRowView)dataItem;

            bool isParent = Convert.ToBoolean(drv["IsParent"]);
            object obj = drv["Entries"];

            decimal entries = 0;

            if (obj != null && obj != DBNull.Value)
                entries = Convert.ToDecimal(obj);

            if (isParent)
                return string.Empty;
            else
                return entries.ToString("#0.00");
        }

        protected string GetEntryFee(object dataItem)
        {
            DataRowView drv = (DataRowView)dataItem;

            bool isParent = Convert.ToBoolean(drv["IsParent"]);
            object obj = drv["EntryFee"];

            decimal entryFee = 0;

            if (obj != null && obj != DBNull.Value)
                entryFee = Convert.ToDecimal(obj);

            if (isParent)
                return string.Empty;
            else
                return entryFee.ToString("C");
        }

        public void PopulateToolDetailData(DateTime period, int clientId)
        {
            //Tool - despite the word 'Detail' in the function name this is actually an aggregate by tool
            var toolBilling = new LNF.Reporting.Individual.ToolBilling(Provider);
            var toolDetail = ToolDetailUtility.GetToolDetailResult(period, clientId, toolBilling);

            lbl20110401RoomSum.Text = toolDetail.LabelRoomSum.Text;
            lbl20110401RoomSum.Visible = toolDetail.LabelRoomSum.Visible;
            lbl20110401ResFee.Text = toolDetail.LabelResFee.Text;
            lbl20110401ResFee.Visible = toolDetail.LabelResFee.Visible;
            lblTool.Text = toolDetail.LabelTool.Text;
            lblTool.Visible = toolDetail.LabelTool.Visible;

            rptToolDetail.DataSource = toolDetail.Items;
            rptToolDetail.DataBind();
        }

        private void PopulateStoreDetailData(DateTime period, int clientId)
        {
            // gets either current or prior period data
            DataTable dtStore = StoreBillingBL.GetStoreBillingDataByClientID(ContextBase, period, clientId);

            gvStoreDetail.DataSource = dtStore;
            gvStoreDetail.DataBind();

            if (dtStore.Rows.Count > 0)
            {
                double totalStoreCharge = Convert.ToDouble(dtStore.Compute("SUM(LineCost)", string.Empty));
                lblStore.Text = string.Format("Total store usage fees: {0:$#,##0.00}", totalStoreCharge);
            }
            else
                lblStore.Text = "No store usage in this period";

            lblStore.Visible = true;
        }

        private void HandlePageDisplay(DateTime period, int clientId)
        {
            if (period >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                divAggReports.Visible = false;
            else
                divAggReports.Visible = true;

            divReportContent.Visible = true;

            lblClientID.ForeColor = System.Drawing.Color.Black;
            lblClientID.Text = clientId.ToString();
        }

        protected override void RunReport(DateTime period, int clientId)
        {
            if (clientId <= 0)
            {
                lblClientID.ForeColor = System.Drawing.Color.Red;
                lblClientID.Text = "&larr; Required";
                return;
            }

            Session.Remove("UserUsageSummaryTables");
            Session.Remove("UserUsageSummaryTables20110701");

            //Because of this cutoff date, the stored procedure BillingTables_Select20110701 should always be used
            DateTime cutoff = new DateTime(2011, 10, 1);
            if (period < cutoff)
            {
                Response.Redirect($"~/IndUserUsageSummary20110401.aspx?p={period:yyyy-MM-dd}&cid={clientId}");
                return;
            }

            // 01) Billing Details: Room
            PopulateRoomDetailData(period, clientId);

            // 02) Billing Details: Tool
            PopulateToolDetailData(period, clientId);

            // 03) Billing Details: Store
            PopulateStoreDetailData(period, clientId);

            // 04) Aggregate by Organization: Room
            gvAggByOrgRoom.DataSource = RoomBillingByOrgBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvAggByOrgRoom.DataBind();

            // 05) Aggregate by Organization: Tool
            gvToolOrg20110701.DataSource = ToolBillingByOrgBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvToolOrg20110701.DataBind();
            gvToolOrg20110701.Visible = true;

            // 06) Aggregate by Organization: Store
            var dtStore = BillingTablesBL.GetMultipleTables(ContextBase, period.Year, period.Month, clientId, BillingTableType.StoreByOrg);
            gvStoreOrg.DataSource = dtStore;
            gvStoreOrg.DataBind();

            // 07) Aggregate by Organization: Subsidy
            var dtSubsidy = TieredSubsidyBillingBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            AddStoreChargesToSubsidyTable(dtStore, dtSubsidy);
            gvSubsidy.DataSource = dtSubsidy;
            gvSubsidy.DataBind();

            // 08) Aggregate by Accounts: Room
            gvRoomAccount.DataSource = RoomBillingByAccountBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvRoomAccount.DataBind();

            // 09) Aggregate by Accounts: Tool
            gvToolAccount20110701.DataSource = ToolBillingByAccountBL.GetDataByPeriodAndClientID20110701(ContextBase, period.Year, period.Month, clientId);
            gvToolAccount20110701.DataBind();
            gvToolAccount20110701.Visible = true;

            // 10) Aggregate by Accounts: Store
            gvStoreAccount.DataSource = StoreBillingByAccountBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvStoreAccount.DataBind();

            // 11) Billing Details: Misc
            gvMisc.DataSource = MiscBillingBL.GetMiscBillingByClientID(ContextBase, period.Year, period.Month, clientId);
            gvMisc.DataBind();

            PopulateReportInfo(divReportInfo, SelectedClientID, pp1.SelectedPeriod);

            HandlePageDisplay(period, clientId);
        }

        private void AddStoreChargesToSubsidyTable(DataTable dtStore, DataTable dtSubsidy)
        {
            var storeChargesByOrg = dtStore.AsEnumerable().Select(x => new
            {
                OrgID = x.Field<int>("OrgID"),
                StoreSum = x.Field<decimal>("TotalChargeNoMisc"),
                StoreMiscSum = x.Field<decimal>("StoreMisc"),
                TotalCharge = x.Field<decimal>("TotalCharge")
            }).ToList();

            dtSubsidy.Columns.Add("StoreSum", typeof(decimal));
            dtSubsidy.Columns.Add("StoreMiscSum", typeof(decimal));
            dtSubsidy.Columns.Add("UsageCharges", typeof(decimal), "UserTotalSum + StoreSum + StoreMiscSum");
            dtSubsidy.Columns.Add("NetCharges", typeof(decimal), "UserPaymentSum + StoreSum + StoreMiscSum");

            foreach (DataRow dr in dtSubsidy.Rows)
            {
                int orgId = dr.Field<int>("OrgID");
                var storeCharge = storeChargesByOrg.FirstOrDefault(x => x.OrgID == orgId);
                if (storeCharge != null)
                {
                    dr.SetField("StoreSum", storeCharge.StoreSum);
                    dr.SetField("StoreMiscSum", storeCharge.StoreMiscSum);
                }
                else
                {
                    dr.SetField("StoreSum", 0D);
                    dr.SetField("StoreMiscSum", 0D);
                }
            }
        }

        protected void GvToolDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[4].ToolTip = "Actual start time - actual end time";
                e.Row.Cells[5].ToolTip = "Time for unused, but activated reservation";
                e.Row.Cells[6].ToolTip = "Overtime - used time after original end time";
                e.Row.Cells[7].ToolTip = "Reserved reservation that has not been used";
                e.Row.Cells[8].ToolTip = "10% of all cancellations before 2 hours of starting time";
                e.Row.Cells[9].ToolTip = "Time used by subsequent users";
            }
        }

        protected void GvToolAccount20110701_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.Cells[4].ToolTip = "Your total reservation time, regardless of whether you activated or not";
        }

        protected void GvToolOrg20110701_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.Cells[2].ToolTip = "Your total reservation time, regardless of whether you activated or not";
        }

        public RoomBillingDetailItem CreateRoomBillingDetailItem(int roomId, DataTable dt)
        {
            DataRow[] rows = dt.Select(string.Format("RoomID = {0}", roomId));
            if (rows.Length > 0)
            {
                RoomBillingDetailItem result = new RoomBillingDetailItem
                {
                    RoomName = rows[0].Field<string>("Room")
                };

                RoomBillingDetailAccount[] accounts = rows.Select(x => new RoomBillingDetailAccount()
                {
                    AccountName = x.Field<string>("Name"),
                    BillingType = x.Field<string>("BillingTypeName"),
                    DailyFee = x.Field<double>("DailyFee"),
                    Days = x.Field<double>("Days"),
                    Entries = x.Field<double>("Entries"),
                    EntryFee = x.Field<double>("EntryFee"),
                    LineTotal = x.Field<double>("LineTotal"),
                    ShortCode = x.Field<string>("ShortCode")
                }).ToArray();

                return result;
            }

            return null;
        }

        protected void ChkShowDisclaimer_CheckedChanged(object sender, EventArgs e)
        {
            SetShowDisclaimerSetting(chkShowDisclaimer.Checked);
            ShowDisclaimer();
        }

        protected override void OnSelectedPeriodChanged(PeriodChangedEventArgs e)
        {
            ShowDisclaimer();
        }

        protected string GetResourceDetailUrl(ToolDetailItem item)
        {
            return $"ResourceDetail.aspx?ResourceID={item.ResourceID}&Period={SelectedPeriod:yyyy-MM-dd}&ClientID={item.ClientID}&AccountID={item.AccountID}";
        }
    }

    public class RoomBillingDetailItem
    {
        public string RoomName { get; set; }
        public RoomBillingDetailAccount[] Accounts { get; set; }
    }

    public class RoomBillingDetailAccount
    {
        public double Days { get; set; }
        public double Entries { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public string BillingType { get; set; }
        public double DailyFee { get; set; }
        public double EntryFee { get; set; }
        public double LineTotal { get; set; }
    }
}