using LNF.Models.Data;
using sselIndReports.AppCode;
using sselIndReports.AppCode.DAL;
using System;
using System.Text;
using System.Data;

namespace sselIndReports
{
    public partial class DatAccount : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var dtAccounts = AccountDA.GetActiveAccountManagers();

            if (Request.QueryString["export"] == "csv")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Account Name,ShortCode,Project Number,Manager,Technical Field,Funding Source");
                foreach (DataRow dr in dtAccounts.Rows)
                {
                    sb.AppendLine(string.Join(",",
                        dr.Field<string>("AccountName"),
                        dr.Field<string>("ShortCode"),
                        dr.Field<string>("ProjectNumber"),
                        dr.Field<string>("ManagerDisplayName"),
                        dr.Field<string>("TechnicalFieldName"),
                        dr.Field<string>("FundingSourceName")));
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename=active-accounts-{0:yyyyMMddHHmmss}.csv", DateTime.Now));
                Response.Write(sb.ToString());
                Response.End();
            }
            else
            {
                rptAccount.DataSource = dtAccounts;
                rptAccount.DataBind();
                litCurrentTime.Text = DateTime.Now.ToString();
            }
        }
    }
}