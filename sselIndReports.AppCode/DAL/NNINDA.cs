using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class NNINDA
    {
        public static DataSet GetTablesWithMinimumMinutes(DateTime period, int privs, bool makeAggData, double minimumHours)
        {
            return DA.Command()
                .Param("Action", "GetAllTablesWithMinimumHours")
                .Param("Period", period)
                .Param("Privs", privs)
                .Param("MakeCumUser", makeAggData)
                .Param("MinimumHours", minimumHours)
                .FillDataSet("dbo.NNIN_Select");
        }

        public static DataTable GetCumulativeUserTable()
        {
            return DA.Command()
                .Param("Action", "AllInternal")
                .FillDataTable("dbo.Account_Select");
        }


        public static DataSet GetCostTables(DateTime period)
        {
            return DA.Command()
                .Param("Action", "GetCostTables")
                .Param("Period", period)
                .FillDataSet("dbo.NNIN_Select");
        }

        public static bool CumulativeUserExists(DateTime period)
        {
            return DA.Command()
                .Param("@Action", "DataCheck")
                .Param("@eDate", period)
                .ExecuteScalar<bool>("dbo.CumUserForNNIN_Select");
        }

        public static DataTable GetCumulativeUserAggregateData(DateTime sDate, DateTime eDate)
        {
            return DA.Command()
                .Param("Action", "Aggregate")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataTable("dbo.CumUserForNNIN_Select");
        }
    }
}
