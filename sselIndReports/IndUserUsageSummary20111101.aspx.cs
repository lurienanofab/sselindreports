using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
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

            base.OnLoad(e);
        }

        public GlobalSettings ShowDisclaimerSetting
        {
            get
            {
                if (_ShowDisclaimerSetting == null)
                {
                    _ShowDisclaimerSetting = DA.Current.Query<GlobalSettings>().FirstOrDefault(x => x.SettingName == "ShowUserUsageSummaryDisclaimer");

                    if (_ShowDisclaimerSetting == null)
                    {
                        _ShowDisclaimerSetting = new GlobalSettings() { SettingName = "ShowUserUsageSummaryDisclaimer", SettingValue = "false" };
                        DA.Current.SaveOrUpdate(_ShowDisclaimerSetting);
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
            DA.Current.SaveOrUpdate(ShowDisclaimerSetting);
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
            DataTable dtRoom = RoomBillingBL.GetRoomBillingDataByClientID(period, clientId);

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
                Rooms room = RoomUtility.GetRoom(dr.Field<int>("RoomID"));

                // Using Convert.ToDecimal because value might be decimal or double depending no if it is from RoomBilling or RoomBillingTemp
                decimal hours = Convert.ToDecimal(dr["Hours"]);

                if (room == Rooms.CleanRoom)
                    totalCleanRoomHours += hours;
                else if (room == Rooms.WetChemistry)
                    totalWetChemHours += hours;
                else if (room == Rooms.TestLab)
                    totalTestLabHours += hours;
                else if (room == Rooms.OrganicsBay)
                    totalOrganicsHours += hours;
                else if (room == Rooms.LNF)
                    totalLnfHours += hours;
            }

            lblRoomHours.Text = string.Format("|  LNF: {0:#0.00} hours, Clean Room: {1:#0.00} hours, Wet Chem: {2:#0.00} hours", totalLnfHours, totalCleanRoomHours, totalWetChemHours);

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

        public void PopulateToolDetailData(DateTime period, int clientId)
        {
            //Tool - despite the word 'Detail' in the function name this is actually an aggregate by tool
            //DataTable dtTool = ToolBillingBL.GetToolBillingDataByClientID20110701(period, clientId);
            IToolBilling[] query;

            if (DateTime.Now.FirstOfMonth() == period)
                query = DA.Current.Query<ToolBillingTemp>().Where(x => x.Period == period && x.ClientID == clientId).ToArray();
            else
                query = DA.Current.Query<ToolBilling>().Where(x => x.Period == period && x.ClientID == clientId).ToArray();

            DataTable dtTool = ToolBillingBL.GetAggreateByTool(query);
            dtTool.DefaultView.Sort = "RoomName ASC, ResourceName ASC";
            gvToolDetail.DataSource = dtTool;
            gvToolDetail.DataBind();

            decimal subTotalActivated = 0;

            lbl20110401RoomSum.Text = string.Empty;
            lbl20110401ResFee.Text = string.Empty;
            if (dtTool.Rows.Count > 0)
            {
                subTotalActivated = Convert.ToDecimal(dtTool.Compute("SUM(LineCost)", string.Empty));
                lbl20110401ResFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalActivated);
                lbl20110401ResFee.Visible = true;
                UpdateRoomSums(dtTool, lbl20110401RoomSum);
            }

            if (subTotalActivated == 0)
                lblTool.Text = "No tool usage fees in this period";
            else
                lblTool.Text = string.Format("Total tool usage fees: {0:$#,##0.00}", subTotalActivated);

            lblTool.Visible = true;
        }

        private void PopulateStoreDetailData(DateTime period, int clientId)
        {
            // gets either current or prior period data
            DataTable dtStore = StoreBillingBL.GetStoreBillingDataByClientID(period, clientId);

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
            gvAggByOrgRoom.DataSource = RoomBillingByOrgBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvAggByOrgRoom.DataBind();

            // 05) Aggregate by Organization: Tool
            gvToolOrg20110701.DataSource = ToolBillingByOrgBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvToolOrg20110701.DataBind();
            gvToolOrg20110701.Visible = true;

            // 06) Aggregate by Organization: Store
            var dtStore = BillingTablesBL.GetMultipleTables(period.Year, period.Month, clientId, BillingTableType.StoreByOrg);
            gvStoreOrg.DataSource = dtStore;
            gvStoreOrg.DataBind();

            // 07) Aggregate by Organization: Subsidy
            var dtSubsidy = TieredSubsidyBillingBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            AddStoreChargesToSubsidyTable(dtStore, dtSubsidy);
            gvSubsidy.DataSource = dtSubsidy;
            gvSubsidy.DataBind();

            // 08) Aggregate by Accounts: Room
            gvRoomAccount.DataSource = RoomBillingByAccountBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvRoomAccount.DataBind();

            // 09) Aggregate by Accounts: Tool
            gvToolAccount20110701.DataSource = ToolBillingByAccountBL.GetDataByPeriodAndClientID20110701(period.Year, period.Month, clientId);
            gvToolAccount20110701.DataBind();
            gvToolAccount20110701.Visible = true;

            // 10) Aggregate by Accounts: Store
            gvStoreAccount.DataSource = StoreBillingByAccountBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvStoreAccount.DataBind();

            // 11) Billing Details: Misc
            gvMisc.DataSource = MiscBillingBL.GetMiscBillingByClientID(period.Year, period.Month, clientId);
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

        protected void gvToolDetail_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void gvToolAccount20110701_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.Cells[4].ToolTip = "Your total reservation time, regardless of whether you activated or not";
        }

        protected void gvToolOrg20110701_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.Cells[2].ToolTip = "Your total reservation time, regardless of whether you activated or not";
        }

        public RoomBillingDetailItem CreateRoomBillingDetailItem(int roomId, DataTable dt)
        {
            DataRow[] rows = dt.Select(string.Format("RoomID = {0}", roomId));
            if (rows.Length > 0)
            {
                RoomBillingDetailItem result = new RoomBillingDetailItem();
                result.RoomName = rows[0].Field<string>("Room");
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

        protected void chkShowDisclaimer_CheckedChanged(object sender, EventArgs e)
        {
            SetShowDisclaimerSetting(chkShowDisclaimer.Checked);
            ShowDisclaimer();
        }

        protected override void OnSelectedPeriodChanged(PeriodChangedEventArgs e)
        {
            ShowDisclaimer();
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