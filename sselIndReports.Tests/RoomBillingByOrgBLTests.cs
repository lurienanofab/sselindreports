using Microsoft.VisualStudio.TestTools.UnitTesting;
using sselIndReports.AppCode.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace sselIndReports.Tests
{
    [TestClass]
    public class RoomBillingByOrgBLTests
    {
        [TestMethod]
        public void CanGetDataByPeriodAndClientID()
        {
            using (var test = new ReportsTestManager())
            {
                DateTime period = DateTime.Parse("2019-02-01");
                int clientId;
                DataTable dt;
                DataTable expected;

                clientId = 3592;
                dt = RoomBillingByOrgBL.GetDataByPeriodAndClientID(test.ContextBase, period.Year, period.Month, clientId);
                expected = GetExpectedTable(
                    new object[] { 39, "Wayne State University", 40M, 0M, 40M, 0M });
                AssertDataTablesAreEqual(expected, dt);

                test.ContextBase.Session.Remove("UserUsageSummaryTables20110701");

                clientId = 81;
                dt = RoomBillingByOrgBL.GetDataByPeriodAndClientID(test.ContextBase, period.Year, period.Month, clientId);
                expected = GetExpectedTable(
                    new object[] { 221, "MicroKosmos LLC", 192M, 0M, 192M, 0M },
                    new object[] { 17, "University of Michigan", 0M, 0M, 0M, 0M });
                AssertDataTablesAreEqual(expected, dt);

                test.ContextBase.Session.Remove("UserUsageSummaryTables20110701");

                clientId = 2691;
                dt = RoomBillingByOrgBL.GetDataByPeriodAndClientID(test.ContextBase, period.Year, period.Month, clientId);
                expected = GetExpectedTable(
                    new object[] { 291, "Hawk Semiconductor, LLC", 498M, 0M, 498M, 0M },
                    new object[] { 50, "Numed Technologies, LLC", 0M, 0M, 0M, 0M },
                    new object[] { 30, "Evigia Systems, Inc.", 0M, 0M, 0M, 0M },
                    new object[] { 240, "Houston Methodist Research Institute", 696M, 0M, 696M, 0M },
                    new object[] { 249, "NVision Labs", 0M, 0M, 0M, 0M },
                    new object[] { 17, "University of Michigan", 578M, 0M, 578M, 0M },
                    new object[] { 281, "Micro Inertial LLC", 81M, 0M, 81M, 0M });
                AssertDataTablesAreEqual(expected, dt);
            }
        }

        private void AssertDataTablesAreEqual(DataTable expected, DataTable actual)
        {
            Assert.AreEqual(expected.Rows.Count, actual.Rows.Count);

            for (int i = 0; i < expected.Rows.Count; ++i)
            {
                var row = expected.Rows[i];
                foreach (DataColumn col in expected.Columns)
                {
                    Assert.IsTrue(actual.Columns.Contains(col.ColumnName));
                    Assert.AreEqual(col.DataType, actual.Columns[col.ColumnName].DataType);

                    object expectedValue = row[col.ColumnName];
                    object actualValue = actual.Rows[i][col.ColumnName];

                    Assert.AreEqual(expectedValue.GetType(), actualValue.GetType());
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }
        }

        private DataTable GetExpectedTable(params object[][] rows)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("OrgID", typeof(int));
            dt.Columns.Add("OrgName", typeof(string));
            dt.Columns.Add("RoomCharge", typeof(decimal));
            dt.Columns.Add("RoomMisc", typeof(decimal));
            dt.Columns.Add("TotalUsageCharge", typeof(decimal));
            dt.Columns.Add("SubsidyDiscount", typeof(decimal));

            foreach (var r in rows)
            {
                dt.Rows.Add(r);
            }

            return dt;
        }
    }
}
