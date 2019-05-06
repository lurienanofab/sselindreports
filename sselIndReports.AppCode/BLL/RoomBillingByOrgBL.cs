using LNF.Repository.Billing;
using System;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingByOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(HttpContextBase context, int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);

            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.RoomByOrg);

            if (!dtSource.Columns.Contains("TotalCharge"))
                dtSource.Columns.Add("TotalCharge", typeof(double));

            if (!dtSource.Columns.Contains("SubsidyDiscount"))
                dtSource.Columns.Add("SubsidyDiscount", typeof(double));

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Grower_Observer)
                {
                    //nothing?
                }
                else if (billingTypeId == BillingType.Other)
                {
                    dr["RoomCharge"] = 0;
                    dr["TotalUsageCharge"] = dr["RoomMisc"];
                }
                else
                {
                    //nothing?
                }

                dr["TotalCharge"] = dr.Field<decimal>("TotalUsageCharge") - dr.Field<decimal>("SubsidyDiscount");
            }

            return dtSource;
        }
    }
}
