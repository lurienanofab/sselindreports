using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class AccountDA
    {
        public static DataTable GetActiveAccountManagers()
        {
            return DA.Command(CommandType.Text)
                .FillDataTable("SELECT * FROM Reporting.dbo.v_ActiveAccountManagers ORDER BY AccountName");
        }

        public static DataView GetInternalAccounts()
        {
            return DA.Command()
                .Param("Action", "AllInternal")
                .FillDataTable("dbo.Account_Select")
                .DefaultView;
        }

        public static DataTable GetActiveManagers(int clientId)
        {
            return DA.Command()
                .Param("Action", "AllActiveManager")
                .Param("ClientID", clientId > 0, clientId)
                .FillDataTable("dbo.ClientOrg_Select");
        }

        public static DataSet GetClientAccountDataSet(int managerOrgId)
        {
            return DA.Command()
                .Param("Action", "ClientAccountByManager")
                .Param("ManagerOrgID", managerOrgId)
                .FillDataSet("dbo.ClientAccount_Select");
        }

        public static DataTable GetAccountsByOrgID(int orgId)
        {
            return DA.Command()
                .Param("Action", "AllByOrg")
                .Param("OrgID", orgId)
                .FillDataTable("dbo.Account_Select");
        }

        public static DataTable GetAccountDetailsByOrgID(int year, int month, int orgId)
        {
            DateTime sDate = new DateTime(year, month, 1);
            DateTime eDate = sDate.AddMonths(1);

            return DA.Command()
                .Param("Action", "GetAccountDetailByOrgID")
                .Param("OrgID", orgId)
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataTable("dbo.Account_Select");
        }

        public static DataTable GetManagersByPeriod(DateTime sDate, DateTime eDate, int chargeTypeId)
        {
            return DA.Command()
                .Param("Action", "AllActiveManagerByPeriodByChargeType")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .Param("ChargeTypeID", chargeTypeId)    //internal, external aca and external business
                .FillDataTable("dbo.ClientOrg_Select");
        }
    }
}
