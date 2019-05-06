using System;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class StoreBillingByOrgBL
    {
        [Obsolete]
        public static DataTable GetDataByPeriodAndClientID(HttpContextBase context, int year, int month, int clientId)
        {
            return BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.StoreByOrg);
        }
    }
}
