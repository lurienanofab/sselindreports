using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomManager
    {
        [Obsolete("Use LNF.Data.Rooms instead.")]
        public enum LabRoom
        {
            CleanRoom = 6,
            ChemRoom = 25
        }

        public static DataTable GetAllNAPRoomsWithCosts(DateTime period)
        {
            return DA.Command()
                .Param("Action", "NAPRoomsWithCost")
                .Param("Period", period)
                .FillDataTable("dbo.Room_Select");
        }
    }
}
