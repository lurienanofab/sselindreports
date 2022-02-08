using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class SubsidyBillingDA
    {
        public static DataTable GetSubsidyBillingDataByClientID(DateTime period, int clientId)
        {
            return DataCommand.Create()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.TieredSubsidyBilling_Select");
        }
    }
}
