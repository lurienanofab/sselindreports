using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class RoomBillingDA
    {
        public static DataTable GetRoomBillingDataByClientID(DateTime period, int clientId)
        {
            return DA.Command()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.RoomApportionmentInDaysMonthly_Select");
        }

        public static DataTable GetRoomBillingTempDataByClientID(DateTime period, int clientId)
        {
            return DA.Command()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.RoomBillingTemp_Select");
        }
    }
}
