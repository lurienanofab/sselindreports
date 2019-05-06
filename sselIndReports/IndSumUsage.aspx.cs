using LNF;
using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Web;
using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndSumUsage : UserUsageSummaryPage
    {
        public override bool ShowButton
        {
            get { return false; }
        }

        public override ClientPrivilege AuthTypes
        {
            get { return 0; }
        }

        private DataSet dsReport;
        private DataTable dtStore2; //used for second billing calculation, this is a temporary solution that need revamp later on

        protected override void RunReport(DateTime period, int clientId)
        {
            DateTime CutoffEnd = new DateTime(2009, 7, 1);

            if (period >= CutoffEnd)
            {
                Response.Redirect(string.Format("~/IndUserUsageSummary.aspx?p={0:yyyy-MM-dd}&cid={1}", period, clientId));
                return;
            }

            Prepare(ref period, ref clientId);
            UpdateTimeDepTables(period);
            GetDataForAllDataGrids(period, clientId);
            BindPerUseBilling(period, clientId);
        }

        private void Prepare(ref DateTime period, ref int clientId)
        {
            if (dsReport != null)
                return;

            lblRoom.Visible = false;
            lblTool.Visible = false;
            lblStore.Visible = false;

            dsReport = new DataSet("IndSumUsage");

            // get account, resource and item info info
            DA.Command()
                .Param("Action", "All")
                .FillDataSet(dsReport, "dbo.Account_Select", "Account");

            dsReport.Tables["Account"].PrimaryKey = new[] { dsReport.Tables["Account"].Columns["AccountID"] };

            DA.Command()
                .Param("Action", "All")
                .FillDataSet(dsReport, "dbo.Room_Select", "Room");

            dsReport.Tables["Room"].PrimaryKey = new[] { dsReport.Tables["Room"].Columns["RoomID"] };

            DA.Command()
                .Param("Action", "AllResources")
                .FillDataSet(dsReport, "dbo.sselScheduler_Select", "Resource");

            dsReport.Tables["Resource"].PrimaryKey = new[] { dsReport.Tables["Resource"].Columns["ResourceID"] };

            DA.Command()
                .Param("Action", "Item")
                .FillDataSet(dsReport, "dbo.sselMAS_Select", "Item");

            dsReport.Tables["Item"].PrimaryKey = new[] { dsReport.Tables["Item"].Columns["ItemID"] };

            //2007-05-30 Adding billing type to data grid
            //2011-06-23 GetBillingTypes was only returning active billing types. This caused problems when
            //   looking at historical data (null reference when the billing type is not active). I added
            //   a second GetBillingTypes method that takes either DBNull or a Boolean. If True then only
            //   IsActive=1, if False then only IsActive=0, if DBNull then both are returned. If no parameter
            //   is passed to GetBillingTypes the results are the same as they always were - only IsActive=1
            //   are returned.
            dsReport.Tables.Add(AppCode.BLL.BillingTypeManager.GetBillingTypes(DBNull.Value));
            dsReport.Tables[4].TableName = "BillingType";

            dsReport.Tables["BillingType"].PrimaryKey = new[] { dsReport.Tables["BillingType"].Columns["BillingTypeID"] };

            //2009-02-17 Adding org name because users might have different orgs
            dsReport.Tables.Add(OrgDA.GetAllOrgs());
            dsReport.Tables[5].TableName = "Org";

            dsReport.Tables["Org"].PrimaryKey = new[] { dsReport.Tables["Org"].Columns["OrgID"] };

            ContextBase.CacheData(dsReport);

            //2007-02-01 Add report button
            //So have to move the ddlUser populated code here
            if (CurrentUser.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Staff | ClientPrivilege.Executive))
            {
                DateTime sDate = period;
                DateTime eDate = sDate.AddMonths(1);

                var command = DA.Command()
                    .Param("sDate", sDate)
                    .Param("eDate", eDate)
                    .Param("ClientID", clientId);

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

            //2007-2-02 Two possible scenario 
            //- if the current user is user himself, we have to add the empty ddlUser with only one entry
            //else, it's a good habit to include the user him/herself as well, since the above ddlUser data code return only a group of people
            //but sometimes the user him/herself is not belong to this group of people
            //ddlUser.Items.Insert(0, new ListItem(CacheManager.Current.CurrentUser.DisplayName, CacheManager.Current.ClientID.ToString()));
            //ddlUser.SelectedValue = CacheManager.Current.ClientID.ToString();

            string urlData = Request.QueryString["URLdata"]; // iiiiyyyymm
            if (!string.IsNullOrEmpty(urlData))
            {
                try
                {
                    //wtf is going on here?
                    clientId = Convert.ToInt32(urlData.Substring(0, 4));
                    int month = (Convert.ToInt32(urlData.Substring(8)) - (clientId % 10)) / 3;
                    int year = (Convert.ToInt32(urlData.Substring(4, 4)) / 2) - clientId;
                    SelectedPeriod = new DateTime(year, month, 1);
                    period = SelectedPeriod;
                }
                catch
                {
                    Session.Abandon();
                    Response.Redirect(ServiceProvider.Current.Context.LoginUrl + "?Action=Exit");
                }
            }
        }

        private void UpdateTimeDepTables(DateTime period)
        {
            //What does this function do?
            DateTime sDate = period;
            DateTime eDate = sDate.AddMonths(1);

            //'the call to UpdateDataType is overly broad, but it would be hard to narrow its scope to match the user priv. And, it doesn't hurt.
            //'Wen: this will be called only when user want to see report that's in current month
            if ((sDate <= DateTime.Now.Date && eDate > DateTime.Now.Date) && !ContextBase.Updated())
            {
                WriteData wd = new WriteData();
                string[] types = { "Tool", "Room", "Store" };
                //wd.UpdateTable(types, 0, 0, UpdateDataType.CleanData | UpdateDataType.Data);
                using (StreamWriter file = new StreamWriter(File.OpenWrite(Request.PhysicalApplicationPath + "\\log.txt")))
                {
                    file.WriteLine("Time: " + DateTime.Now.ToString() + " Name: " + CurrentUser.DisplayName);
                    file.Close();
                }

                ContextBase.Updated(true);
            }
        }

        private void GetDataForAllDataGrids(DateTime period, int clientId)
        {
            // empty datagrids
            dgRoom.DataSource = null;
            dgRoom.DataBind();

            gvTool.DataSource = null;
            gvTool.DataBind();

            dgStore.DataSource = null;
            dgStore.DataBind();

            lblRoom.Visible = true;
            lblTool.Visible = true;
            lblStore.Visible = true;

            object sumCost;
            Compile compile = new Compile();

            //Room realted

            //2008-01-15
            //dtRoomCost has the following columns
            //0	"ClientID"	
            //1	"AccountID"	
            //2	"RoomID"	
            //3	"BillingType"	
            //4	"TotalCalcCost"	
            //5	"TotalEntries"	
            //6	"TotalHours"	

            DataTable dtRoomCost = compile.CalcCost("Room", string.Empty, string.Empty, 0, period, 0, clientId, Compile.AggType.CliAcctType);
            dtRoomCost.Columns.Add("Room", typeof(string));
            dtRoomCost.Columns.Add("Name", typeof(string));
            dtRoomCost.Columns.Add("BillingTypeName", typeof(string));
            dtRoomCost.Columns.Add("OrgName", typeof(string));
            dtRoomCost.Columns.Add("LineCost", typeof(double));
            dtRoomCost.Columns.Add("ShortCode", typeof(string));

            //Create the list to contain all summary total for each organization
            //List<UsageSummaryTotal> mylist = new List<UsageSummaryTotal>();

            DataTable SummaryTable = new DataTable();
            SummaryTable.Columns.Add("OrgID", typeof(int));
            SummaryTable.Columns.Add("OrgName", typeof(string));
            SummaryTable.Columns.Add("BillingTypeID", typeof(int));
            SummaryTable.Columns.Add("RoomTotal", typeof(double));
            SummaryTable.Columns.Add("ToolTotal", typeof(double));
            SummaryTable.Columns.Add("StoreTotal", typeof(double));

            //It's possible that the above code makes the table row count to 0.
            //If it's the case, we have to skip the code below, but why delete the only row?
            if (dtRoomCost.Rows.Count > 0)
            {
                //we have to get the total lab hours for monthly users because we have to find out the appropriate proportion of monthly fee distribution
                int currentBillingTypeId = dtRoomCost.Rows[0].Field<int>("BillingTypeID");
                decimal totalCleanRoomHours = 0; //this stores the total clean room hours for this user at this month
                decimal totalChemRoomHours = 0; //this stores the total chem room hours
                int[] specialBillingTypesForSomeUnknownReason = { BillingType.ExtAc_Ga, BillingType.ExtAc_Si, BillingType.Int_Si, BillingType.Int_Ga };
                if (specialBillingTypesForSomeUnknownReason.Contains(currentBillingTypeId))
                {
                    try
                    {
                        //2008-06-12 it's possible that user access only chem room and no clean room at all, so it will return DB Null if no clean room hours
                        totalCleanRoomHours = Convert.ToDecimal(dtRoomCost.Compute("SUM(TotalHours)", "RoomID = 6"));
                    }
                    catch
                    {
                        totalCleanRoomHours = 0;
                    }

                    try
                    {
                        //2008-06-12 it's possible that user access only chem room and no clean room at all, so it will return DB Null if no clean room hours
                        totalChemRoomHours = Convert.ToDecimal(dtRoomCost.Compute("SUM(TotalHours)", "RoomID = 25"));
                    }
                    catch
                    {
                        totalChemRoomHours = 0;
                    }
                }

                int previousOrgId = dtRoomCost.Rows[0].Field<int>("OrgID");
                int currentOrgId = previousOrgId;

                DataRow nr = SummaryTable.NewRow();
                nr["OrgID"] = previousOrgId;
                nr["OrgName"] = dsReport.Tables["Org"].Rows.Find(previousOrgId)["OrgName"];
                nr["BillingTypeID"] = dtRoomCost.Rows[0]["BillingType"];
                nr["RoomTotal"] = 0;
                nr["ToolTotal"] = 0;
                nr["StoreTotal"] = 0;
                SummaryTable.Rows.Add(nr);

                //**************** NAP room handling ******************
                //Get all active NAP Rooms with their costs, all chargetypes are returned
                //This is a temporary table, it's used to derive the really useful table below
                DataTable dtNAPRoomForAllChargeType = AppCode.BLL.RoomManager.GetAllNAPRoomsWithCosts(period);

                //filter out the chargetype so that we only have Internal costs with each NAP room
                //2009-04-05 the chartype id is difficult to get here, so we assume everyone is interanl.  This is okay, because we need to find out the percentage, not the actual cost
                DataRow[] drsNAPRoomForInternal = dtNAPRoomForAllChargeType.Select("ChargeTypeID = 5");

                //Loop through each room and find out this specified month's apportionment data.
                foreach (DataRow dr1 in drsNAPRoomForInternal)
                {
                    DataTable dtApportionData = RoomApportionDataManager.GetNAPRoomApportionDataByPeriod(period, dr1.Field<int>("RoomID"));

                    foreach (DataRow dr2 in dtApportionData.Rows)
                    {
                        DataRow[] drs = dtRoomCost.Select(string.Format("ClientID = {0} AND AccountID = {1} AND RoomID = {2}", dr2["ClientID"], dr2["AccountID"], dr2["RoomID"]));

                        if (drs.Length == 1)
                        {
                            drs[0].SetField("TotalCalcCost", dr2.Field<double>("Percentage") * dr1.Field<double>("RoomCost") / 100D);
                        }
                        else
                        {
                            //?
                        }
                    }

                    ////We now have the data of each room on this specific month, next we loop through the accounts
                    //foreach (DataRow dr2 in dtRoom.Rows)
                    //{
                    //    if (dr2.Field<int>("RoomID") == dr1.Field<int>("RoomID"))
                    //    {
                    //        dr2.SetField("TotalCalcCost", (drs[0].Field<double>("Percentage") * dr1.Field<double>("RoomCost")) / 100D);
                    //        DataRow[] drs = dtApportionData.Select();
                    //    }
                    //}
                }


                //**************** main loop to do the fee calculation ******
                decimal tempTotalHours = 0;
                foreach (DataRow dr in dtRoomCost.Rows)
                {
                    dr["Room"] = dsReport.Tables["Room"].Rows.Find(dr["RoomID"])["Room"];
                    dr["Name"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["Name"];
                    dr["ShortCode"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["ShortCode"];
                    DataRow drBillingType = dsReport.Tables["BillingType"].Rows.Find(dr["BillingType"]);
                    if (drBillingType != null)
                        dr["BillingTypeName"] = drBillingType["BillingTypeName"];
                    else
                        dr["BillingTypeName"] = "[nothing]";

                    dr["OrgName"] = dsReport.Tables["Org"].Rows.Find(dr["OrgID"])["OrgName"];

                    if (dr.Field<int>("RoomID") == 6)
                        tempTotalHours = totalCleanRoomHours;
                    else if (dr.Field<int>("RoomID") == 25)
                        tempTotalHours = totalChemRoomHours;

                    dr["LineCost"] = AppCode.BLL.BillingTypeManager.GetTotalCostByBillingType(dr.Field<int>("BillingType"), dr.Field<decimal>("TotalHours"), dr.Field<decimal>("TotalEntries"), dr.Field<Rooms>("RoomID"), dr.Field<decimal>("TotalCalcCost"), tempTotalHours);

                    currentOrgId = dr.Field<int>("OrgID");
                    if (previousOrgId != currentOrgId)
                    {
                        nr = SummaryTable.NewRow();
                        nr["OrgID"] = currentOrgId;
                        nr["OrgName"] = dr["OrgName"];
                        nr["BillingTypeID"] = dr["BillingType"];
                        SummaryTable.Rows.Add(nr);

                        previousOrgId = currentOrgId;
                    }
                }

                foreach (DataRow r in SummaryTable.Rows)
                    r["RoomTotal"] = dtRoomCost.Compute("SUM(LineCost)", string.Format("OrgID = {0}", r["OrgID"]));

                //Get total cost for this table
                sumCost = dtRoomCost.Compute("SUM(LineCost)", string.Empty);
                lblRoom.Text = string.Format("Total room usage fees: {0:C}", sumCost);
                lblRoom.ForeColor = System.Drawing.Color.Red;

                dgRoom.DataSource = dtRoomCost;
                dgRoom.DataBind();
            }
            else
            {
                lblRoom.Text = "No room usage during period";
                lblRoom.ForeColor = System.Drawing.Color.Red;

                foreach (DataRow r in SummaryTable.Rows)
                    r["RoomTotal"] = 0;
            }


            //***************** Tool related *******************************
            //2007-02-23 Must handle the exception here because if the user doesn't exist on that period, a error occur inside the CalcCost function
            DataTable dtToolCost = null;

            //0 ClientID
            //1 AccountID
            //2 ResourceID
            //3 IsStarted
            //4 TotalCalcCost
            //5 TotalUses
            //6 TotalActDuration

            try
            {
                dtToolCost = compile.CalcCost("Tool", string.Empty, string.Empty, 0, period, 0, clientId, Compile.AggType.CliAcctType);
            }
            catch
            {
                dtToolCost = null;
            }

            if (dtToolCost.Rows.Count > 0)
            {
                dtToolCost.Columns.Add("ResourceName", typeof(string));
                dtToolCost.Columns.Add("AccountName", typeof(string));
                dtToolCost.Columns.Add("OrgName", typeof(string));
                dtToolCost.Columns.Add("ShortCode", typeof(string));

                //Populate the two new columns, ResourceName and AccountName
                foreach (DataRow dr in dtToolCost.Rows)
                {
                    dr["ResourceName"] = dsReport.Tables["Resource"].Rows.Find(dr["ResourceID"])["ResourceName"];
                    dr["AccountName"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["Name"];
                    dr["ShortCode"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["ShortCode"];
                    dr["OrgName"] = dsReport.Tables["Org"].Rows.Find(dr["OrgID"])["OrgName"];
                }

                DataTable dtCloneActivated = dtToolCost.Clone();
                DataTable dtCloneCancelled = dtToolCost.Clone();
                DataTable dtCloneForgiven = dtToolCost.Clone();

                DataRow nr;
                foreach (DataRow dr in dtToolCost.Rows)
                {
                    if (dr.Field<int>("IsStarted") == 1)
                    {
                        if (dr.Field<int>("ToolChargeMultiplierMul") == 1)
                        {
                            nr = dtCloneActivated.NewRow();
                            nr["ResourceName"] = dr["ResourceName"];
                            nr["TotalUses"] = dr["TotalUses"];
                            nr["TotalActDuration"] = dr["TotalActDuration"];
                            nr["TotalCalcCost"] = dr["TotalCalcCost"];
                            nr["AccountName"] = dr["AccountName"];
                            nr["OrgName"] = dr["OrgName"];
                            nr["OrgID"] = dr["OrgID"];
                            dtCloneActivated.Rows.Add(nr);
                        }
                        else
                        {
                            //all forgiven records are collected here
                            nr = dtCloneForgiven.NewRow();
                            nr["ResourceName"] = dr["ResourceName"];
                            nr["TotalUses"] = dr["TotalUses"];
                            nr["TotalActDuration"] = dr["TotalActDuration"];
                            nr["TotalCalcCost"] = dr["TotalCalcCost"];
                            nr["AccountName"] = dr["AccountName"];
                            nr["OrgName"] = dr["OrgName"];
                            nr["OrgID"] = dr["OrgID"];
                            dtCloneForgiven.Rows.Add(nr);
                        }
                    }
                    else
                    {
                        nr = dtCloneCancelled.NewRow();
                        nr["ResourceName"] = dr["ResourceName"];
                        nr["TotalUses"] = dr["TotalUses"];
                        nr["TotalActDuration"] = dr["TotalActDuration"];
                        nr["TotalCalcCost"] = dr["TotalCalcCost"];
                        nr["AccountName"] = dr["AccountName"];
                        nr["OrgName"] = dr["OrgName"];
                        nr["OrgID"] = dr["OrgID"];
                        dtCloneCancelled.Rows.Add(nr);
                    }
                }

                //We claculate the total for each tables accordingly
                double totalToolCost = 0;

                if (dtCloneActivated.Rows.Count > 0)
                {
                    //to calculate the sum for activated too usage fee
                    sumCost = dtCloneActivated.Compute("SUM(TotalCalcCost)", string.Empty);
                    totalToolCost = Convert.ToDouble(sumCost);
                    lblActivatedToolFee.Text = string.Format("Sub Total: {0:C}", sumCost);

                    foreach (DataRow r in SummaryTable.Rows)
                        r["ToolTotal"] = dtCloneActivated.Compute("SUM(TotalCalcCost)", string.Format("OrgID = {0}", r["OrgID"]));
                }
                else
                    lblActivatedToolFee.Text = "Sub Total: $0";

                if (dtCloneCancelled.Rows.Count > 0)
                {
                    //to calculate the sum for cancelled too usage fee
                    sumCost = dtCloneCancelled.Compute("SUM(TotalCalcCost)", string.Empty);
                    totalToolCost += Convert.ToDouble(sumCost);
                    lblCancelledToolFee.Text = string.Format("Sub Total: {0:C}", sumCost);

                    foreach (DataRow r in SummaryTable.Rows)
                    {
                        object obj = dtCloneCancelled.Compute("SUM(TotalCalcCost)", string.Format("OrgID = {0}", r["OrgID"]));

                        if (obj != null && obj != DBNull.Value)
                            r["ToolTotal"] = r.Field<double>("ToolTotal") + Convert.ToDouble(obj);
                    }
                }
                else
                    lblCancelledToolFee.Text = "Sub Total: $0";

                if (dtCloneForgiven.Rows.Count > 0)
                {
                    sumCost = dtCloneForgiven.Compute("SUM(TotalCalcCost)", string.Empty);
                    lblForgivenToolFee.Text = string.Format("Sub Total: {0:$#,##0.00;($#,##0.00)}", sumCost);
                }
                else
                    lblForgivenToolFee.Text = "Sub Total: $0";

                gvTool.DataSource = dtCloneActivated;
                gvTool.DataBind();

                gvToolCancelled.DataSource = dtCloneCancelled;
                gvToolCancelled.DataBind();

                gvToolForgiven.DataSource = dtCloneForgiven;
                gvToolForgiven.DataBind();

                lblTool.Text = string.Format("Total tool usage fees: {0:C}", totalToolCost);
                //lblTool.Text += " (This doesn't include the fees that have been forgiven by tool engineers)";
                lblTool.ForeColor = System.Drawing.Color.Red;
                divTool.Visible = true;
            }
            else
            {
                lblTool.Text = "No tool usage during period";
                lblTool.ForeColor = System.Drawing.Color.Red;
                divTool.Visible = false;

                foreach (DataRow r in SummaryTable.Rows)
                    r["ToolTotal"] = 0;
            }

            //********************* Store related ***************************
            DataTable dtStoreCost = compile.CalcCost("StoreInv", string.Empty, string.Empty, 0, period, 0, clientId, Compile.AggType.None);
            dtStoreCost.Columns.Add("Item", typeof(string));
            dtStoreCost.Columns.Add("Name", typeof(string));
            dtStoreCost.Columns.Add("OrgName", typeof(string));
            dtStoreCost.Columns.Add("ShortCode", typeof(string));

            if (dtStoreCost.Rows.Count > 0)
            {
                foreach (DataRow dr in dtStoreCost.Rows)
                {
                    dr["Item"] = dsReport.Tables["Item"].Rows.Find(dr["ItemID"])["Item"];
                    dr["Name"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["Name"];
                    dr["ShortCode"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["ShortCode"];
                    dr["OrgName"] = dsReport.Tables["Org"].Rows.Find(dr["OrgID"])["OrgName"];
                }
                sumCost = dtStoreCost.Compute("SUM(CalcCost)", string.Empty);
                lblStore.Text = string.Format("Total store usage fees: {0:C}", sumCost);

                foreach (DataRow r in SummaryTable.Rows)
                {
                    r["StoreTotal"] = dtStoreCost.Compute("SUM(CalcCost)", string.Format("OrgID = {0}", r["OrgID"]));

                    if (r["StoreTotal"] == null || r["StoreTotal"] == DBNull.Value)
                        r["StoreTotal"] = 0.0;
                }

                dgStore.DataSource = dtStoreCost;
                dgStore.DataBind();

                lblStore.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblStore.Text = "No store usage during period";
                lblStore.ForeColor = System.Drawing.Color.Red;

                foreach (DataRow r in SummaryTable.Rows)
                    r["StoreTotal"] = 0.0;
            }

            dtStore2 = dtStoreCost;

            foreach (DataRow r in SummaryTable.Rows)
            {
                int billingTypeId = r.Field<int>("BillingTypeID");
                int[] specialBillingTypesForSomeUnknownReason = { BillingType.NonAc, BillingType.ExtAc_Ga, BillingType.ExtAc_Si, BillingType.Int_Ga, BillingType.Int_Si, BillingType.Other };
                if (specialBillingTypesForSomeUnknownReason.Contains(billingTypeId))
                    r["ToolTotal"] = 0.0;
            }

            dlSummary.DataSource = SummaryTable;
            dlSummary.DataBind();
        }

        private void BindPerUseBilling(DateTime period, int clientId)
        {
            gvRoom.DataSource = null;
            gvRoom.DataBind();

            //Create the list to contain all summary total for each organization
            //IList<UsageSummaryTotal> mylist = new List<UsageSummaryTotal>();

            DataTable SummaryTable = new DataTable();
            SummaryTable.Columns.Add("OrgID", typeof(int));
            SummaryTable.Columns.Add("OrgName", typeof(string));
            SummaryTable.Columns.Add("BillingTypeID", typeof(int));
            SummaryTable.Columns.Add("RoomTotal", typeof(double));
            SummaryTable.Columns.Add("ToolTotal", typeof(double));
            SummaryTable.Columns.Add("StoreTotal", typeof(double));

            double sumCost = 0.0;

            var dsReport = ContextBase.CacheData();

            gvRoom.DataSource = BillingManager.GetRoomCost(dsReport, period, clientId, SummaryTable, ref sumCost);
            gvRoom.DataBind();
            lblRoom2.Text = string.Format("Total lab usage fees: {0:C}", sumCost);

            DataTable dtCancelled = null;
            DataTable dtForgiven = null;

            gvTool2.DataSource = BillingManager.GetToolCost(dsReport, period, clientId, SummaryTable, ref sumCost, dtCancelled, dtForgiven);
            gvTool2.DataBind();

            gvToolCancelled2.DataSource = dtCancelled;
            gvToolCancelled2.DataBind();

            gvToolForgiven2.DataSource = dtForgiven;
            gvToolForgiven2.DataBind();

            double cancelledCost = 0;
            double forgivenCost = 0;

            if (dtCancelled == null || dtCancelled.Rows.Count == 0)
                cancelledCost = 0;
            else
                cancelledCost = Convert.ToDouble(dtCancelled.Compute("SUM(TotalCalcCost)", string.Empty));

            if (dtForgiven == null || dtForgiven.Rows.Count == 0)
                forgivenCost = 0;
            else
                forgivenCost = Convert.ToDouble(dtForgiven.Compute("SUM(TotalCalcCost)", string.Empty));

            lblTool2.Text = string.Format("Total tool usage fees: {0:C}", sumCost + cancelledCost);
            lblActivatedToolFee2.Text = string.Format("Sub Total: {0:C}", sumCost);
            lblCancelledToolFee2.Text = string.Format("Sub Total: {0:C}", cancelledCost);
            lblForgivenToolFee2.Text = string.Format("Sub Total: {0:$#,##0.00;($#,##0.00)}", forgivenCost);

            //Store
            //gvStore2.DataSource = BillingManager.GetStoreCost(period, clientId, SummaryTable, sumCost);
            //gvStore2.DataBind();
            //lblStore2.Text = string.Format("Total store usage fees: {0:C}", sumCost);
            if (dtStore2.Rows.Count > 0)
            {
                foreach (DataRow r in SummaryTable.Rows)
                {
                    r["StoreTotal"] = dtStore2.Compute("SUM(CalcCost)", string.Format("OrgID = {0}", r["OrgID"]));

                    if (r["StoreTotal"] == null || r["StoreTotal"] == DBNull.Value)
                        r["StoreTotal"] = 0.0;
                }

                gvStore2.DataSource = dtStore2;
                gvStore2.DataBind();
                object sumobj;

                //2009-08-05 it's possible that a user bought stuff but didn't use the lab at all
                sumobj = SummaryTable.Compute("SUM(StoreTotal)", string.Empty);
                if (sumobj == null || sumobj == DBNull.Value)
                {
                    sumCost = 0.0;
                    //no lab usage, only store usage
                    sumobj = dtStore2.Compute("SUM(CalcCost)", string.Empty);
                    if (sumobj != null && sumobj != DBNull.Value)
                        sumCost = Convert.ToDouble(sumobj);
                }
                else
                    sumCost = Convert.ToDouble(sumobj);

                lblStore2.Text = string.Format("Total store usage fees: {0:C}", sumCost);
            }
            else
            {
                lblStore2.Text = "No store usage during period";

                foreach (DataRow r in SummaryTable.Rows)
                    r["StoreTotal"] = 0.0;
            }

            dlSummary2.DataSource = SummaryTable;
            dlSummary2.DataBind();
        }

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
    }
}