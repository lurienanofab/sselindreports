using LNF.Repository.Billing;
using System;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class ToolBillingByAccountBL
    {
        public static DataTable GetDataByPeriodAndClientID(HttpContextBase context, int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.ToolByAccount);

            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Other)
                {
                    dr["UsageFeeCharged"] = 0;
                    dr["OverTimePenaltyFee"] = 0;
                    dr["UncancelledPenaltyFee"] = 0;
                    dr["ReservationFee"] = 0;
                    dr["TotalCharge"] = 0;
                }
            }

            return dtSource;
        }

        public static DataTable GetDataByPeriodAndClientID20110401(HttpContextBase context, int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultipleTables(context, year, month, clientId, BillingTableType.ToolByAccount20110401);
            
            foreach (DataRow dr in dtSource.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Other)
                {
                    dr["UsageFeeCharged"] = 0;
                    dr["OverTimePenaltyFee"] = 0;
                    dr["BookingFee"] = 0;
                    dr["TotalCharge"] = 0;
                }
            }

            return dtSource;
        }

        public static DataTable GetDataByPeriodAndClientID20110701(HttpContextBase context, int year, int month, int clientId)
        {
            var btt = BillingTablesBL.GetToolByAccountBillingTableType(new DateTime(year, month, 1));
            DataTable dt = BillingTablesBL.GetMultipleTables(context, year, month, clientId, btt);

            if (!dt.Columns.Contains("UsageFeeDisplay"))
                dt.Columns.Add("UsageFeeDisplay", typeof(double));

            if (!dt.Columns.Contains("SubsidyDiscount"))
                dt.Columns.Add("SubsidyDiscount", typeof(double));

            if (!dt.Columns.Contains("ForgivenFee"))
                dt.Columns.Add("ForgivenFee", typeof(double));

            foreach (DataRow dr in dt.Rows)
            {
                int billingTypeId = dr.Field<int>("BillingTypeID");

                if (billingTypeId == BillingType.Grower_Observer)
                    dr["UsageFeeDisplay"] = dr["UsageFeeCharged"];
                else if (billingTypeId == BillingType.Other)
                {
                    dr["UsageFeeCharged"] = 0;
                    dr["OverTimePenaltyFee"] = 0;
                    dr["BookingFee"] = 0;
                    dr["TotalCharge"] = 0;
                }
                else
                    dr["UsageFeeDisplay"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("TransferredFee") + dr.Field<decimal>("ForgivenFee");
            }

            return dt;
        }
    }
}
