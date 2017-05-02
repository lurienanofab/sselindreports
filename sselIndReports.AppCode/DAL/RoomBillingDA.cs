using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class RoomBillingDA
    {
        public static DataTable GetRoomBillingDataByClientID(DateTime period, int clientId)
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
		    dba.AddParameter("@Action", "ByClientIDPeriod");
		    dba.AddParameter("@Period", period);
		    dba.AddParameter("@ClientID", clientId);
		    return dba.FillDataTable("RoomApportionmentInDaysMonthly_Select");
	    }

	    public static DataTable GetRoomBillingTempDataByClientID(DateTime period, int clientId)
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
		    dba.AddParameter("@Action", "ByClientIDPeriod");
		    dba.AddParameter("@Period", period);
		    dba.AddParameter("@ClientID", clientId);
            return dba.FillDataTable("RoomBillingTemp_Select");
	    }
    }
}
