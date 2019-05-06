using LNF.Repository.Billing;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class TieredSubsidyBillingBL
    {
        public static DataTable GetDataByPeriodAndClientID(HttpContextBase context, int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.Subsidy);

            if (!dtSource.Columns.Contains("Subsidy"))
                dtSource.Columns.Add("Subsidy", typeof(double), "UserTotalSum - UserPaymentSum");

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
