using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class TieredSubsidyBillingDA
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);

            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "ByClientIDPeriod");
                dba.AddParameter("@Period", period);
                dba.AddParameter("@ClientID", clientId);
                return dba.FillDataTable("TieredSubsidyBilling_Select");
            }
        }

        public static DataTable GetAggSubsidy(DateTime startPeriod, DateTime endPeriod, int managerOrgId)
        {
            //This stored procedure selects StartPeriod >= x <= EndPeriod (using BETWEEN)
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "SubsidyAggReport");
                dba.AddParameter("@Period", startPeriod);
                dba.AddParameter("@EndPeriod", endPeriod);
                dba.AddParameter("@ManagerOrgID", managerOrgId);
                return dba.FillDataTable("TieredSubsidyBilling_Select");
            }
        }
    }
}
