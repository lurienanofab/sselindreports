using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class SubsidyBillingDA
    {
        public static DataTable GetSubsidyBillingDataByClientID(DateTime period, int clientId)
        {
            using (SQLDBAccess dba = new LNF.CommonTools.SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "ByClientIDPeriod");
                dba.AddParameter("@Period", period);
                dba.AddParameter("@ClientID", clientId);
                return dba.FillDataTable("TieredSubsidyBilling_Select");
            }
        }
    }
}
