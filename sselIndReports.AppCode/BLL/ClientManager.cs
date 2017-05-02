using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using sselIndReports.AppCode.DAL;

namespace sselIndReports.AppCode.BLL
{
    public static class ClientManager
    {
        public static DataTable GetUsersByManagerOrgID(int sYear, int sMonth, int NumMonths, int ManagerOrgID)
        {
            DateTime earlyDate = new DateTime(2008, 4, 1);
            DateTime sDate = new DateTime(sYear, sMonth, 1);
            DateTime eDate = DateTime.Now;

            if (sDate > eDate) return null;

            //get all the clients associated with this manager org id since very very early date
            DataTable dtAllClientsWithTheManager = ClientDA.GetClientsByManagerOrgID(earlyDate, eDate, ManagerOrgID);

            //get the clients list that has access the lab during the month specified by the sDate and eDate
            DataTable dtClientsWhoAccessedLab = BillingManager.GetMonthlyClientID(sDate);

            //this will store all the clients who are associated with the manager and also has accessed the lab during this month.
            DataTable dtFilteredClients = dtAllClientsWithTheManager.Clone();

            foreach (DataRow row in dtAllClientsWithTheManager.Rows)
            {
                DataRow[] rows = dtClientsWhoAccessedLab.Select(string.Format("ClientID = {0}", row["ClientID"]));
                if (rows.Length > 0)
                {
                    DataRow ndr = dtFilteredClients.NewRow();
                    ndr["ClientID"] = row["ClientID"];
                    ndr["DisplayName"] = row["DisplayName"];
                    dtFilteredClients.Rows.Add(ndr);
                }
            }

            return dtFilteredClients;
        }
    }
}
