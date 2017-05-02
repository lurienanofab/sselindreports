using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

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
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "NAPRoomsWithCost");
                dba.AddParameter("@Period", period);
                return dba.FillDataTable("Room_Select");
            }
        }
    }
}
