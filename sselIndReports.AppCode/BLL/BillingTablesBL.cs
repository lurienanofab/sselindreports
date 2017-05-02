using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using sselIndReports.AppCode.DAL;

namespace sselIndReports.AppCode.BLL
{
    public static class BillingTablesBL
    {
        public static readonly DateTime CutoffPeriod = new DateTime(2011, 4, 1);

        public static BillingTableType GetToolByOrgBillingTableType(DateTime period)
        {
            BillingTableType result = (period < CutoffPeriod) ? BillingTableType.ToolByOrg : BillingTableType.ToolByOrg20110401;
            return result;
        }

        public static DataTable GetMultileTables20110701(int year, int month, int clientId, BillingTableType bt)
        {
            DataSet ds;
            if (HttpContext.Current.Session["UserUsageSummaryTables20110701"] == null)
            {
                ds = BillingTablesDA.GetMultileTables20110701(year, month, clientId);
                HttpContext.Current.Session["UserUsageSummaryTables20110701"] = ds;
            }
            else
                ds = (DataSet)HttpContext.Current.Session["UserUsageSummaryTables20110701"];

            return ds.Tables[(int)bt];
        }

        public static DataTable GetMultileTables(int year, int month, int clientId, BillingTableType bt)
        {
            DataSet ds;
            if (HttpContext.Current.Session["UserUsageSummaryTables"] == null)
            {
                ds = BillingTablesDA.GetMultileTables(year, month, clientId);
                HttpContext.Current.Session["UserUsageSummaryTables"] = ds;
            }
            else
                ds = (DataSet)HttpContext.Current.Session["UserUsageSummaryTables"];

            return ds.Tables[(int)bt];
        }
    }
}
