using LNF.Impl.Testing;
using Moq;
using System.Collections;
using System.Collections.Specialized;
using System.Web;

namespace sselIndReports.Tests
{
    public class ReportsTestManager : ContextManager
    {
        public ReportsTestManager(string ipaddr = null, string username = null, IDictionary contextItems = null, SessionItemCollection sessionItems = null, NameValueCollection queryString = null)
            : base(ipaddr, username, contextItems, sessionItems, queryString) { }

        public override void ConfigureMockSession(Mock<HttpSessionStateBase> session)
        {
            session.SetupSet(x => x["UserUsageSummaryTables"] = It.IsAny<object>()).Callback<string, object>(SessionItems.Set);
            session.SetupSet(x => x["UserUsageSummaryTables20110701"] = It.IsAny<object>()).Callback<string, object>(SessionItems.Set);
        }
    }
}
