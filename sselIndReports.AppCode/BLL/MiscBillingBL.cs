using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class MiscBillingBL
    {
        public static DataTable GetMiscBillingByClientID(int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultileTables(year, month, clientId, BillingTableType.MiscDetail);
            return dtSource;
        }
    }
}
