using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndUserUsageSummary20100701 : UserUsageSummaryPage
    {
        public override bool ShowButton
        {
            get { return false; }
        }

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
            if (!Page.IsPostBack)
            {
                SelectedPeriod = DateTime.Parse("2011-03-01");
                Session.Remove("UserUsageSummaryTables");
                Session.Remove("UserUsageSummaryTables20110701");
            }

            base.OnLoad(e);
        }

        protected override void RunReport(DateTime period, int clientId)
        {
            Session.Remove("UserUsageSummaryTables");
            Session.Remove("UserUsageSummaryTables20110701");
            
            DateTime CutoffStart = new DateTime(2010, 7, 1);
            DateTime CutoffEnd = new DateTime(2011, 4, 1);

            if (period < CutoffStart)
            {
                Response.Redirect(string.Format("~/IndUserUsageSummary.aspx?p={0:yyyy-MM-dd}&cid={1}", period, clientId));
                return;
            }

            if (period >= CutoffEnd)
            {
                Response.Redirect(string.Format("~/IndUserUsageSummary20110401.aspx?p={0:yyyy-MM-dd}&cid={1}", period, clientId));
                return;
            }

            if (period >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                divAggReports.Visible = false;
            else
                divAggReports.Visible = true;

            PopulateRoomData(period, clientId);

            if (period < CutoffEnd)
            {
                PopulateToolData(period, clientId);
                divTool.Visible = true;
                divTool20110401.Visible = false;
            }
            else
            {
                PopulateToolData20110401(period, clientId);
                divTool.Visible = false;
                divTool20110401.Visible = true;
            }

            PopulateStoreData(period, clientId);

            divReportContent.Visible = true;
            lblClientID.Text = clientId.ToString();

            gvRoomOrg.DataSource = RoomBillingByOrgBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvRoomOrg.DataBind();

            gvStoreOrg.DataSource = BillingTablesBL.GetMultipleTables(ContextBase, period.Year, period.Month, clientId, BillingTableType.StoreByOrg);
            gvStoreOrg.DataBind();

            gvSubsidy.DataSource = TieredSubsidyBillingBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvSubsidy.DataBind();

            gvRoomAccount.DataSource = RoomBillingByAccountBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvRoomAccount.DataBind();

            gvStoreAccount.DataSource = StoreBillingByAccountBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
            gvStoreAccount.DataBind();

            gvMisc.DataSource = MiscBillingBL.GetMiscBillingByClientID(ContextBase, period.Year, period.Month, clientId);
            gvMisc.DataBind();

            if (period < CutoffEnd)
            {
                gvToolOrg.DataSource = ToolBillingByOrgBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
                gvToolOrg.DataBind();
                gvToolOrg.Visible = true;

                gvToolAccount.DataSource = ToolBillingByAccountBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
                gvToolAccount.DataBind();
                gvToolAccount.Visible = true;

                gvToolOrg20110401.Visible = false;
                gvToolAccount20110401.Visible = false;
            }
            else
            {
                gvToolOrg20110401.DataSource = ToolBillingByOrgBL.GetDataByPeriodAndClientID(ContextBase, period.Year, period.Month, clientId);
                gvToolOrg20110401.DataBind();
                gvToolOrg20110401.Visible = true;

                gvToolAccount20110401.DataSource = ToolBillingByAccountBL.GetDataByPeriodAndClientID20110401(ContextBase, period.Year, period.Month, clientId);
                gvToolAccount20110401.DataBind();
                gvToolAccount20110401.Visible = true;

                gvToolOrg.Visible = false;
                gvToolAccount.Visible = false;
            }
        }

        private void PopulateRoomData(DateTime period, int clientId)
        {
            dtRoom = RoomBillingBL.GetRoomBillingDataByClientID(ContextBase, period, clientId);
            gvRoom.DataSource = dtRoom;
            gvRoom.DataBind();

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

        private void PopulateToolData(DateTime period, int clientId)
        {
            //Tool
            DataSet dsTool = ToolBillingBL.GetToolBillingDataByClientID_Old(period, clientId);
            dtToolActivated = dsTool.Tables[0];
            dtToolUncancelled = dsTool.Tables[1];
            DataTable dtToolForgiven = dsTool.Tables[2];

            gvTool.DataSource = dtToolActivated;
            gvTool.DataBind();

            gvToolCancelled.DataSource = dtToolUncancelled;
            gvToolCancelled.DataBind();

            gvToolForgiven.DataSource = dtToolForgiven;
            gvToolForgiven.DataBind();

            double subTotalActivated = 0;
            double subTotalUncancelled = 0;

            lblRoomsSumActivated.Text = string.Empty;
            lblActivatedToolFee.Text = string.Empty;
            if (dtToolActivated.Rows.Count > 0)
            {
                subTotalActivated = Convert.ToDouble(dtToolActivated.Compute("SUM(LineCost)", string.Empty));
                lblActivatedToolFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalActivated);
                lblActivatedToolFee.Visible = true;
                UpdateRoomSums(dtToolActivated, lblRoomsSumActivated);
            }

            lblCancelledToolFee.Text = string.Empty;
            lblRoomSumUnCancelled.Text = string.Empty;
            if (dtToolUncancelled.Rows.Count > 0)
            {
                subTotalUncancelled = Convert.ToDouble(dtToolUncancelled.Compute("SUM(LineCost)", string.Empty));
                lblCancelledToolFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalUncancelled);
                lblCancelledToolFee.Visible = true;
                UpdateRoomSums(dtToolUncancelled, lblRoomSumUnCancelled);
            }

            if (subTotalActivated == 0 && subTotalUncancelled == 0)
                lblTool.Text = "No tool usage fees in this period";
            else
                lblTool.Text = string.Format("Total tool usage fees: {0:$#,##0.00}", subTotalActivated + subTotalUncancelled);

            lblTool.Visible = true;
            divTool.Visible = true;
        }

        private void PopulateToolData20110401(DateTime period, int clientId)
        {
            //Tool
            DataSet dsTool = ToolBillingBL.GetToolBillingDataByClientID_Old(period, clientId);
            DataTable dtTool20110401 = dsTool.Tables[0];
            DataTable dtToolCancelled20110401 = dsTool.Tables[1];
            DataTable dtToolForgiven20110401 = dsTool.Tables[2];

            gvTool20110401.DataSource = dtTool20110401;
            gvTool20110401.DataBind();

            gvToolCancelled20110401.DataSource = dtToolCancelled20110401;
            gvToolCancelled20110401.DataBind();

            gvToolForgiven20110401.DataSource = dtToolForgiven20110401;
            gvToolForgiven20110401.DataBind();

            double subTotalActivated = 0;
            double subTotalUncancelled = 0;

            lbl20110401RoomSum.Text = string.Empty;
            lbl20110401ResFee.Text = string.Empty;
            if (dtTool20110401.Rows.Count > 0)
            {
                subTotalActivated = Convert.ToDouble(dtTool20110401.Compute("SUM(LineCost)", string.Empty));
                lbl20110401ResFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalActivated);
                lbl20110401ResFee.Visible = true;
                UpdateRoomSums(dtTool20110401, lbl20110401RoomSum);
            }

            lbl20110401CanFee.Text = string.Empty;
            lbl20110401RoomSumCan.Text = string.Empty;
            if (dtToolCancelled20110401.Rows.Count > 0)
            {
                subTotalUncancelled = Convert.ToDouble(dtToolCancelled20110401.Compute("SUM(LineCost)", string.Empty));
                lbl20110401CanFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalUncancelled);
                lbl20110401CanFee.Visible = true;
                UpdateRoomSums(dtToolCancelled20110401, lbl20110401RoomSumCan);
            }

            if (subTotalActivated == 0 && subTotalUncancelled == 0)
                lblTool.Text = "No tool usage fees in this period";
            else
                lblTool.Text = string.Format("Total tool usage fees: {0:$#,##0.00}", subTotalActivated + subTotalUncancelled);

            lblTool.Visible = true;
            divTool20110401.Visible = true;
        }

        private void PopulateStoreData(DateTime period, int clientId)
        {
            dtStore = StoreBillingBL.GetStoreBillingDataByClientID(ContextBase, period, clientId);
            gvStore.DataSource = dtStore;
            gvStore.DataBind();

            if (dtStore.Rows.Count > 0)
            {
                double totalStoreCharge = Convert.ToDouble(dtStore.Compute("SUM(LineCost)", string.Empty));
                lblStore.Text = string.Format("Total store usage fees: {0:$#,##0.00}", totalStoreCharge);
            }
            else
                lblStore.Text = "No store usage in this period";

            lblStore.Visible = true;
        }
    }
}