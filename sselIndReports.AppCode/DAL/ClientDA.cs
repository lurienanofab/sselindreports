using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class ClientDA
    {
        public static string GetTechnicalInterestByClientID(int clientId)
        {
            using (var dba = new SQLDBAccess("cnSselData"))
            {
                string TechnicalField = string.Empty;
                try
                {
                    TechnicalField = dba.ApplyParameters(new { Action = "GetTechnicalInterest", ClientID = clientId }).ExecuteScalar<string>("Client_Select");
                }
                catch
                {
                    TechnicalField = "There is no technical interest associated";
                }

                return TechnicalField;
            }
        }

        public static DataTable GetClientsByManagerOrgID(DateTime sDate, DateTime eDate, int managerOrgId)
        {
            using (var dba = new SQLDBAccess("cnSselData"))
                return dba.ApplyParameters(new { Action = "GetUsersByManagerOrgID", sDate = sDate, eDate = eDate, ManagerOrgID = managerOrgId }).FillDataTable("ClientManager_Select");
        }

        public static int GetOrgIDByClientOrgID(int ClientOrgID)
        {
            using (var dba = new SQLDBAccess("cnSselData"))
                return dba.ApplyParameters(new { Action = "GetOrgIDByClientOrgID", ClientOrgID = ClientOrgID }).ExecuteScalar<int>("ClientOrg_Select");
        }

        public static DataTable GetAllAccountsByClientOrgID(int clientOrgId)
        {
            using (var dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "GetAllAccountsByClientOrgID");
                dba.AddParameter("@ClientOrgID", clientOrgId);
                return dba.FillDataTable("Account_Select");
            }
        }

        [Obsolete("do not use!")]
        public static DataTable GetAllClientsByDateAndPrivs(DateTime sDate, DateTime eDate, int privs)
        {
            SQLDBAccess DB = new SQLDBAccess("cnSselData");
            DB.AddParameter("@Action", "All");
            DB.AddParameter("@sDate", sDate);
            DB.AddParameter("@eDate", eDate);
            DB.AddParameter("@Privs", privs);
            return DB.FillDataTable("Client_Select");
        }

        public static DataTable GetClientsByManagerID(DateTime sDate, DateTime eDate, int clientId)
        {
            SQLDBAccess DB = new SQLDBAccess("cnSselData");
            DB.AddParameter("@Action", "ByMgr");
            DB.AddParameter("@sDate", sDate);
            DB.AddParameter("@eDate", eDate);
            DB.AddParameter("@ClientID", clientId);
            return DB.FillDataTable("Client_Select");
        }
    }
}
