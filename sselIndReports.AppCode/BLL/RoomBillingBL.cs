using LNF.Billing;
using LNF.Repository;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingBL
    {
        public static IBillingTypeManager BillingTypeManager => DA.Use<IBillingTypeManager>();

        public static DataTable GetRoomBillingDataByClientID(DateTime period, int clientId)
        {
            DataTable dt;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
                dt = RoomBillingDA.GetRoomBillingTempDataByClientID(period, clientId);
            else
                dt = BillingTablesBL.GetMultipleTables(period.Year, period.Month, clientId, BillingTableType.RoomBilling);

            if (!dt.Columns.Contains("DailyFee"))
                dt.Columns.Add("DailyFee", typeof(decimal));

            if (!dt.Columns.Contains("EntryFee"))
                dt.Columns.Add("EntryFee", typeof(decimal));

            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            BillingTypeManager.CalculateRoomLineCost(dt);

            return dt;
        }
    }
}
