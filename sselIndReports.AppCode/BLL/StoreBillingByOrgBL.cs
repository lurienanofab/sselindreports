using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class StoreBillingByOrgBL
    {
        [Obsolete]
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            return BillingTablesBL.GetMultipleTables(year, month, clientId, BillingTableType.StoreByOrg);
        }
    }
}
