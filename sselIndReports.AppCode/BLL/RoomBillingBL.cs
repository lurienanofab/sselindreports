using LNF;
using LNF.Models.Billing;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingBL
    {
        public static IBillingTypeManager BillingType => ServiceProvider.Current.Billing.BillingType;

        public static DataTable GetRoomBillingDataByClientID(HttpContextBase context, DateTime period, int clientId)
        {
            DataTable dt;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
                dt = RoomBillingDA.GetRoomBillingTempDataByClientID(period, clientId);
            else
                dt = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, BillingTableType.RoomBilling);

            if (!dt.Columns.Contains("DailyFee"))
                dt.Columns.Add("DailyFee", typeof(decimal));

            if (!dt.Columns.Contains("EntryFee"))
                dt.Columns.Add("EntryFee", typeof(decimal));

            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            BillingType.CalculateRoomLineCost(dt);

            return dt;
        }
    }
}
