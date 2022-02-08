using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class ClientDA
    {
        public static string GetTechnicalInterestByClientID(int clientId)
        {
            string technicalField = string.Empty;
            try
            {
                technicalField = DataCommand.Create().Param("Action", "GetTechnicalInterest").Param("ClientID", clientId).ExecuteScalar<string>("dbo.Client_Select").Value;
            }
            catch
            {
                technicalField = "There is no technical interest associated";
            }

            return technicalField;
        }

        public static DataTable GetClientsByManagerOrgID(DateTime sDate, DateTime eDate, int managerOrgId)
        {
            return DataCommand.Create()
                .Param(new { Action = "GetUsersByManagerOrgID", sDate, eDate, ManagerOrgID = managerOrgId })
                .FillDataTable("dbo.ClientManager_Select");
        }

        public static int GetOrgIDByClientOrgID(int ClientOrgID)
        {
            return DataCommand.Create()
                .Param(new { Action = "GetOrgIDByClientOrgID", ClientOrgID })
                .ExecuteScalar<int>("dbo.ClientOrg_Select").Value;
        }

        public static DataTable GetAllAccountsByClientOrgID(int clientOrgId)
        {
            return DataCommand.Create()
                .Param("Action", "GetAllAccountsByClientOrgID")
                .Param("ClientOrgID", clientOrgId)
                .FillDataTable("dbo.Account_Select");
        }

        [Obsolete("do not use!")]
        public static DataTable GetAllClientsByDateAndPrivs(DateTime sDate, DateTime eDate, int privs)
        {
            return DataCommand.Create()
                .Param("Action", "All")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .Param("Privs", privs)
                .FillDataTable("dbo.Client_Select");
        }

        public static DataTable GetClientsByManagerID(DateTime sDate, DateTime eDate, int clientId)
        {
            return DataCommand.Create()
                .Param("Action", "ByMgr")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .Param("ClientID", clientId)
                .FillDataTable("dbo.Client_Select");
        }
    }
}
