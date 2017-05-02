using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class MiscBillingChargeDA
    {
        public static DataTable GetDataByPeriodAndClientID(DateTime period, int clientId)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "ByClientIDPeriod");
                dba.AddParameter("@Period", period);
                dba.AddParameter("@ClientID", clientId);
                return dba.FillDataTable("MiscBillingCharge_Select");
            }
        }
    }
}
