using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class TieredSubsidyBillingDA
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);

            return DA.Command()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.TieredSubsidyBilling_Select");
        }

        public static DataTable GetAggSubsidy(DateTime startPeriod, DateTime endPeriod, int managerOrgId)
        {
            //This stored procedure selects StartPeriod >= x <= EndPeriod (using BETWEEN)
            return DA.Command()
                .Param("Action", "SubsidyAggReport")
                .Param("Period", startPeriod)
                .Param("EndPeriod", endPeriod)
                .Param("ManagerOrgID", managerOrgId)
                .FillDataTable("dbo.TieredSubsidyBilling_Select");
        }
    }
}
