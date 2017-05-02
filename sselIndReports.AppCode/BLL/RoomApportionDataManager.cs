using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomApportionDataManager
    {
        public static DataTable GetNAPRoomApportionDataByPeriod(DateTime period, int roomId)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "SelectByPeriod");
                dba.AddParameter("@Period", period);
                dba.AddParameter("@RoomID", roomId);
                DataTable dt = dba.FillDataTable("RoomApportionData_Select");
                dt.PrimaryKey = new DataColumn[] { dt.Columns["RoomApportionDataID"] };
                return dt;
            }
        }
    }
}
