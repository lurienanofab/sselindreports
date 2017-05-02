using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sselIndReports.AppCode.BLL
{
    public enum BillingTableType
    {
        Subsidy = 0,
        RoomByOrg = 1,
        ToolByOrg = 2,
        StoreByOrg = 3,
        RoomByRoomOrg = 4,
        ToolByToolOrg = 5,
        StoreByItemOrg = 6,
        RoomBilling = 7,
        ToolBillingActivated = 8,
        ToolBillingUncancelled = 9,
        ToolBillingForgiven = 10,
        StoreBilling = 11,
        RoomByAccount = 12,
        ToolByAccount = 13,
        StoreByAccount = 14,
        MiscDetail = 15,
        ToolBilling20110401Reservations = 16,
        ToolBilling20110401Cancelled = 17,
        ToolBilling20110401Forgiven = 18,
        ToolByAccount20110401 = 19,
        ToolByOrg20110401 = 20,
        ToolBillingStarted = 21, //2011-09-22 This is consolidated tool billing table, no more three separated tables for tool billing on display
        ToolBillingUnStarted = 22, //2011-09-22 This is consolidated tool billing table, no more three separated tables for tool billing on display
        ToolBillingCancelled = 23 //2011-09-22 We need this to get the booking fee total
    }
}
