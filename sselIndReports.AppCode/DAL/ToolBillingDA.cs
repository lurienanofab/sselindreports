using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class ToolBillingDA
    {
        public static DataSet GetToolBillingDataByClientID(DateTime period, int clientId)
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
		    dba.AddParameter("@Action", "ByClientIDPeriod");
		    dba.AddParameter("@Period", period);
		    dba.AddParameter("@ClientID", clientId);
		    return dba.FillDataSet("ToolBilling_Select");
	    }

        public static DataSet GetToolBillingTempDataByClientID20110701(DateTime period, int clientId)
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
            dba.AddParameter("@Period", period);
            dba.AddParameter("@ClientID", clientId);
            return dba.FillDataSet("ToolBillingTemp20110701_Select");
        }

	    public static DataSet GetToolBillingTempDataByClientID(DateTime period, int clientId)
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
		    dba.AddParameter("@Action", "ByClientIDPeriod");
		    dba.AddParameter("@Period", period);
		    dba.AddParameter("@ClientID", clientId);
		    return dba.FillDataSet("ToolBillingTemp_Select");
	    }
    }
}
