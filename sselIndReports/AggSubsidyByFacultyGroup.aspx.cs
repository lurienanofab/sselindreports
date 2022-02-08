using LNF.Data;
using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using sselIndReports.AppCode.DAL;
using System;

namespace sselIndReports
{
    public partial class AggSubsidyByFacultyGroup : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator; }
        }

        public int ManagerOrgID
        {
            get { return Convert.ToInt32(ddlManager.SelectedValue); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DateTime now = DateTime.Now.Date;
                DateTime nowPeriod = new DateTime(now.Year, now.Month, 1);
                DateTime endPeriod = nowPeriod.AddMonths(-1);
                ppStart.SelectedPeriod = endPeriod;
                ppEnd.SelectedPeriod = endPeriod;
                FillManagerDropDown();
            }
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            gv.DataSource = TieredSubsidyBillingDA.GetAggSubsidy(ppStart.SelectedPeriod, ppEnd.SelectedPeriod, ManagerOrgID);
            gv.DataBind();
        }

        protected void gv_DataBound(object sender, EventArgs e)
        {
            gv.Rows[gv.Rows.Count - 1].BackColor = System.Drawing.Color.RosyBrown;
        }

        protected void ddlStartPeriodYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillManagerDropDown();
        }

        protected void ddlStartPeriodMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillManagerDropDown();
        }

        private void FillManagerDropDown()
        {
            ddlManager.DataSource = ClientAccountManager.GetManagersByPeriod(ppStart.SelectedPeriod, 5);
            ddlManager.DataBind();
        }
    }
}
