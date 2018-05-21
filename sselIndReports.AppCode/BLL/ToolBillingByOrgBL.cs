using LNF.Repository.Billing;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class ToolBillingByOrgBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            BillingTableType btt = BillingTablesBL.GetToolByOrgBillingTableType(new DateTime(year, month, 1));
            DataTable dtSource = BillingTablesBL.GetMultipleTables(year, month, clientId, btt);

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
