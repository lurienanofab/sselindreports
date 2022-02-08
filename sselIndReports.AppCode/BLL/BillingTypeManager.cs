using LNF.Billing;
using LNF.Data;
using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.BLL
{
    public static class BillingTypeManager
    {
        public static DataTable GetBillingTypes()
        {
            return DataCommand.Create()
                .Param("Action", "All")
                .FillDataTable("dbo.BillingType_Select");
        }

        public static DataTable GetBillingTypes(object isActive)
        {
            if (isActive == DBNull.Value || isActive is bool)
            {
                return DataCommand.Create()
                    .Param("Action", "All")
                    .Param("IsActive", isActive)
                    .FillDataTable("dbo.BillingType_Select");
            }

            throw new Exception("Invalid type for parameter IsActive. Must be System.DBNull or System.Boolean.");
        }

        public static decimal GetTotalCostByBillingType(int billingTypeId, decimal hours, decimal entries, LabRoom room, decimal totalCalcCost, decimal totalHours = 0)
        {
            decimal result = 0;

            if (room == LabRoom.CleanRoom)
            {
                if (billingTypeId == BillingTypes.Int_Ga)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 875;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingTypes.Int_Si)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 1315;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingTypes.Int_Hour)
                    return 2.5M * entries + 15 * hours;
                else if (billingTypeId == BillingTypes.Int_Tools)
                    return 2.5M * entries;
                else if (billingTypeId == BillingTypes.ExtAc_Ga)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 875;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingTypes.ExtAc_Si)
                {
                    if (totalHours > 0)
                        return (hours / totalHours) * 1315;
                    else
                        return 0;
                }
                else if (billingTypeId == BillingTypes.ExtAc_Tools)
                    return 2.5M * entries;
                else if (billingTypeId == BillingTypes.ExtAc_Hour)
                    return 2.5M * entries + 15 * hours;
                else if (billingTypeId == BillingTypes.NonAc)
                    return hours * 77;
                else if (billingTypeId == BillingTypes.NonAc_Tools)
                    return 2.5M * hours;
                else if (billingTypeId == BillingTypes.NonAc_Hour)
                    return 2.5M * entries + 45 * hours;
                else if (billingTypeId == BillingTypes.Other)
                    return 0;
            }
            else if (room == LabRoom.ChemRoom)
            {
                if (billingTypeId == BillingTypes.Other)
                    return 0;
                else if (billingTypeId >= BillingTypes.NonAc)
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
            else if (room == LabRoom.TestLab)
            {
                if (billingTypeId == BillingTypes.Other)
                    return 0;
                else if (billingTypeId >= BillingTypes.NonAc)
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
            else if (room == LabRoom.OrganicsBay)
            {
                if (billingTypeId == BillingTypes.Other)
                    return 0;
                else if (billingTypeId >= BillingTypes.NonAc)
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
            try
            {
                return DataCommand.Create()
                    .Param("Action", "GetCurrentTypeName")
                    .Param("ClientOrgID", clientOrgId)
                    .ExecuteScalar<string>("dbo.ClientOrgBillingTypeTS_Select").Value;
            }
            catch
            {
                return "Error, there is no billing type with this user";
            }
        }

        public static DataTable GetClientBillingTypesByPeriod(DateTime period)
        {
            return DataCommand.Create()
                .Param("Action", "GetClientBillingTypeByPeriod")
                .Param("Period", period)
                .FillDataTable("dbo.ClientOrgBillingTypeTS_Select");
        }
    }
}
