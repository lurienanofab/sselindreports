using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class BillingTypeManager
    {
        public static DataTable GetBillingTypes()
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "All");
                return dba.FillDataTable("BillingType_Select");
            }
        }

        public static DataTable GetBillingTypes(object isActive)
        {
            if (isActive == DBNull.Value || isActive is bool)
            {
                using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                {
                    dba.AddParameter("@Action", "All");
                    dba.AddParameter("@IsActive", isActive);
                    return dba.FillDataTable("BillingType_Select");
                }
            }
            
            throw new Exception("Invalid type for parameter IsActive. Must be System.DBNull or System.Boolean.");
        }

        public static decimal GetTotalCostByBillingType(int billingTypeId, decimal hours, decimal entries, Rooms room, decimal totalCalcCost, decimal totalHours = 0)
        {
            decimal result = 0;

            if (room == Rooms.CleanRoom)
            {
                if (billingTypeId == BillingType.Int_Ga)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 875;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingType.Int_Si)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 1315;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingType.Int_Hour)
                    return 2.5M * entries + 15 * hours;
                else if (billingTypeId == BillingType.Int_Tools)
                    return 2.5M * entries;
                else if (billingTypeId == BillingType.ExtAc_Ga)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 875;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingType.ExtAc_Si)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 1315;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingType.ExtAc_Tools)
                    return 2.5M * entries;
                else if (billingTypeId == BillingType.ExtAc_Hour)
                    return 2.5M * entries + 15 * hours;
                else if (billingTypeId == BillingType.NonAc)
                    return hours * 77;
                else if (billingTypeId == BillingType.NonAc_Tools)
                    return 2.5M * hours;
                else if (billingTypeId == BillingType.NonAc_Hour)
                    return 2.5M * entries + 45 * hours;
                else if (billingTypeId == BillingType.Other)
                    return 0;
            }
            else if (room == Rooms.WetChemistry)
            {
                if (billingTypeId == BillingType.Other)
                    return 0;
                else if (billingTypeId >= BillingType.NonAc)
                {
                    if (hours > 0)
                        return 190;
                    else
                        return 0;
                }
                else
                {
                    if (hours > 0)
                    {
                        if (totalHours > 0)
                            return (hours / totalHours) * 95;
                        else
                            return 95; //2009-05-07 For hourly users, it's possible for them to come here
                    }
                    else
                        return 0;
                }
            }
            else if (room == Rooms.TestLab)
            {
                if (billingTypeId == BillingType.Other)
                    return 0;
                else if (billingTypeId >= BillingType.NonAc)
                {
                    if ((entries > 0 || hours > 0) && totalCalcCost > 0)
                        return 50;
                    else
                        return 0;
                }
                else
                {
                    if ((entries > 0 || hours > 0) && totalCalcCost > 0)
                        return 25;
                    else
                        return 0;
                }
            }
            else if (room == Rooms.OrganicsBay)
            {
                if (billingTypeId == BillingType.Other)
                    return 0;
                else if (billingTypeId >= BillingType.NonAc)
                {
                    if (entries > 0 || hours > 0)
                        return 190;
                    else
                        return 0;
                }
                else
                {
                    if (entries > 0 || hours > 0)
                        return 95;
                    else
                        return 0;
                }
            }
            else
                return 99999;

            return result;
        }

        public static string GetBillingTypeName(int clientOrgId)
        {
            using(SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "GetCurrentTypeName");
                dba.AddParameter("@ClientOrgID", clientOrgId);

                try
                {
                    string result = dba.ExecuteScalar<string>("ClientOrgBillingTypeTS_Select");
                    return result;
                }
                catch
                {
                    return "Error, there is no billing type with this user";
                }
            }
        }

        public static DataTable GetClientBillingTypesByPeriod(DateTime period)
        {
            using(SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "GetClientBillingTypeByPeriod");
                dba.AddParameter("@Period", period);
                return dba.FillDataTable("ClientOrgBillingTypeTS_Select");
            }
        }
    }
}
