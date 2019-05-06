using LNF.Cache;
using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Data;
using System.Linq;
using LNF.Web;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class BillingManager
    {
        //Get all users' ClientID who had accessed to the lab in any specific month
        public static DataTable GetMonthlyClientID(DateTime period)
        {
            return DA.Command()
                .Param("Action", "GetAllUsersDuringSpecificMonth")
                .Param("Period", period)
                .FillDataTable("dbo.RoomData_Select");
        }

        public static DataTable GetRoomCost(DataSet dsReport, DateTime period, int clientId, DataTable dtSummary, ref double sumCost)
        {
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

            //DataSet dsReport = context.CacheData();

            Compile compile = new Compile();
            DataTable dtRoomCost = compile.CalcCost2("Room", string.Empty, string.Empty, 0, period, 0, clientId, Compile.AggType.CliAcctType);

            dtRoomCost.Columns.Add("Room", typeof(string));
            dtRoomCost.Columns.Add("Name", typeof(string));
            dtRoomCost.Columns.Add("BillingTypeName", typeof(string));
            dtRoomCost.Columns.Add("OrgName", typeof(string));
            dtRoomCost.Columns.Add("LineCost", typeof(double));
            dtRoomCost.Columns.Add("ShortCode", typeof(string));

            //It's possible that the above code makes the table row count to 0.
            //If it's the case, we have to skip the code below, but why delete the only row?
            if (dtRoomCost.Rows.Count > 0)
            {
                //we have to get the total lab hours for monthly users because we have to find out the appropriate proportion of monthly fee distribution
                int currentBillingTypeId = dtRoomCost.Rows[0].Field<int>("BillingType");

                double totalHours = 0;
                int[] specialBillingTypesForSomeUnknownReason = { BillingType.ExtAc_Ga, BillingType.ExtAc_Si, BillingType.Int_Si, BillingType.Int_Ga };
                if (specialBillingTypesForSomeUnknownReason.Contains(currentBillingTypeId))
                {
                    try
                    {
                        //2008-06-12 it's possible that user access only chem room and no clean room at all, so it will return DB Null if no clean room hours
                        totalHours = Convert.ToDouble(dtRoomCost.Compute("SUM(TotalHours)", "RoomID = 6")); //special code again
                    }
                    catch
                    {
                        totalHours = 0;
                    }
                }

                int previousOrgID = Convert.ToInt32(dtRoomCost.Rows[0]["OrgID"]);
                int currentOrgID = previousOrgID;

                DataRow ndr = dtSummary.NewRow();
                ndr["OrgID"] = previousOrgID;
                ndr["OrgName"] = dsReport.Tables["Org"].Rows.Find(previousOrgID)["OrgName"];
                ndr["BillingTypeID"] = dtRoomCost.Rows[0]["BillingType"];
                ndr["RoomTotal"] = 0;
                ndr["ToolTotal"] = 0;
                ndr["StoreTotal"] = 0;
                dtSummary.Rows.Add(ndr);

                //Get all active NAP Rooms with their costs, all chargetypes are returned
                //This is a temporary table, it's used to derive the really useful table below
                DataTable dtNAPRoomForAllChargeType = RoomManager.GetAllNAPRoomsWithCosts(period);

                //filter out the chargetype so that we only have Internal costs with each NAP room
                //2009-04-05 the chartype id is difficult to get here, so we assume everyone is interanl.  This is okay, because we need to find out the percentage, not the actual cost
                DataRow[] drsNAPRoomForInternal = dtNAPRoomForAllChargeType.Select("ChargeTypeID = 5");

                //Loop through each room and find out this specified month's apportionment data.
                foreach (DataRow dr1 in drsNAPRoomForInternal)
                {
                    DataTable dtApportionData = RoomApportionDataManager.GetNAPRoomApportionDataByPeriod(period, Convert.ToInt32(dr1["RoomID"]));

                    foreach (DataRow dr2 in dtApportionData.Rows)
                    {
                        DataRow[] drs = dtRoomCost.Select(string.Format("ClientID = {0} AND AccountID = {1} AND RoomID = {2}", dr2["ClientID"], dr2["AccountID"], dr2["RoomID"]));
                        if (drs.Length == 1)
                            drs[0]["TotalCalcCost"] = Convert.ToDouble(dr2["Percentage"]) * Convert.ToDouble(dr1["RoomCost"]) / 100D;
                    }
                }

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

                    currentOrgID = Convert.ToInt32(dr["OrgID"]);
                    if (previousOrgID != currentOrgID)
                    {
                        ndr = dtSummary.NewRow();
                        ndr["OrgID"] = currentOrgID;
                        ndr["OrgName"] = dr["OrgName"];
                        ndr["BillingTypeID"] = dr["BillingType"];
                        dtSummary.Rows.Add(ndr);

                        previousOrgID = currentOrgID;
                    }
                }

                foreach (DataRow r in dtSummary.Rows)
                    r["RoomTotal"] = dtRoomCost.Compute("SUM(TotalCalcCost)", string.Format("OrgID = {0}", r["OrgID"]));

                //Get total cost for this table
                sumCost = Convert.ToDouble(dtRoomCost.Compute("SUM(TotalCalcCost)", string.Empty));

                return dtRoomCost;
            }
            else
            {
                foreach (DataRow r in dtSummary.Rows)
                    r["RoomTotal"] = 0;

                sumCost = 0.0;

                return null;
            }
        }

        public static DataTable GetToolCost(DataSet dsReport, DateTime period, int clientId, DataTable dtSummary, ref double sumCost, DataTable dtCloneCancelled, DataTable dtCloneForgiven)
        {
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
            //DataSet dsReport = context.CacheData();

            Compile compile = new Compile();
            dtToolCost = compile.CalcCost2("Tool", string.Empty, string.Empty, 0, period, 0, clientId, Compile.AggType.CliAcctType);

            if (dtToolCost.Rows.Count > 0)
            {
                dtToolCost.Columns.Add("ResourceName", typeof(string));
                dtToolCost.Columns.Add("AccountName", typeof(string));
                dtToolCost.Columns.Add("ShortCode", typeof(string));
                dtToolCost.Columns.Add("OrgName", typeof(string));

                //Populate the two new columns, ResourceName and AccountName
                foreach (DataRow dr in dtToolCost.Rows)
                {
                    dr["ResourceName"] = dsReport.Tables["Resource"].Rows.Find(dr["ResourceID"])["ResourceName"];
                    dr["AccountName"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["Name"];
                    dr["ShortCode"] = dsReport.Tables["Account"].Rows.Find(dr["AccountID"])["ShortCode"];
                    dr["OrgName"] = dsReport.Tables["Org"].Rows.Find(dr["OrgID"])["OrgName"];
                }

                DataTable dtCloneActivated = dtToolCost.Clone();
                dtCloneCancelled = dtToolCost.Clone();
                dtCloneForgiven = dtToolCost.Clone();

                DataRow ndr;
                foreach (DataRow dr in dtToolCost.Rows)
                {
                    if (Convert.ToBoolean(dr["IsStarted"]))
                    {
                        if (Convert.ToDouble(dr["ToolChargeMultiplierMul"]) == 1)
                        {
                            ndr = dtCloneActivated.NewRow();
                            ndr["ResourceName"] = dr["ResourceName"];
                            ndr["TotalUses"] = dr["TotalUses"];
                            ndr["TotalActDuration"] = dr["TotalActDuration"];
                            ndr["TotalCalcCost"] = dr["TotalCalcCost"];
                            ndr["AccountName"] = dr["AccountName"];
                            ndr["OrgName"] = dr["OrgName"];
                            ndr["OrgID"] = dr["OrgID"];
                            dtCloneActivated.Rows.Add(ndr);
                        }
                        else
                        {
                            //all forgiven records are collected here
                            ndr = dtCloneForgiven.NewRow();
                            ndr["ResourceName"] = dr["ResourceName"];
                            ndr["TotalUses"] = dr["TotalUses"];
                            ndr["TotalActDuration"] = dr["TotalActDuration"];
                            ndr["TotalCalcCost"] = dr["TotalCalcCost"];
                            ndr["AccountName"] = dr["AccountName"];
                            ndr["OrgName"] = dr["OrgName"];
                            ndr["OrgID"] = dr["OrgID"];
                            dtCloneForgiven.Rows.Add(ndr);
                        }
                    }
                    else
                    {
                        ndr = dtCloneCancelled.NewRow();
                        ndr["ResourceName"] = dr["ResourceName"];
                        ndr["TotalUses"] = dr["TotalUses"];
                        ndr["TotalActDuration"] = dr["TotalActDuration"];
                        ndr["TotalCalcCost"] = dr["TotalCalcCost"];
                        ndr["AccountName"] = dr["AccountName"];
                        ndr["OrgName"] = dr["OrgName"];
                        ndr["OrgID"] = dr["OrgID"];
                        dtCloneCancelled.Rows.Add(ndr);
                    }
                }

                //We claculate the total for each tables accordingly
                double tempSumCost = 0.0;

                if (dtCloneActivated.Rows.Count > 0)
                {
                    //to calculate the sum for activated too usage fee
                    tempSumCost = Convert.ToDouble(dtCloneActivated.Compute("SUM(TotalCalcCost)", string.Empty));
                    sumCost = tempSumCost;

                    foreach (DataRow r in dtSummary.Rows)
                    {
                        r["ToolTotal"] = dtCloneActivated.Compute("SUM(TotalCalcCost)", string.Format("OrgID = {0}", r["OrgID"]));
                        if (r["ToolTotal"] == DBNull.Value) r["ToolTotal"] = 0.0;
                    }
                }
                else
                    sumCost = 0;

                if (dtCloneCancelled.Rows.Count > 0)
                {
                    foreach (DataRow r in dtSummary.Rows)
                    {
                        object obj = dtCloneCancelled.Compute("SUM(TotalCalcCost)", string.Format("OrgID = {0}", r["OrgID"]));
                        if (obj != null && obj != DBNull.Value)
                            r["ToolTotal"] = Convert.ToDouble(r["ToolTotal"]) + Convert.ToDouble(obj);
                    }
                }

                return dtCloneActivated;
            }
            else
            {
                foreach (DataRow r in dtSummary.Rows)
                    r["ToolTotal"] = 0;

                sumCost = 0.0;

                return null;
            }
        }
    }
}
