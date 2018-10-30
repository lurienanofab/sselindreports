using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class StoreBillingDA
    {
        public static DataTable GetStoreBillingDataByClientID(DateTime period, int clientId)
        {
            return DA.Command()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.StoreBilling_Select");
        }

        public static DataTable GetStoreBillingTempDataByClientID(DateTime period, int clientId)
        {
            return DA.Command()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.StoreBillingTemp_Select");
        }
    }
}
