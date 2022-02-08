using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomManager
    {
        public static DataTable GetAllNAPRoomsWithCosts(DateTime period)
        {
            return DataCommand.Create()
                .Param("Action", "NAPRoomsWithCost")
                .Param("Period", period)
                .FillDataTable("dbo.Room_Select");
        }
    }
}
