using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomApportionDataManager
    {
        public static DataTable GetNAPRoomApportionDataByPeriod(DateTime period, int roomId)
        {
            var dt = DataCommand.Create()
                .Param("Action", "SelectByPeriod")
                .Param("Period", period)
                .Param("RoomID", roomId)
                .FillDataTable("dbo.RoomApportionData_Select");

            dt.PrimaryKey = new[] { dt.Columns["RoomApportionDataID"] };

            return dt;
        }
    }
}
