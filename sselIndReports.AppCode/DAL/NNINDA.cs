using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class NNINDA
    {
        public static DataSet GetTablesWithMinimumMinutes(DateTime period, int privs, bool makeAggData, double minimumHours)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "GetAllTablesWithMinimumHours");
                dba.AddParameter("@Period", period);
                dba.AddParameter("@Privs", privs);
                dba.AddParameter("@MakeCumUser", makeAggData);
                dba.AddParameter("@MinimumHours", minimumHours);
                DataSet ds = dba.FillDataSet("NNIN_Select");
                return ds;
            }
        }

        public static DataTable GetCumulativeUserTable()
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                return dba.ApplyParameters(new { Action = "AllInternal" }).FillDataTable("Account_Select");
        }


        public static DataSet GetCostTables(DateTime period)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "GetCostTables");
                dba.AddParameter("@Period", period);
                return dba.FillDataSet("NNIN_Select");
            }
        }

        public static bool CumulativeUserExists(DateTime period)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "DataCheck");
                dba.AddParameter("@eDate", period);
                return dba.ExecuteScalar<bool>("CumUserForNNIN_Select");
            }
        }

        public static DataTable GetCumulativeUserAggregateData(DateTime sDate, DateTime eDate)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "Aggregate");
                dba.AddParameter("@sDate", sDate);
                dba.AddParameter("@eDate", eDate);
                return dba.FillDataTable("CumUserForNNIN_Select");
            }
        }
    }
}
