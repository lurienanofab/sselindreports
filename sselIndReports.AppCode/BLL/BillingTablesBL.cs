using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class BillingTablesBL
    {
        public static readonly DateTime NewBillingStartPeriod = new DateTime(2011, 10, 1);
        public static readonly DateTime CutoffPeriod = new DateTime(2011, 4, 1);

        public static BillingTableType GetToolByOrgBillingTableType(DateTime period)
        {
            BillingTableType result = (period < CutoffPeriod) ? BillingTableType.ToolByOrg : BillingTableType.ToolByOrg20110401;
            return result;
        }

        public static BillingTableType GetToolByAccountBillingTableType(DateTime period)
        {
            BillingTableType result = (period < CutoffPeriod) ? BillingTableType.ToolByAccount : BillingTableType.ToolByAccount20110401;
            return result;
        }

        public static DataSet GetDataSet(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);

            DataSet ds;

            if (period < NewBillingStartPeriod)
            {
                if (HttpContext.Current.Session["UserUsageSummaryTables"] == null)
                    HttpContext.Current.Session["UserUsageSummaryTables"] = BillingTablesDA.GetMultipleTables(year, month, clientId);
                ds = (DataSet)HttpContext.Current.Session["UserUsageSummaryTables"];
            }
            else
            {
                if (HttpContext.Current.Session["UserUsageSummaryTables20110701"] == null)
                    HttpContext.Current.Session["UserUsageSummaryTables20110701"] = BillingTablesDA.GetMultipleTables20110701(year, month, clientId);
                ds = (DataSet)HttpContext.Current.Session["UserUsageSummaryTables20110701"];
            }

            return ds;
        }

        public static DataTable GetMultipleTables(int year, int month, int clientId, BillingTableType bt)
        {
            DataSet ds = GetDataSet(year, month, clientId);
            return ds.Tables[(int)bt];
        }
    }
}
