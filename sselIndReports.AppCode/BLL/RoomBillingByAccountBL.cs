using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;

namespace sselIndReports.AppCode.BLL
{
    public static class RoomBillingByAccountBL
    {
        public static DataTable GetDataByPeriodAndClientID(int year, int month, int clientId)
        {
            DataTable dtSource = BillingTablesBL.GetMultileTables(year, month, clientId, BillingTableType.RoomByAccount);

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
