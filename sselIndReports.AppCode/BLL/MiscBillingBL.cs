using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class MiscBillingBL
    {
        public static DataTable GetMiscBillingByClientID(HttpContextBase context, int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.MiscDetail);
            return dtSource;
        }
    }
}
