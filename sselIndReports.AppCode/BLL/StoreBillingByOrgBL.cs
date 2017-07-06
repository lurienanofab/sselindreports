using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class StoreBillingByOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(year, month, clientId, BillingTableType.StoreByOrg);
            return dtSource;
        }
    }
}
