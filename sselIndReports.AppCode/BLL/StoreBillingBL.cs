using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using sselIndReports.AppCode.DAL;

namespace sselIndReports.AppCode.BLL
{
    public static class StoreBillingBL
    {
        public static DataTable GetStoreBillingDataByClientID(DateTime period , int clientId)
        {
            DataTable dt;

            if (period.Month == DateTime.Now.Month && period.Year == DateTime.Now.Year)
                dt = StoreBillingDA.GetStoreBillingTempDataByClientID(period, clientId);
            else
            {
                if (period >= new DateTime(2011, 10, 1))
                    dt = BillingTablesBL.GetMultipleTables20110701(period.Year, period.Month, clientId, BillingTableType.StoreBilling);
                else
                    dt = BillingTablesBL.GetMultipleTables(period.Year, period.Month, clientId, BillingTableType.StoreBilling);
            }

            return dt;
        }
    }
}
