using LNF;
using LNF.Billing;
using LNF.Data;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Linq;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class ToolBillingBL
    {
        // Call this after 2011-07-01

        public static IBillingTypeRepository BillingType => ServiceProvider.Current.Billing.BillingType;

        [Obsolete("Use ToolBillingBL.GetAggreateByTool instead.")]
        public static DataTable GetToolBillingDataByClientID20110701(HttpContextBase context, DateTime period, int clientId)
        {
            DataTable dt, dtUnStarted, dtCancelled;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
            {
                //Viewing current month data
                DataSet ds = ToolBillingDA.GetToolBillingTempDataByClientID20110701(period, clientId);
                dt = ds.Tables[0];
                dtUnStarted = ds.Tables[1];
                dtCancelled = ds.Tables[2];
            }
            else
            {
                //started reservations
                dt = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, BillingTableType.ToolBillingStarted);
                //unstarted reservations
                dtUnStarted = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, BillingTableType.ToolBillingUnStarted);
                dtCancelled = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, BillingTableType.ToolBillingCancelled);
            }

            dt.Columns.Add("TotalUnStartedUnusedDuration", typeof(double));
            dt.Columns.Add("ActivatedButUnused", typeof(double));
            dt.Columns.Add("ActualDisplay", typeof(double));
            dt.Columns.Add("LineCost", typeof(decimal));

            foreach (DataRow dr in dt.Rows)
            {
                int ResourceID = Convert.ToInt32(dr["ResourceID"]);

                //[2013-08-21 jg]
                //Added AccountID to the filter because otherwise we get two rows if the user has two unstarted reservations, using
                //two different accounts, on one resource. Then we end up in the 9999.99 "logically impossible" situation. I assume
                //that at some point we were aggregating to the resouce level but now we are at the resource/account level because
                //at this time both ResourceID and AccountID are in each DataTable object.
                int AccountID = Convert.ToInt32(dr["AccountID"]);

                DataRow[] rowsUnStarted = dtUnStarted.Select(string.Format("ResourceID = {0} AND AccountID = {1}", ResourceID, AccountID));
                DataRow[] rowsCancelled = dtCancelled.Select(string.Format("ResourceID = {0} AND AccountID = {1}", ResourceID, AccountID));

                if (rowsUnStarted.Length == 1)
                {
                    //For started reservations, the unused is the difference between charge duration and actual duration
                    double unusedButStarted = Convert.ToDouble(dr["TotalChargeDuration"]) - Convert.ToDouble(dr["TotalActDuration"]);
                    if (unusedButStarted < 0.0) unusedButStarted = 0;

                    dr["ActivatedButUnused"] = unusedButStarted;
                    dr["TotalUnStartedUnusedDuration"] = rowsUnStarted[0]["TotalChargeDuration"];
                    dr["TotalChargeDuration"] = Convert.ToDouble(dr["TotalChargeDuration"]) + Convert.ToDouble(rowsUnStarted[0]["TotalChargeDuration"]);
                    dr["TotalTransferredDuration"] = Convert.ToDouble(dr["TotalTransferredDuration"]) + Convert.ToDouble(rowsUnStarted[0]["TotalTransferredDuration"]);
                    dr["TotalForgivenDuration"] = Convert.ToDouble(dr["TotalForgivenDuration"]) + Convert.ToDouble(rowsUnStarted[0]["TotalForgivenDuration"]);
                    dr["TotalOverTime"] = Convert.ToDouble(dr["TotalOverTime"]) + Convert.ToDouble(rowsUnStarted[0]["TotalOverTime"]); //how can an unstarted resrevation have any overtime?
                    dr["UsageFeeCharged"] = Convert.ToDouble(dr["UsageFeeCharged"]) + Convert.ToDouble(rowsUnStarted[0]["UsageFeeCharged"]);
                    dr["OverTimePenaltyFee"] = Convert.ToDouble(dr["OverTimePenaltyFee"]) + Convert.ToDouble(rowsUnStarted[0]["OverTimePenaltyFee"]);
                    dr["BookingFee"] = Convert.ToDouble(dr["BookingFee"]) + Convert.ToDouble(rowsUnStarted[0]["BookingFee"]);
                    dr["ActualDisplay"] = Convert.ToDouble(dr["TotalActDuration"]) - Convert.ToDouble(dr["TotalOverTime"]);

                    rowsUnStarted[0].Delete();
                }
                else if (rowsUnStarted.Length == 0)
                {
                    dr["ActivatedButUnused"] = Convert.ToDouble(dr["TotalChargeDuration"]) - Convert.ToDouble(dr["TotalActDuration"]);
                    dr["TotalUnStartedUnusedDuration"] = 0;
                    dr["ActualDisplay"] = Convert.ToDouble(dr["TotalActDuration"]) - Convert.ToDouble(dr["TotalOverTime"]);
                }
                else
                {
                    //it's logically impossible to have more than 1 rows
                    dr["ActualDisplay"] = 9999.99;
                }

                if (rowsCancelled.Length == 1)
                {
                    dr["BookingFee"] = Convert.ToDouble(dr["BookingFee"]) + Convert.ToDouble(rowsCancelled[0]["BookingFee"]);
                    rowsCancelled[0].Delete();
                }
            }

            //We need to handle those unstarted reservations that has no started reservations in above loop
            if (dtUnStarted.Rows.Count > 0)
            {
                foreach (DataRow drUnstarted in dtUnStarted.Rows)
                {
                    if (drUnstarted.RowState == DataRowState.Unchanged)
                    {
                        DataRow ndr = dt.NewRow();
                        ndr.ItemArray = drUnstarted.ItemArray;
                        ndr["TotalUnStartedUnusedDuration"] = drUnstarted["TotalChargeDuration"];
                        ndr["ActualDisplay"] = 0; //because unstarted reservation never has actual usage hour
                        ndr["ActivatedButUnused"] = 0;
                        dt.Rows.Add(ndr);
                    }
                }
            }

            //We need to handle those cancelled reservations that has no started reservations in above loop
            if (dtCancelled.Rows.Count > 0)
            {
                foreach (DataRow drCancelled in dtCancelled.Rows)
                {
                    if (drCancelled.RowState == DataRowState.Unchanged)
                    {
                        DataRow ndr = dt.NewRow();
                        ndr.ItemArray = drCancelled.ItemArray;
                        ndr["TotalUnStartedUnusedDuration"] = 0; //because cancelled reservation has no unused time
                        ndr["ActualDisplay"] = 0; //because cancelled reservation never has actual usage hour
                        ndr["ActivatedButUnused"] = 0;
                        dt.Rows.Add(ndr);
                    }
                }
            }

            // this can't be used because the dt rows are aggreated by tool so there is no reservation info available and LineCostUtility needs this
            //LineCostUtility.CalculateToolLineCost(dt);

            foreach (DataRow dr in dt.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");
                bool isCancelledBeforeAllowedTime = dr.Field<bool>("IsCancelledBeforeAllowedTime");

                if (billingTypeId == BillingTypes.Other)
                    dr["LineCost"] = 0;
                else
                {
                    //Per Use types
                    if (!isCancelledBeforeAllowedTime)
                    {
                        double total = Convert.ToDouble(dr["UsageFeeCharged"]) + Convert.ToDouble(dr["OverTimePenaltyFee"]) + Convert.ToDouble(dr["BookingFee"]);
                        if (total > 0)
                            dr["LineCost"] = total;
                        else
                            dr["LineCost"] = 0;
                    }
                    else
                        dr["LineCost"] = dr["BookingFee"]; //Cancelled before two hours
                }

                //if the tool rate is 0, then everything should be 0
                double toolRate = Convert.ToDouble(dr["ResourceRate"]) + Convert.ToDouble(dr["PerUseRate"]);
                if (toolRate == 0) dr["LineCost"] = 0;
            }

            dt.Columns["Room"].ColumnName = "RoomName";
            dt.Columns["ActualDisplay"].ColumnName = "ActivatedUsed";
            dt.Columns["ActivatedButUnused"].ColumnName = "ActivatedUnused";
            dt.Columns["TotalUnStartedUnusedDuration"].ColumnName = "UnstartedUnused";
            dt.Columns["Name"].ColumnName = "AccountName";

            return dt;
        }

        // Call this function if period is between 2011-04-01 and 2011-06-01
        public static DataSet GetToolBillingDataByClientID20110401(HttpContextBase context, DateTime period, int clientId)
        {
            DataSet ds = null;
            DataTable dt1, dt2, dt3;
            DateTime CutOffDate20110401 = new DateTime(2011, 4, 1);

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
            {
                //Viewing current month data
                ds = ToolBillingDA.GetToolBillingTempDataByClientID(period, clientId);
            }
            else
            {
                ds = new DataSet();

                BillingTableType t1, t2, t3;
                if (period < CutOffDate20110401)
                {
                    t1 = BillingTableType.ToolBillingActivated;
                    t2 = BillingTableType.ToolBillingUncancelled;
                    t3 = BillingTableType.ToolBillingForgiven;
                }
                else
                {
                    t1 = BillingTableType.ToolBilling20110401Reservations;
                    t2 = BillingTableType.ToolBilling20110401Cancelled;
                    t3 = BillingTableType.ToolBilling20110401Forgiven;
                }

                dt1 = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, t1);
                dt2 = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, t2);
                dt3 = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, t3);
                ds.Tables.Add(dt1.Copy());
                ds.Tables.Add(dt2.Copy());
                ds.Tables.Add(dt3.Copy());
            }

            if (period < CutOffDate20110401)
            {
                //Part I : calculate the true Line Cost based on billing type, reservation status
                foreach (DataTable dt in ds.Tables)
                {
                    dt.Columns.Add("LineCost", typeof(double));
                    foreach (DataRow dr in dt.Rows)
                    {
                        int billingTypeId = dr.Field<int>("BillingTypeID");
                        var room = Rooms.All().FirstOrDefault(x => x.RoomID == dr.Field<int>("RoomID"));
                        bool isStarted = dr.Field<bool>("IsStarted");

                        if (BillingTypes.IsMonthlyUserBillingType(billingTypeId))
                        {
                            if (period >= new DateTime(2010, 7, 1))
                            {
                                //Monthly User, so we have flat fee for clean room
                                if (room == Rooms.CleanRoom)
                                {
                                    if (dr.Field<int>("ResourceID") == 56000)
                                    {
                                        if (isStarted)
                                            dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("ReservationFee");
                                        else
                                            dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee");
                                    }
                                    else
                                        dr["LineCost"] = 0;
                                }
                                else
                                {
                                    if (isStarted)
                                        dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("ReservationFee");
                                    else
                                        dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee");
                                }
                            }
                            else
                            {
                                //Monthly User, so we have flat fee for clean room
                                if (room == Rooms.CleanRoom)
                                {
                                    if (dr.Field<int>("ResourceID") == 56000)
                                    {
                                        if (isStarted)
                                            dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("ReservationFee");
                                        else
                                            dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee");
                                    }
                                    else
                                        dr["LineCost"] = 0;
                                }
                                else
                                {
                                    if (isStarted)
                                        dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("ReservationFee");
                                    else
                                        dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee");
                                }
                            }
                        }
                        else if (billingTypeId == BillingTypes.Other)
                            dr["LineCost"] = 0;
                        else
                        {
                            if (period >= new DateTime(2010, 7, 1))
                            {
                                //Per Use types
                                if (isStarted)
                                    dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("ReservationFee");
                                else
                                    dr["LineCost"] = dr["UncancelledPenaltyFee"];
                            }
                            else
                            {
                                if (isStarted)
                                    dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("ReservationFee");
                                else
                                    dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee");
                            }
                        }

                        //if the tool rate is 0, then everything should be 0
                        bool hasPerUseRateColumn = dt.Columns.Contains("PerUseRate");
                        var resourceRate = dr.Field<decimal>("ResourceRate");
                        var perUserRate = hasPerUseRateColumn ? dr.Field<decimal>("PerUseRate") : 0M;

                        if (resourceRate + perUserRate == 0)
                            dr["LineCost"] = 0;
                    }
                }
            }
            else
            {
                //2011-04-01
                foreach (DataTable dt in ds.Tables)
                {
                    dt.Columns.Add("LineCost", typeof(double));
                    foreach (DataRow dr in dt.Rows)
                    {
                        int billingTypeId = dr.Field<int>("BillingTypeID");
                        bool isCancelledBeforeAllowedTime = dr.Field<bool>("IsCancelledBeforeAllowedTime");

                        if (billingTypeId == BillingTypes.Other)
                            dr["LineCost"] = 0;
                        else
                        {
                            if (period >= new DateTime(2010, 7, 1))
                            {
                                //Per Use types
                                if (!isCancelledBeforeAllowedTime)
                                    dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("BookingFee");
                                else
                                    dr["LineCost"] = dr.Field<decimal>("BookingFee"); //Cancelled before two hours
                            }
                        }

                        //if the tool rate is 0, then everything should be 0
                        decimal rateTotal = dr.Field<decimal>("ResourceRate") + dr.Field<decimal>("PerUserRate");
                        if (rateTotal == 0)
                            dr["LineCost"] = 0;
                    }
                }
            }

            return ds;
        }

        // Oldest version of getting tool data, used by Oldest UserUsageSummary
        public static DataSet GetToolBillingDataByClientID_Old(DateTime period, int clientId)
        {
            DataSet ds;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
            {
                //Viewing current month data
                ds = ToolBillingDA.GetToolBillingTempDataByClientID(period, clientId);
            }
            else
                ds = ToolBillingDA.GetToolBillingDataByClientID(period, clientId);

            //Part I : calculate the true Line Cost based on billing type, reservation status
            foreach (DataTable dt in ds.Tables)
            {
                dt.Columns.Add("LineCost", typeof(double));
                foreach (DataRow dr in dt.Rows)
                {
                    int billingTypeId = dr.Field<int>("BillingTypeID");
                    int roomId = dr.Field<int>("RoomID");
                    bool isStarted = dr.Field<bool>("IsStarted");

                    if (billingTypeId == BillingTypes.Other)
                        dr["LineCost"] = 0;
                    else
                    {
                        //Per Use types
                        if (isStarted)
                            dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("ReservationFee");
                        else
                            dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee");
                    }

                    //if the tool rate is 0, then everything should be 0
                    decimal rateTotal = dr.Field<decimal>("ResourceRate") + dr.Field<decimal>("PerUseRate");
                    if (rateTotal == 0)
                        dr["LineCost"] = 0;
                }
            }

            return ds;
        }
    }
}
