using LNF.Repository.Billing;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingByAccountBL
    {
        public static DataTable GetDataByPeriodAndClientID(HttpContextBase context, int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.RoomByAccount);

            if (!dtSource.Columns.Contains("SubsidyDiscount"))
                dtSource.Columns.Add("SubsidyDiscount", typeof(double));

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Other)
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
