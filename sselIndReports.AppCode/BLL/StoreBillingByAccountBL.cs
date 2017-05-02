using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class StoreBillingByAccountBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultileTables(year, month, clientId, BillingTableType.StoreByAccount);
            return dtSource;
        }
    }
}
