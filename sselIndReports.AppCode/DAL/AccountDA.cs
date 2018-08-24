using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class AccountDA
    {
        public static DataTable GetActiveAccountManagers()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.CommandTypeText();
                var dt = dba.FillDataTable("SELECT * FROM Reporting.dbo.v_ActiveAccountManagers ORDER BY AccountName");
                return dt;
            }
        }

        public static DataView GetInternalAccounts()
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                return dba.ApplyParameters(new { Action = "AllInternal" }).FillDataTable("Account_Select").DefaultView;
        }

        public static DataTable GetActiveManagers(int clientId)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                return dba
                    .AddParameter("@Action", "AllActiveManager")
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .FillDataTable("ClientOrg_Select");
            }
        }

        public static DataSet GetClientAccountDataSet(int managerOrgId)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "ClientAccountByManager");
                dba.AddParameter("@ManagerOrgID", managerOrgId);
                return dba.FillDataSet("ClientAccount_Select");
            }
        }

        public static DataTable GetAccountsByOrgID(int orgId)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                return dba.ApplyParameters(new { Action = "AllByOrg", OrgID = orgId }).FillDataTable("Account_Select");
        }

        public static DataTable GetAccountDetailsByOrgID(int year, int month, int orgId)
        {
            DateTime sDate = new DateTime(year, month, 1);
            DateTime eDate = sDate.AddMonths(1);

            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "GetAccountDetailByOrgID");
                dba.AddParameter("@OrgID", orgId);
                dba.AddParameter("@sDate", sDate);
                dba.AddParameter("@eDate", eDate);
                return dba.FillDataTable("Account_Select");
            }
        }

        public static DataTable GetManagersByPeriod(DateTime sDate, DateTime eDate, int chargeTypeId)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "AllActiveManagerByPeriodByChargeType");
                dba.AddParameter("@sDate", sDate);
                dba.AddParameter("@eDate", eDate);
                dba.AddParameter("@ChargeTypeID", chargeTypeId);	//internal, external aca and external business
                return dba.FillDataTable("ClientOrg_Select");
            }
        }
    }
}
