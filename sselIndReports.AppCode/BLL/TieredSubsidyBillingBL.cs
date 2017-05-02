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
    public static class TieredSubsidyBillingBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultileTables(year, month, clientId, BillingTableType.Subsidy);

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Grower_Observer)
                {
                    //???
                }
                else if (billingTypeId == BillingType.Other)
                {
                    dr["RoomSum"] = 0;
                    dr["ToolSum"] = 0;
                    dr["UserTotalSum"] = 0;
                    dr["UserPaymentSum"] = 0;
                    dr["Accumulated"] = 0;
                }
            }

            return dtSource;
        }
    }
}
