using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class StoreBillingByItemOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(HttpContextBase context, int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.StoreByItemOrg);
            return dtSource;
        }
    }
}
