using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class RoomBillingDA
    {
        public static DataTable GetRoomBillingDataByClientID(DateTime period, int clientId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ByClientIDPeriod");
                dba.AddParameter("@Period", period);
                dba.AddParameter("@ClientID", clientId);
                return dba.FillDataTable("RoomApportionmentInDaysMonthly_Select");
            }
        }

        public static DataTable GetRoomBillingTempDataByClientID(DateTime period, int clientId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ByClientIDPeriod");
                dba.AddParameter("@Period", period);
                dba.AddParameter("@ClientID", clientId);
                return dba.FillDataTable("RoomBillingTemp_Select");
            }
        }
    }
}
