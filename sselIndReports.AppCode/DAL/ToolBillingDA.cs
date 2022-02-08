using LNF.Repository;
using System;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class ToolBillingDA
    {
        public static DataSet GetToolBillingDataByClientID(DateTime period, int clientId)
        {
            return DataCommand.Create()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataSet("dbo.ToolBilling_Select");
        }

        public static DataSet GetToolBillingTempDataByClientID20110701(DateTime period, int clientId)
        {
            return DataCommand.Create()
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataSet("dbo.ToolBillingTemp20110701_Select");
        }

        public static DataSet GetToolBillingTempDataByClientID(DateTime period, int clientId)
        {
            return DataCommand.Create()
                .Param("Action", "ByClientIDPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .FillDataSet("dbo.ToolBillingTemp_Select");
        }
    }
}
