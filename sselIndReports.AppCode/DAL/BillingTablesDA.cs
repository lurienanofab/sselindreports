using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class BillingTablesDA
    {
        public static DataSet GetMultileTables20110701(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
            dba.AddParameter("@Action", "UserUsageSummary");
            dba.AddParameter("@Period", period);
            dba.AddParameter("@ClientID", clientId);
            return dba.FillDataSet("BillingTables_Select20110701");
        }

        public static DataSet GetMultileTables(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
            dba.AddParameter("@Action", "UserUsageSummary");
            dba.AddParameter("@Period", period);
            dba.AddParameter("@ClientID", clientId);
            return dba.FillDataSet("BillingTables_Select");
        }
    }
}
