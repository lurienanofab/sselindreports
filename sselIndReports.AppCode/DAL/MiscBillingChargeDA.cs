using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class MiscBillingChargeDA
    {
        public static DataTable GetDataByPeriodAndClientID(DateTime period, int clientId)
        {
            return DA.Command()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.MiscBillingCharge_Select");
        }
    }
}
