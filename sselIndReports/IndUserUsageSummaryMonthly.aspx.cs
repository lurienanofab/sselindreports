using LNF.Models.Data;
using sselIndReports.AppCode;

namespace sselIndReports
{
    public partial class IndUserUsageSummaryMonthly : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return 0; }
        }
    }
}