﻿using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class BillingTablesDA
    {
        public static DataSet GetMultipleTables20110701(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);

            var ds = DataCommand.Create()
                .Param("Action", "UserUsageSummary")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataSet("dbo.BillingTables_Select20110701");

            return ds;
        }

        public static DataSet GetMultipleTables(int year, int month, int clientId)
        {
            DateTime period = new DateTime(year, month, 1);

            return DataCommand.Create()
                .Param("Action", "UserUsageSummary")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataSet("dbo.BillingTables_Select");
        }
    }
}
