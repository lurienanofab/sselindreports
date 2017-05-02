using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Repository.Billing;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingBL
    {
        public static DataTable GetRoomBillingDataByClientID(DateTime period, int clientId)
        {
            DataTable dt;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
                dt = RoomBillingDA.GetRoomBillingTempDataByClientID(period, clientId);
            else
            {
                if (period >= new DateTime(2011, 10, 1))
                    dt = BillingTablesBL.GetMultileTables20110701(period.Year, period.Month, clientId, BillingTableType.RoomBilling);
                else
                    dt = BillingTablesBL.GetMultileTables(period.Year, period.Month, clientId, BillingTableType.RoomBilling);
            }

            dt.Columns.Add("DailyFee", typeof(decimal));
            dt.Columns.Add("EntryFee", typeof(decimal));
            dt.Columns.Add("LineCost", typeof(decimal));

            LineCostUtility.CalculateRoomLineCost(dt);

            ////Part I: Get the true cost based on billing types
            //foreach (DataRow dr in dt.Rows)
            //{
            //    BillingType billingType = DA.Current.Single<BillingType>(dr.Field<int>("BillingTypeID"));
            //    Rooms room = Room.GetRoom(dr.Field<int>("RoomID"));

            //    dr.SetField("Room", Room.GetRoomDisplayName(dr.Field<int>("RoomID")));

            //    if (billingType.IsMonthlyUserBillingType())
            //    {
            //        //Monthly User, so we have flat fee
            //        if (room == Rooms.CleanRoom)
            //            dr["LineCost"] = dr.Field<decimal>("MonthlyRoomCharge");
            //        else
            //            dr["LineCost"] = dr.Field<decimal>("RoomCharge") + dr.Field<decimal>("EntryCharge");
            //    }
            //    else if (billingType.IsGrowerUserBillingType())
            //    {
            //        if (room == Rooms.OrganicsBay)
            //        {
            //            //Organics bay must be charged for growers as well
            //            dr["LineCost"] = dr["RoomCharge"];
            //        }
            //        else
            //            dr["LineCost"] = dr.Field<decimal>("AccountDays") * dr.Field<decimal>("RoomRate") + dr.Field<decimal>("EntryCharge");

            //    }
            //    else if (billingType == BillingType.Other)
            //        dr["LineCost"] = 0;
            //    else if (billingType == BillingType.Grower_Observer)
            //    {
            //        dr["LineCost"] = Convert.ToDouble(dr["RoomCharge"]) + Convert.ToDouble(dr["EntryCharge"]);

            //        double roomCharge = dr.Field<double>("RoomCharge");
            //        if (roomCharge > 0)
            //            dr.SetField("DailyFee", roomCharge);

            //        double entryCharge = dr.Field<double>("EntryCharge");
            //        if (entryCharge > 0)
            //            dr.SetField("EntryFee", entryCharge);
            //    }
            //    else
            //    {
            //        //Per Use types
            //        dr["LineCost"] = Convert.ToDouble(dr["RoomCharge"]) + Convert.ToDouble(dr["EntryCharge"]);

            //        decimal roomCharge = dr.Field<decimal>("RoomCharge");
            //        if (roomCharge > 0)
            //            dr.SetField("DailyFee", roomCharge);

            //        decimal entryCharge = dr.Field<decimal>("EntryCharge");
            //        if (entryCharge > 0)
            //            dr.SetField("EntryFee", entryCharge);
            //    }
            //}

            return dt;
        }

        public static DataTable GetRoomBillingDataByClientID2(DateTime period, int clientId)
        {
            DataTable dt;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
                dt = RoomBillingDA.GetRoomBillingTempDataByClientID(period, clientId);
            else
                dt = RoomBillingDA.GetRoomBillingDataByClientID(period, clientId);

            dt.Columns.Add("LineCost", typeof(double));

            //Part I: Get the true cost based on billing types
            foreach (DataRow dr in dt.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");
                Rooms room = RoomUtility.GetRoom(dr.Field<int>("RoomID"));

                if (billingTypeId == BillingType.Other)
                    dr["LineCost"] = 0;
                else if (BillingTypeUtility.IsGrowerUserBillingType(billingTypeId))
                {
                    if (room == Rooms.OrganicsBay)
                    {
                        //Organics bay must be charged for growers as well
                        dr["LineCost"] = dr.Field<decimal>("RoomCharge");
                    }
                    else
                        dr["LineCost"] = dr.Field<decimal>("AccountDays") * dr.Field<decimal>("RoomRate") + dr.Field<decimal>("EntryCharge");
                }
                else
                {
                    //Per Use types
                    dr["LineCost"] = dr.Field<decimal>("RoomCharge") + dr.Field<decimal>("EntryCharge");
                }
            }

            return dt;
        }
    }
}
