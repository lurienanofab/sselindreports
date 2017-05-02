using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingByRoomOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultileTables(year, month, clientId, BillingTableType.RoomByRoomOrg);

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Grower_Observer)
                {
                    //???
                }
                else if (billingTypeId == BillingType.Other)
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
