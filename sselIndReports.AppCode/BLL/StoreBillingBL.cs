using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class StoreBillingBL
    {
        public static DataTable GetStoreBillingDataByClientID(HttpContextBase context, DateTime period , int clientId)
        {
            DataTable dt;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
                dt = StoreBillingDA.GetStoreBillingTempDataByClientID(period, clientId);
            else
                dt = BillingTablesBL.GetMultipleTables(context, period.Year, period.Month, clientId, BillingTableType.StoreBilling);

            return dt;
        }
    }
}
