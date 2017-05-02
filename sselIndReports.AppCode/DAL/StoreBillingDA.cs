using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class StoreBillingDA
    {
        public static DataTable GetStoreBillingDataByClientID(DateTime period, int clientId)
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
            dba.AddParameter("@Action", "ByClientIDPeriod");
            dba.AddParameter("@Period", period);
            dba.AddParameter("@ClientID", clientId);
            return dba.FillDataTable("StoreBilling_Select");
        }

        public static DataTable GetStoreBillingTempDataByClientID(DateTime period, int clientId)
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
            dba.AddParameter("@Action", "ByClientIDPeriod");
            dba.AddParameter("@Period", period);
            dba.AddParameter("@ClientID", clientId);
            return dba.FillDataTable("StoreBillingTemp_Select");
        }
    }
}
