using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class AggFeeComparison : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int CalculateMonthlyFeePerUser(int clientId, DateTime period, int currentBillingTypeId, DataTable dtAccount, ref decimal totalRoomSum, ref decimal totalRoomPerUseSum, ref decimal totalToolSum)
        {
            int billingTypeId = currentBillingTypeId;

            Compile compile = new Compile();
            decimal tempMonthSum = 0;

            //************* Room ************
            DataTable dtRawRoomCost = compile.CalcCost2("Room", "", "", 0, period, 0, clientId, Compile.AggType.CliAcctType);

            //dtRoomCost columns
            //ClientID
            //AccountID
            //RoomID
            //BillingType
            //TotalCalcCost
            //TotalEntries
            //TotalHours	

            //create the real table that will filter out accounts that are not in this specific org
            DataTable dtRoomCost = dtRawRoomCost.Clone();

            foreach (DataRow dr in dtRawRoomCost.Rows)
            {
                DataRow[] drs = dtAccount.Select(string.Format("AccountID = {0}", dr["AccountID"]));
                if (drs.Length >= 1)
                {
                    DataRow ndr = dtRoomCost.NewRow();
                    ndr["ClientID"] = dr["ClientID"];
                    ndr["AccountID"] = dr["AccountID"];
                    ndr["RoomID"] = dr["RoomID"];
                    ndr["BillingType"] = dr["BillingType"];
                    ndr["TotalCalcCost"] = dr["TotalCalcCost"];
                    ndr["TotalEntries"] = dr["TotalEntries"];
                    ndr["TotalHours"] = dr["TotalHours"];

                    dtRoomCost.Rows.Add(ndr);
                }
            }

            //at this moment, dtRoomCost has the clean data with only record belong to this org (and this manager)
            dtRoomCost.Columns.Add("LineCost", typeof(double)); //this will hold the current cost that users really pay to the lab

            if (dtRoomCost.Rows.Count > 0)
            {
                billingTypeId = dtRoomCost.Rows[0].Field<int>("BillingType");

                if (billingTypeId != BillingType.Other)
                {
                    //first, we have to get the total hours for people who are not pay by hours - we will use the "total hours" to find out the correct apportion for each account
                    decimal totalCleanRoomHours = 0; //this stores the total clean room hours for this user at this month
                    decimal totalChemRoomHours = 0; //this stores the total chem room hours

                    int[] specialBillingTypesForSomeUnknownReason = { BillingType.ExtAc_Ga, BillingType.ExtAc_Si, BillingType.Int_Si, BillingType.Int_Ga };
                    if (specialBillingTypesForSomeUnknownReason.Contains(billingTypeId))
                    {
                        try
                        {
                            //2008-06-12 it's possible that user access only chem room and no clean room at all, so it will return DBNull if no clean room hours
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

                    decimal tempTotalHours = 0;
                    foreach (DataRow dr in dtRoomCost.Rows)
                    {
                        Rooms room = RoomUtility.GetRoom(dr.Field<int>("RoomID"));
                        if (room == Rooms.CleanRoom)
                            tempTotalHours = totalCleanRoomHours;
                        else if (room == Rooms.WetChemistry)
                            tempTotalHours = totalChemRoomHours;

                        //if the current user is not using the Hourly rate, we have to calcuate it
                        int[] billingTypesThatDoNotUseHourlyRate = { BillingType.ExtAc_Ga, BillingType.ExtAc_Si, BillingType.Int_Si, BillingType.Int_Ga, BillingType.ExtAc_Tools, BillingType.Int_Tools };
                        if (billingTypesThatDoNotUseHourlyRate.Contains(billingTypeId))
                            dr["LineCost"] = BillingTypeManager.GetTotalCostByBillingType(billingTypeId, dr.Field<decimal>("TotalHours"), dr.Field<decimal>("TotalEntries"), room, dr.Field<decimal>("TotalCalcCost"), tempTotalHours);
                        else if (billingTypeId == BillingType.Other)
                            dr["LineCost"] = 0;

                        //for some reasons, the TotalCalcCost column is never filled for DC lab and Chem room, so we have to add it manually here
                        if (room != Rooms.CleanRoom)
                        {
                            if (billingTypeId == BillingType.Other)
                                dr["TotalCalcCost"] = 0;
                        }
                    }

                    //Get total cost for this table
                    //First, we have to collect the per use amount no matter which billing type the user has
                    tempMonthSum = Convert.ToDecimal(dtRoomCost.Compute("SUM(TotalCalcCost)", string.Empty));
                    //we add the "ideal" cost to the totalRoomPerUseSum
                    totalRoomPerUseSum = tempMonthSum;

                    //if it's non-per usage type, we must also calculate it's current cost
                    int[] billingTypesThatAreNotPerUse = { BillingType.ExtAc_Ga, BillingType.ExtAc_Si, BillingType.Int_Si, BillingType.Int_Ga, BillingType.ExtAc_Tools, BillingType.Int_Tools };
                    if (billingTypesThatAreNotPerUse.Contains(billingTypeId))
                        tempMonthSum = Convert.ToDecimal(dtRoomCost.Compute("SUM(LineCost)", string.Empty));

                    ///we add the "real" cost to the totalRoomSum
                    totalRoomSum = tempMonthSum;
                }
            }

            //***************Tool ****************
            //Get tool usage tables and the sum of the fee, then add to the total
            DataTable dtRawToolCost = compile.CalcCost2("Tool", string.Empty, string.Empty, 0, period, 0, clientId, Compile.AggType.CliAcctType);

            //again, filter out accoutns that are not assoicated with the org
            DataTable dtToolCost = dtRawToolCost.Clone();

            foreach (DataRow dr in dtRawToolCost.Rows)
            {
                DataRow[] drs = dtAccount.Select(string.Format("AccountID = {0}", dr["AccountID"]));
                if (drs.Length >= 1)
                {
                    DataRow ndr = dtToolCost.NewRow();
                    ndr[0] = dr[0];
                    ndr[1] = dr[1];
                    ndr[2] = dr[2];
                    ndr[3] = dr[3];
                    ndr[4] = dr[4];
                    ndr[5] = dr[5];
                    ndr[6] = dr[6];
                    dtToolCost.Rows.Add(ndr);
                }
            }

            if (billingTypeId != BillingType.Other)
            {
                //at this moment, dtToolCost has clean records
                if (dtToolCost.Rows.Count > 0)
                {
                    tempMonthSum = Convert.ToDecimal(dtToolCost.Compute("SUM(TotalCalcCost)", string.Empty));
                    totalToolSum = tempMonthSum;
                }
            }

            //show billing type ONLY when calculating for one person
            if (ddlUser.SelectedValue != "0")
                lblBillingType.Text = "Billing type = " + DA.Current.Single<BillingType>(billingTypeId).BillingTypeName;
            else
                lblBillingType.Text = "All users are selected";

            return billingTypeId;
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            DateTime period = pp1.SelectedPeriod;
            int totalMonths = Convert.ToInt32(txtNumMonths.Text);
            int currentBillingTypeId = BillingType.Other;

            decimal totalRoomSum = 0;
            decimal totalRoomPerUseSum = 0;
            decimal totalToolSum = 0;

            decimal finalRoomSum = 0;
            decimal finalToolSum = 0;
            decimal finalFutureRoomSum = 0;
            decimal finalFutureToolSum = 0;

            //2008-11-04 For non Academics, we need to reduce the hour rate from current $77 to about $15 dollar, so the mutiplier ratio here is 15/77 = 0.19
            decimal nonAcFutureRoomMultiplier = 0.19M; //WTF IS THIS????

            DataTable dtAllUsers = new DataTable();
            dtAllUsers.Columns.Add("Name", typeof(string));
            dtAllUsers.Columns.Add("CurrentPayment", typeof(double));
            dtAllUsers.Columns.Add("FuturePayment", typeof(double));

            //we need a table that contains all the accounts associated with this manager
            //people like Ning belongs to more than 1 org, so we have to filter out the accounts that are not in this particular org
            DataTable dtAccount = ClientDA.GetAllAccountsByClientOrgID(Convert.ToInt32(ddlManager.SelectedValue));

            int[] billingTypesChargedByToolUsage = { BillingType.ExtAc_Hour, BillingType.Int_Hour, BillingType.NonAc_Hour, BillingType.ExtAc_Tools, BillingType.Int_Tools, BillingType.NonAc_Tools };
            int[] specialBillingTypesForSomeUnknownReason = { BillingType.NonAc, BillingType.NonAc_Hour, BillingType.NonAc_Tools };

            if (ddlUser.SelectedValue == "0")
            {
                //We have to caluclate ALL users belong to this manager
                foreach (ListItem userItem in ddlUser.Items)
                {
                    if (userItem.Value != "0") //we must exclude the "All" listItem
                    {
                        DataRow ndr = dtAllUsers.NewRow();
                        DateTime p = period;
                        for (int i = 1; i <= totalMonths; i++)
                        {
                            totalRoomSum = 0;
                            totalRoomPerUseSum = 0;
                            totalToolSum = 0;

                            currentBillingTypeId = CalculateMonthlyFeePerUser(Convert.ToInt32(userItem.Value), p, currentBillingTypeId, dtAccount, ref totalRoomSum, ref totalRoomPerUseSum, ref totalToolSum);

                            finalRoomSum += totalRoomSum;

                            //addl tool cost for those users who are charged by tools usage now (hours and tools)
                            if (billingTypesChargedByToolUsage.Contains(currentBillingTypeId))
                                finalToolSum += totalToolSum;

                            if (specialBillingTypesForSomeUnknownReason.Contains(currentBillingTypeId))
                            {
                                //2008-11-04 For non Academics, we need to reduce the hour rate
                                totalRoomPerUseSum *= nonAcFutureRoomMultiplier;
                            }

                            finalFutureRoomSum += totalRoomPerUseSum;
                            finalFutureToolSum += totalToolSum;

                            ndr["Name"] = userItem.Text;

                            //for per usage types (such as xxx_hour or xxx_tool, we need to include the tool fee for current payment
                            int[] billingTypesForPerUsage = { BillingType.ExtAc_Hour, BillingType.Int_Hour, BillingType.NonAc_Hour, BillingType.ExtAc_Tools, BillingType.Int_Tools, BillingType.NonAc_Tools };
                            if (billingTypesForPerUsage.Contains(currentBillingTypeId))
                                ndr["CurrentPayment"] = Math.Round(totalRoomSum, 2) + Math.Round(totalToolSum, 2);
                            else
                            {
                                //the users are montly flat room billing type
                                ndr["CurrentPayment"] = Math.Round(totalRoomSum, 2);
                            }

                            ndr["FuturePayment"] = Math.Round((totalRoomPerUseSum + totalToolSum), 2);

                            //If there is no usage for this account, then don't show it
                            if (ndr.Field<double>("CurrentPayment") != 0 && ndr.Field<double>("FuturePayment") != 0)
                                dtAllUsers.Rows.Add(ndr);

                            //remember to add one month at the very end
                            p = p.AddMonths(1);
                        }
                    }
                }
            }
            else
            {
                DateTime p = period;
                for (int i = 1; i <= totalMonths; i++)
                {
                    totalRoomSum = 0;
                    totalRoomPerUseSum = 0;
                    totalToolSum = 0;

                    currentBillingTypeId = CalculateMonthlyFeePerUser(Convert.ToInt32(ddlUser.SelectedValue), p, currentBillingTypeId, dtAccount, ref totalRoomSum, ref totalRoomPerUseSum, ref totalToolSum);

                    finalRoomSum += totalRoomSum;

                    //addl tool cost for those users who are charged by tools usage now (hours and tools)
                    if (billingTypesChargedByToolUsage.Contains(currentBillingTypeId))
                        finalToolSum += totalToolSum;

                    if (specialBillingTypesForSomeUnknownReason.Contains(currentBillingTypeId))
                    {
                        //2008-11-04 For non Academics, we need to reduce the hour rate
                        totalRoomPerUseSum *= nonAcFutureRoomMultiplier;
                    }

                    finalFutureRoomSum += totalRoomPerUseSum;
                    finalFutureToolSum += totalToolSum;

                    //remember to add one month at the very end
                    p = p.AddMonths(1);
                }
            }

            lblRoomCost.Text = string.Format("Current Room Cost (plus entry fee): {0:$#,##0.00}", finalRoomSum);
            lblToolCost.Text = string.Format("Current Tool Cost (if applies): {0:$#,##0.00}", finalToolSum);
            lblCurrentRoomCost.Text = string.Format("Current payment: {0:$#,##0.00}", finalRoomSum + finalToolSum);

            lblPerUseRoomCost.Text = string.Format("Future Per Usage Room Cost: {0:$#,##0.00}", finalFutureRoomSum);
            lblPerUseToolCost.Text = string.Format("Future Per Usage Tool Cost: {0:$#,##0.00}", finalFutureToolSum);

            lblRealCost.Text = string.Format("Real Cost (Room + Tool): {0:$#,##0.00}", finalFutureRoomSum + finalFutureToolSum);

            gvAllUsers.DataSource = dtAllUsers;
            gvAllUsers.DataBind();
        }
    }
}