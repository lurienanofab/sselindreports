using LNF.Repository.Billing;
using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndUserUsageSummary : UserUsageSummaryPage
    {
        protected override DateTime SelectedPeriod
        {
            get { return pp1.SelectedPeriod; }
            set { pp1.SelectedPeriod = value; }
        }

        protected override Label SummaryApproximateLabel
        {
            get { return null; }
        }

        protected override DropDownList ClientDropDownList
        {
            get { return ddlUser; }
        }

        protected override Button RetrieveDataButton
        {
            get { return btnReport; }
        }

        protected override void RunReport(DateTime period, int clientId)
        {
            Session.Remove("UserUsageSummaryTables");
            Session.Remove("UserUsageSummaryTables20110701");

            lblRoom.Visible = false;
            lblTool.Visible = false;
            lblStore.Visible = false;
            divTool.Visible = false;
            lblActivatedToolFee.Visible = false;
            lblCancelledToolFee.Visible = false;
            lblRoomsSum.Visible = false;
            lblRoomsSum.Text = string.Empty;
            lblRoomsSumActivated.Visible = false;
            lblRoomSumUnCancelled.Visible = false;
            lblRoomsSumActivated.Text = string.Empty;
            lblRoomSumUnCancelled.Text = string.Empty;

            DateTime CutoffStart = new DateTime(2009, 7, 1);
            DateTime CutoffEnd = new DateTime(2010, 7, 1);

            if (period < CutoffStart)
            {
                Response.Redirect(string.Format("~/IndSumUsage.aspx?p={0:yyyy-MM-dd}&cid={1}", period, clientId));
                return;
            }

            if (period >= CutoffEnd)
            {
                Response.Redirect(string.Format("~/IndUserUsageSummary20100701.aspx?p={0:yyyy-MM-dd}&cid={1}", period, clientId));
                return;
            }

            //Room
            dtRoom = RoomBillingBL.GetRoomBillingDataByClientID(period, clientId);
            gvRoom.DataSource = dtRoom;
            gvRoom.DataBind();

            if (dtRoom.Rows.Count > 0)
            {
                double totalRoomCharge = Convert.ToDouble(dtRoom.Compute("SUM(LineCost)", string.Empty));
                lblRoom.Text = string.Format("Total room usage fees: {0:$#,##0.00}", totalRoomCharge);
                UpdateRoomSums(dtRoom, lblRoomsSum);
            }
            else
                lblRoom.Text = "No room usage during period";
            
            lblRoom.Visible = true;

            //Tool
            DataSet dsTool = ToolBillingBL.GetToolBillingDataByClientID20110401(period, clientId);
            dtToolActivated  = dsTool.Tables[0];
            dtToolUncancelled = dsTool.Tables[1];
            dtToolForgiven = dsTool.Tables[2];

            gvTool.DataSource = dtToolActivated;
            gvTool.DataBind();

            gvToolCancelled.DataSource = dtToolUncancelled;
            gvToolCancelled.DataBind();

            gvToolForgiven.DataSource = dtToolForgiven;
            gvToolForgiven.DataBind();

            double subTotalActivated = 0;
            double subTotalUncancelled = 0;
            if (dtToolActivated.Rows.Count > 0)
            {
                subTotalActivated = Convert.ToDouble(dtToolActivated.Compute("SUM(LineCost)", string.Empty));
                lblActivatedToolFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalActivated);
                lblActivatedToolFee.Visible = true;

                UpdateRoomSums(dtToolActivated, lblRoomsSumActivated);
            }

            if (dtToolUncancelled.Rows.Count > 0)
            {
                subTotalUncancelled = Convert.ToDouble(dtToolUncancelled.Compute("SUM(LineCost)", string.Empty));
                lblCancelledToolFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalUncancelled);
                lblCancelledToolFee.Visible = true;

                UpdateRoomSums(dtToolUncancelled, lblRoomSumUnCancelled);
            }

            if (subTotalActivated == 0 && subTotalUncancelled == 0)
                lblTool.Text = "No tool usage fees during this period";
            else
                lblTool.Text = string.Format("Total tool usage fees: {0:$#,##0.00}", subTotalActivated + subTotalUncancelled);

            lblTool.Visible = true;
            divTool.Visible = true;

            //Store
            dtStore = StoreBillingBL.GetStoreBillingDataByClientID(period, clientId);
            gvStore.DataSource = dtStore;
            gvStore.DataBind();

            if (dtStore.Rows.Count > 0)
            {
                double totalStoreCharge = Convert.ToDouble(dtStore.Compute("SUM(LineCost)", string.Empty));
                lblStore.Text = string.Format("Total store usage fees: {0:$#,##0.00}", totalStoreCharge);
            }
            else
                lblStore.Text = "No store usage during period";
            
            lblStore.Visible = true;

            DataTable  dtSummary = new DataTable();
            dtSummary.Columns.Add("OrgID", typeof(int));
            dtSummary.Columns.Add("OrgName", typeof(string));
            dtSummary.Columns.Add("RoomTotal", typeof(double));
            dtSummary.Columns.Add("ToolTotal", typeof(double));
            dtSummary.Columns.Add("StoreTotal", typeof(double));

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
                    dtSummary.Rows.Add(nr);
                }
                previousOrgId = currentOrgId;
            }

            foreach (DataRow dr in dtSummary.Rows)
            {
                SetTotalPerOrgCharge(dtRoom, dr, "RoomTotal");
                SetTotalPerOrgCharge(dtToolActivated, dr, "ToolTotal");
                SetTotalPerOrgCharge(dtToolUncancelled, dr, "ToolTotal");
                SetTotalPerOrgCharge(dtStore, dr, "StoreTotal");
            }

            dlSummary.DataSource = dtSummary;
            dlSummary.DataBind();

            //2009-10-12 future billing button display
            int billingTypeId = BillingType.Other;

            if (dtRoom.Rows.Count > 0)
                billingTypeId = dtRoom.Rows[0].Field<int>("BillingTypeID");
            else if (dsTool.Tables[0].Rows.Count > 0)
                billingTypeId = dsTool.Tables[0].Rows[0].Field<int>("BillingTypeID");
            else if (dsTool.Tables[1].Rows.Count > 0)
                billingTypeId = dsTool.Tables[1].Rows[0].Field<int>("BillingTypeID");
            else if (dsTool.Tables[2].Rows.Count > 0)
                billingTypeId = dsTool.Tables[2].Rows[0].Field<int>("BillingTypeID");

            int[] specialBillingTypesForSomeUnknownReason = { BillingType.Int_Ga, BillingType.Int_Si, BillingType.ExtAc_Ga, BillingType.ExtAc_Si };
            if (specialBillingTypesForSomeUnknownReason.Contains(billingTypeId))
            {
                btnCurrent.Visible = true;
                btnCurrent.BackColor = System.Drawing.Color.LightGray;

                btnFuture.Visible = true;
                btnFuture.BackColor = System.Drawing.Color.White;
            }
            else
            {
                btnFuture.Visible = false;
                btnCurrent.Visible = false;
            }
        }

        protected void RunFutureReport(DateTime period, int clientId)
        {
            btnFuture.BackColor = System.Drawing.Color.LightGray;
            btnCurrent.BackColor = System.Drawing.Color.White;
            lblRoom.Visible = false;
            lblTool.Visible = false;
            lblStore.Visible = false;
            divTool.Visible = false;
            lblActivatedToolFee.Visible = false;
            lblCancelledToolFee.Visible = false;
            lblRoomsSum.Visible = false;
            lblRoomsSum.Text = "0";
            lblRoomsSumActivated.Visible = false;
            lblRoomSumUnCancelled.Visible = false;
            lblRoomsSumActivated.Text = string.Empty;
            lblRoomSumUnCancelled.Text = string.Empty;

            //Room
            dtRoom = RoomBillingBL.GetRoomBillingDataByClientID2(period, clientId);
            gvRoom.DataSource = dtRoom;
            gvRoom.DataBind();

            if (dtRoom.Rows.Count > 0)
            {
                double totalRoomCharge = Convert.ToDouble(dtRoom.Compute("SUM(LineCost)", string.Empty));
                lblRoom.Text = string.Format("Total room usage fees: {0:$#,##0.00}", totalRoomCharge);
                UpdateRoomSums(dtRoom, lblRoomsSum);
            }
            else
                lblRoom.Text = "No room usage during period";

            lblRoom.Visible = true;

            //Tool
            DataSet dsTool = ToolBillingBL.GetToolBillingDataByClientID_Old(period, clientId);
            dtToolActivated = dsTool.Tables[0];
            dtToolUncancelled = dsTool.Tables[1];
            dtToolForgiven = dsTool.Tables[2];

            gvTool.DataSource = dtToolActivated;
            gvTool.DataBind();

            gvToolCancelled.DataSource = dtToolUncancelled;
            gvToolCancelled.DataBind();

            gvToolForgiven.DataSource = dtToolForgiven;
            gvToolForgiven.DataBind();

            double subTotalActivated = 0;
            double subTotalUncancelled = 0;
            if (dtToolActivated.Rows.Count > 0)
            {
                subTotalActivated = Convert.ToDouble(dtToolActivated.Compute("SUM(LineCost)", string.Empty));
                lblActivatedToolFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalActivated);
                lblActivatedToolFee.Visible = true;
                UpdateRoomSums(dtToolActivated, lblRoomsSumActivated);
            }

            if (dtToolUncancelled.Rows.Count > 0)
            {
                subTotalUncancelled = Convert.ToDouble(dtToolUncancelled.Compute("SUM(LineCost)", string.Empty));
                lblCancelledToolFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalUncancelled);
                lblCancelledToolFee.Visible = true;
                UpdateRoomSums(dtToolUncancelled, lblRoomSumUnCancelled);
            }

            if (subTotalActivated == 0 && subTotalUncancelled == 0)
                lblTool.Text = "No tool usage fees during this period";
            else
                lblTool.Text = string.Format("Total tool usage fees: {0:$#,##0.00}", subTotalActivated + subTotalUncancelled);

            lblTool.Visible = true;
            divTool.Visible = true;

            //Store
            dtStore = StoreBillingBL.GetStoreBillingDataByClientID(period, clientId);
            gvStore.DataSource = dtStore;
            gvStore.DataBind();

            if (dtStore.Rows.Count > 0)
            {
                double totalStoreCharge = Convert.ToDouble(dtStore.Compute("SUM(LineCost)", string.Empty));
                lblStore.Text = string.Format("Total store usage fees: {0:$#,##0.00}", totalStoreCharge);
            }
            else
                lblStore.Text = "No store usage during period";
            
            lblStore.Visible = true;

            DataTable dtSummary = new DataTable();
            dtSummary.Columns.Add("OrgID", typeof(int));
            dtSummary.Columns.Add("OrgName", typeof(string));
            dtSummary.Columns.Add("RoomTotal", typeof(double));
            dtSummary.Columns.Add("ToolTotal", typeof(double));
            dtSummary.Columns.Add("StoreTotal", typeof(double));

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
                    dtSummary.Rows.Add(nr);
                }
                previousOrgId = currentOrgId;
            }

            foreach (DataRow dr in dtSummary.Rows)
            {
                SetTotalPerOrgCharge(dtRoom, dr, "RoomTotal");
                SetTotalPerOrgCharge(dtToolActivated, dr, "ToolTotal");
                SetTotalPerOrgCharge(dtToolUncancelled, dr, "ToolTotal");
                SetTotalPerOrgCharge(dtStore, dr, "StoreTotal");
            }

            dlSummary.DataSource = dtSummary;
            dlSummary.DataBind();
        }

        protected void gvRoom_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[0].Text == "SEM / DC Testing Lab" || e.Row.Cells[0].Text == "Organics Bay")
                    e.Row.Cells[3].Text = "N/A";
            }
        }

        protected void btnFuture_Click(object sender, EventArgs e)
        {
            RunFutureReport(SelectedPeriod, SelectedClientID);
        }
    }
}