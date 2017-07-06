using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.CommonTools;

namespace sselIndReports.AppCode.BLL
{
    public static class ToolBillingByOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);
            BillingTableType bt = BillingTablesBL.GetToolByOrgBillingTableType(period);
            DataTable dtSource;

            if (period < new DateTime(2011, 10, 1))
                dtSource = BillingTablesBL.GetMultipleTables(year, month, clientId, bt);
            else
                dtSource = BillingTablesBL.GetMultipleTables20110701(year, month, clientId, bt);

            if (!dtSource.Columns.Contains("UsageFeeDisplay"))
                dtSource.Columns.Add("UsageFeeDisplay", typeof(double));

            if (!dtSource.Columns.Contains("TotalCharge"))
                dtSource.Columns.Add("TotalCharge", typeof(double));

            if (!dtSource.Columns.Contains("SubsidyDiscount"))
                dtSource.Columns.Add("SubsidyDiscount", typeof(double));

            if (!dtSource.Columns.Contains("TransferredFee"))
                dtSource.Columns.Add("TransferredFee", typeof(double));

            if (!dtSource.Columns.Contains("ForgivenFee"))
                dtSource.Columns.Add("ForgivenFee", typeof(double));

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Grower_Observer)
                    dr["UsageFeeDisplay"] = dr["UsageFeeCharged"];
                else if (billingTypeId == BillingType.Other)
                {
                    dr["ToolCharge"] = 0;
                    dr["TotalUsageCharge"] = dr["ToolMisc"];
                }
                else
                    dr["UsageFeeDisplay"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("TransferredFee") + dr.Field<decimal>("ForgivenFee");

                dr["SubsidyDiscount"] = dr.Field<decimal>("SubsidyDiscount") + dr.Field<decimal>("ToolMiscSubsidyDiscount");
                dr["TotalCharge"] = dr.Field<decimal>("TotalUsageCharge") - dr.Field<decimal>("SubsidyDiscount");
            }

            return dtSource;
        }

        public static DataTable GetDataByPeriodAndClientID20110401(int year, int month, int clientId)
        {
            BillingTableType bt = BillingTablesBL.GetToolByOrgBillingTableType(new DateTime(year, month, 1));

            DataTable dtSource = BillingTablesBL.GetMultipleTables(year, month, clientId, bt);

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Other)
                {
                    dr["ToolCharge"] = 0;
                    dr["TotalUsageCharge"] = dr["ToolMisc"];
                }
            }

            return dtSource;
        }
    }
}
