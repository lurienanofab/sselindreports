using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace sselIndReports.Tests
{
    [TestClass]
    public class UserUsageSummaryTests
    {
        [TestMethod]
        public void UserUsageSummaryTests_CanPopulateToolDetailData()
        {
            var page = new IndUserUsageSummary20111101();
            page.PopulateToolDetailData(DateTime.Parse("2016-04-01"), 81);
        }
    }
}
