using LNF.Billing;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingByRoomOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(HttpContextBase context, int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.RoomByRoomOrg);

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingTypes.Grower_Observer)
                {
                    //???
                }
                else if (billingTypeId == BillingTypes.Other)
                {
                    dr["RoomCharge"] = 0;
                    dr["EntryCharge"] = 0;
                    dr["TotalCharge"] = 0;
                }
            }

            return dtSource;
        }
    }
}
