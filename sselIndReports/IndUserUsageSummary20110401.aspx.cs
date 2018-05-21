using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndUserUsageSummary20110401 : UserUsageSummaryPage
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
                SelectedPeriod = DateTime.Parse("2011-09-01");
                Session.Remove("UserUsageSummaryTables");
                Session.Remove("UserUsageSummaryTables20110701");
            }

            base.OnLoad(e);
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

            DateTime CutoffStart = new DateTime(2011, 4, 1);
            DateTime CutoffEnd = new DateTime(2011, 10, 1);

            if (period < CutoffStart)
            {
                Response.Redirect(string.Format("~/IndUserUsageSummary20100701.aspx?p={0:yyyy-MM-dd}&cid={1}", period, clientId));
                return;
            }

            if (period >= CutoffEnd)
            {
                Response.Redirect(string.Format("~/IndUserUsageSummary20111101.aspx?p={0:yyyy-MM-dd}&cid={1}", period, clientId));
                return;
            }

            if (period >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                divAggReports.Visible = false;
            else
                divAggReports.Visible = true;

            PopulateRoomData(period, clientId);

            if (period < CutoffStart)
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

            lblClientID.ForeColor = System.Drawing.Color.Black;
            lblClientID.Text = clientId.ToString();

            gvRoomOrg.DataSource = RoomBillingByOrgBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvRoomOrg.DataBind();

            gvStoreOrg.DataSource = BillingTablesBL.GetMultipleTables(period.Year, period.Month, clientId, BillingTableType.StoreByOrg);
            gvStoreOrg.DataBind();

            gvSubsidy.DataSource = TieredSubsidyBillingBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvSubsidy.DataBind();

            gvRoomAccount.DataSource = RoomBillingByAccountBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvRoomAccount.DataBind();

            gvStoreAccount.DataSource = StoreBillingByAccountBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
            gvStoreAccount.DataBind();

            gvMisc.DataSource = MiscBillingBL.GetMiscBillingByClientID(period.Year, period.Month, clientId);
            gvMisc.DataBind();

            if (period < CutoffStart)
            {
                gvToolOrg.DataSource = ToolBillingByOrgBL.GetDataByPeriodAndClientID20110401(period.Year, period.Month, clientId);
                gvToolOrg.DataBind();
                gvToolOrg.Visible = true;

                gvToolAccount.DataSource = ToolBillingByAccountBL.GetDataByPeriodAndClientID(period.Year, period.Month, clientId);
                gvToolAccount.DataBind();
                gvToolAccount.Visible = true;

                gvToolOrg20110401.Visible = false;
                gvToolAccount20110401.Visible = false;
            }
            else
            {
                gvToolOrg20110401.DataSource = ToolBillingByOrgBL.GetDataByPeriodAndClientID20110401(period.Year, period.Month, clientId);
                gvToolOrg20110401.DataBind();
                gvToolOrg20110401.Visible = true;

                gvToolAccount20110401.DataSource = ToolBillingByAccountBL.GetDataByPeriodAndClientID20110401(period.Year, period.Month, clientId);
                gvToolAccount20110401.DataBind();
                gvToolAccount20110401.Visible = true;

                gvToolOrg.Visible = false;
                gvToolAccount.Visible = false;
            }
        }

        private void PopulateRoomData(DateTime period, int clientId)
        {
            dtRoom = RoomBillingBL.GetRoomBillingDataByClientID(period, clientId);
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
            DataSet dsTool = ToolBillingBL.GetToolBillingDataByClientID20110401(period, clientId);
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
            dtStore = StoreBillingBL.GetStoreBillingDataByClientID(period, clientId);
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

        private void PopulateSummaryDataList(DateTime period, int clientId)
        {
            DataTable dtSubsidy = SubsidyBillingDA.GetSubsidyBillingDataByClientID(period, clientId);
            DataTable dtMisc = MiscBillingChargeDA.GetDataByPeriodAndClientID(period, clientId);

            DataTable dtSummary = new DataTable();
            dtSummary.Columns.Add("OrgID", typeof(int));
            dtSummary.Columns.Add("OrgName", typeof(string));
            dtSummary.Columns.Add("RoomTotal", typeof(double));
            dtSummary.Columns.Add("ToolTotal", typeof(double));
            dtSummary.Columns.Add("StoreTotal", typeof(double));
            dtSummary.Columns.Add("RoomMiscTotal", typeof(double));
            dtSummary.Columns.Add("ToolMiscTotal", typeof(double));
            dtSummary.Columns.Add("StoreMiscTotal", typeof(double));
            dtSummary.Columns.Add("SubsidizedAmount", typeof(double));
            dtSummary.Columns.Add("TotalCostSubsidized", typeof(double));

            int currentOrgId, previousOrgId = -1;
            DataView dv = dtRoom.DefaultView;
            dv.Sort = "OrgID";

            foreach (DataRowView drv in dv)
            {
                currentOrgId = Convert.ToInt32(drv["OrgID"]);
                if (currentOrgId != previousOrgId)
                {
                    DataRow nr = dtSummary.NewRow();
                    nr["OrgID"] = currentOrgId;
                    nr["OrgName"] = drv["OrgName"];
                    nr["RoomTotal"] = 0;
                    nr["ToolTotal"] = 0;
                    nr["StoreTotal"] = 0;

                    DataRow[] rows = dtMisc.Select(string.Format("OrgID = {0}", currentOrgId));

                    if (rows.Length > 0)
                    {
                        foreach (DataRow row in rows)
                        {
                            if (row["SUBType"].ToString() == "Room")
                                nr["RoomMiscTotal"] = rows[0]["MiscCost"];
                            else if (row["SUBType"].ToString() == "Tool")
                                nr["ToolMiscTotal"] = rows[0]["MiscCost"];
                            else if (row["SUBType"].ToString() == "Store")
                                nr["StoreMiscTotal"] = rows[0]["MiscCost"];
                        }
                    }
                    else
                    {
                        nr["RoomMiscTotal"] = 0;
                        nr["ToolMiscTotal"] = 0;
                        nr["StoreMiscTotal"] = 0;
                    }

                    if (IsPrimaryOrg(currentOrgId))
                    {
                        if (dtSubsidy.Rows.Count == 1)
                        {
                            nr["SubsidizedAmount"] = dtSubsidy.Rows[0].Field<double>("UserTotalSum") - dtSubsidy.Rows[0].Field<double>("UserPaymentSum");
                            nr["TotalCostSubsidized"] = dtSubsidy.Rows[0]["UserPaymentSum"];
                        }
                    }

                    dtSummary.Rows.Add(nr);
                }

                previousOrgId = currentOrgId;
            }

            object totalRoomChargePerOrg;
            object totalToolActivatedPerOrg;
            object totalToolUncancelledPerOrg;
            object totalStorePerOrg;

            foreach (DataRow dr in dtSummary.Rows)
            {
                totalRoomChargePerOrg = DBNull.Value;
                totalToolActivatedPerOrg = DBNull.Value;
                totalToolUncancelledPerOrg = DBNull.Value;
                totalStorePerOrg = DBNull.Value;

                if (dtRoom.Rows.Count > 0)
                    totalRoomChargePerOrg = dtRoom.Compute("SUM(LineCost)", string.Format("OrgID = {0}", dr["OrgID"]));

                if (totalRoomChargePerOrg != DBNull.Value)
                    dr["RoomTotal"] = Convert.ToDouble(totalRoomChargePerOrg);

                if (dtToolActivated.Rows.Count > 0)
                    totalToolActivatedPerOrg = dtToolActivated.Compute("SUM(LineCost)", string.Format("OrgID = {0}", dr["OrgID"]));

                if (dtToolUncancelled.Rows.Count > 0)
                    totalToolUncancelledPerOrg = dtToolUncancelled.Compute("SUM(LineCost)", string.Format("OrgID = {0}", dr["OrgID"]));

                if (totalToolActivatedPerOrg != DBNull.Value)
                    dr["ToolTotal"] = Convert.ToDouble(totalToolActivatedPerOrg);

                if (totalToolUncancelledPerOrg != DBNull.Value)
                    dr["ToolTotal"] = dr.Field<double>("ToolTotal") + Convert.ToDouble(totalToolUncancelledPerOrg);

                if (dtStore.Rows.Count > 0)
                    totalStorePerOrg = dtStore.Compute("SUM(LineCost)", string.Format("OrgID = {0}", dr["OrgID"]));

                if (totalStorePerOrg != DBNull.Value)
                    dr["StoreTotal"] = Convert.ToDouble(totalStorePerOrg);
            }
        }
    }
}