using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.CommonTools;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingByOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);
            DataTable dtSource;
            if (period < new DateTime(2011, 10, 1))
                dtSource = BillingTablesBL.GetMultipleTables(year, month, clientId, BillingTableType.RoomByOrg);
            else
                dtSource = BillingTablesBL.GetMultipleTables20110701(year, month, clientId, BillingTableType.RoomByOrg);

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
