using GemBox.ExcelLite;
using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using sselIndReports.AppCode;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class AggNNIN : ReportPage
    {
        private string xlsFilePath = HttpContext.Current.Server.MapPath("~/Spreadsheets/");

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblWarning.Visible = false;

            if (!Page.IsPostBack)
            {
                //filename format: "AS" + Format(aggStart, "yyyyMM") + "_Report" + Format(repStart, "yyyyMM") + ".xls"
                string newestFile = string.Empty;
                DateTime newestFileDate = new DateTime(2000, 1, 1);
                DirectoryInfo dir = new DirectoryInfo(xlsFilePath);
                foreach (FileSystemInfo file in dir.GetFileSystemInfos("AS*.xls"))
                {
                    if (file.CreationTime > newestFileDate)
                    {
                        newestFile = file.Name;
                        newestFileDate = file.CreationTime;
                    }
                }

                DateTime aggStartDate;
                if (string.IsNullOrEmpty(newestFile))
                    aggStartDate = DateTime.Now.AddMonths(-1);
                else
                    aggStartDate = DateTime.Parse(newestFile.Substring(6, 2) + "/1/" + newestFile.Substring(2, 4));

                DateTime repDate = SetReportDate(aggStartDate);
                ppRep.SelectedPeriod = repDate;
                ppAgg.SelectedPeriod = aggStartDate;
            }
        }

        protected void ppAgg_SelectedPeriodChanged(object sender, EventArgs e)
        {
            DateTime aggStartDate = ppAgg.SelectedPeriod;
            DateTime repDate = SetReportDate(aggStartDate);
            ppRep.SelectedPeriod = repDate;
        }

        private DateTime SetReportDate(DateTime aggStartDate)
        {
            string newestFile = string.Empty;
            DateTime newestFileDate = new DateTime(2000, 1, 1);
            DirectoryInfo dirInfo = new DirectoryInfo(xlsFilePath);
            foreach (FileSystemInfo fileInfo in dirInfo.GetFileSystemInfos("AS" + aggStartDate.ToString("yyyyMM") + "*.xls"))
            {
                if (fileInfo.CreationTime > newestFileDate)
                {
                    newestFile = fileInfo.Name;
                    newestFileDate = fileInfo.CreationTime;
                }
            }

            DateTime repDate;
            if (newestFileDate.Year == 2000) //no files found, new agg start
            {
                repDate = aggStartDate;
                ppRep.Enabled = false;
            }
            else
            {
                repDate = DateTime.Parse(newestFile.Substring(19, 2) + "/1/" + newestFile.Substring(15, 4));
                repDate = repDate.AddMonths(1);
                ppRep.Enabled = true;
            }

            return repDate; //the maximum allowable date for the report
        }

        protected void btnReport_Command(object sender, CommandEventArgs e)
        {
            DateTime aggStartDate = ppAgg.SelectedPeriod;
            DateTime repDate = ppRep.SelectedPeriod;

            //simple checks
            if (aggStartDate > repDate)
            {
                lblWarning.Text = "The aggregation start date must precede the report date.";
                lblWarning.Visible = true;
                return;
            }

            if (repDate > SetReportDate(aggStartDate))
            {
                lblWarning.Text = "The report is too far in the future.";
                lblWarning.Visible = true;
                return;
            }

            //find out if today is within 4 business days of beginning of month
            var sd = repDate.AddMonths(1);
            var ed = sd.AddMonths(1);
            DateTime businessDay = Utility.NextBusinessDay(sd, Utility.GetHolidays(sd, ed));

            if (DateTime.Now < businessDay)
            {
                lblWarning.Text = "It's not the fourth business day of the reporting period. You should know the risk of using premature NNIN report";
                lblWarning.Visible = true;
            }

            //need to check to see if data exists for selected report month
            bool makeAggData = DA.Command()
                .Param("Action", "DataCheck")
                .Param("eDate", repDate)
                .ExecuteScalar<bool>("dbo.CumUser_Select").Value;

            //clean up old files
            DirectoryInfo dirInfo = new DirectoryInfo(xlsFilePath);
            foreach (FileSystemInfo fileInfo in dirInfo.GetFileSystemInfos("AS*.xls"))
            {
                if ((DateTime.Now - fileInfo.CreationTime).TotalDays > 480)
                    fileInfo.Delete();
            }

            string xlsReportName = "AS" + aggStartDate.ToString("yyyyMM") + "_Report" + repDate.ToString("yyyyMM") + ".xls";

            string newfilepath = MakeReport(aggStartDate, repDate, makeAggData, xlsReportName);

            //Display excel spreadsheet
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "filename=" + xlsReportName);
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = string.Empty;
            EnableViewState = false;

            if (File.Exists(xlsFilePath + xlsReportName))
                Response.WriteFile(xlsFilePath + xlsReportName);
            else
                Response.WriteFile(newfilepath);

            repDate = SetReportDate(aggStartDate);
            ppRep.SelectedPeriod = repDate;

            Response.End();
        }

        private string MakeReport(DateTime aggStartDate, DateTime repDate, bool makeAggData, string xlsReportName)
        {
            //if the month is the aggStart use the blank template. Otherwise, use last months
            string xlsBaseFileName;
            if (aggStartDate == repDate)
                xlsBaseFileName = "NNIN Data Blank.xls";
            else
                xlsBaseFileName = "AS" + aggStartDate.ToString("yyyyMM") + "_Report" + repDate.AddMonths(-1).ToString("yyyyMM") + ".xls";

            string xlsBaseFilePath = xlsFilePath + xlsBaseFileName;
            string xlsReportPath = xlsFilePath + xlsReportName;

            DateTime period = repDate;
            DateTime sDate = period;
            DateTime eDate = sDate.AddMonths(1);
            Compile compile = new Compile();

            //this gets the aggregated costs for rooms and tools
            DataSet dsCostTables = NNINDA.GetCostTables(period);
            DataTable dtRoomCost = dsCostTables.Tables[0];
            DataTable dtToolCost = dsCostTables.Tables[1];

            //get Client info
            DataSet ds = DA.Command()
                .Param("Action", "GetAllTables")
                .Param("Period", period)
                .Param("Privs", (int)ClientPrivilege.LabUser)
                .Param("MakeCumUser", makeAggData)
                .FillDataSet("dbo.NNIN_Select");

            ds.Tables[0].TableName = "ClientTechInt";
            ds.Tables[1].TableName = "AcctOrgType";
            ds.Tables[2].TableName = "TechnicalInterest_Hours";
            ds.Tables[3].TableName = "OrgType_Hours";
            ds.Tables[4].TableName = "TechnicalInterest_Users";
            ds.Tables[5].TableName = "OrgType_Users";
            ds.Tables[6].TableName = "TechnicalInterest_Fees";
            ds.Tables[7].TableName = "OrgType_Fees";

            DataColumn[] pk;

            pk = new DataColumn[] { ds.Tables["TechnicalInterest_Fees"].Columns["TechnicalInterestID"] };
            ds.Tables["TechnicalInterest_Fees"].PrimaryKey = pk;

            pk = new DataColumn[] { ds.Tables["OrgType_Fees"].Columns["OrgTypeID"] };
            ds.Tables["OrgType_Fees"].PrimaryKey = pk;

            //calc fees by tech interest and orgtype
            DataRow drTI, drOT;
            DataRow[] fdr;
            foreach (DataTable dt in dsCostTables.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    fdr = ds.Tables["ClientTechInt"].Select(string.Format("ClientID = {0}", dr["ClientID"]));
                    if (fdr.Length > 0)
                    {
                        drTI = ds.Tables["TechnicalInterest_Fees"].Rows.Find(fdr[0]["TechnicalInterestID"]);

                        drTI["Fees"] = drTI.Field<double>("Fees") + dr.Field<double>("TotalCalcCost");
                        fdr = ds.Tables["AcctOrgType"].Select(string.Format("AccountID = {0}", dr["AccountID"]));
                        if (fdr.Length > 0)
                        {
                            drOT = ds.Tables["OrgType_Fees"].Rows.Find(fdr[0]["OrgTypeID"]);
                            drOT["Fees"] = drOT.Field<double>("Fees") + dr.Field<double>("TotalCalcCost");
                        }
                    }
                }
            }

            //get raw cumulative user data
            DA.Command()
                .Param("Action", "Aggregate")
                .Param("sDate", aggStartDate)
                .Param("eDate", repDate)
                .FillDataSet(ds, "dbo.CumUser_Select", "CumUser");

            DataTable dtReport = new DataTable();
            dtReport.Columns.Add("BaseRow", typeof(int));
            dtReport.Columns.Add("TotalRow", typeof(int));
            dtReport.Columns.Add("RepType", typeof(string)); //fees, hours, cum
            dtReport.Columns.Add("ByType", typeof(string)); //orgtype, techid, none
            dtReport.Columns.Add("DateOnly", typeof(bool)); //orgtype, techid, none

            AddReportRow(dtReport, new object[] { 10, 0, string.Empty, string.Empty, true });
            AddReportRow(dtReport, new object[] { 24, 43, "Hours", "TechnicalInterest", false });
            AddReportRow(dtReport, new object[] { 11, 43, "Hours", "OrgType", false });
            AddReportRow(dtReport, new object[] { 40, 0, string.Empty, string.Empty, true });
            AddReportRow(dtReport, new object[] { 69, 88, "Users", "TechnicalInterest", false });
            AddReportRow(dtReport, new object[] { 55, 88, "Users", "OrgType", false });
            AddReportRow(dtReport, new object[] { 85, 0, string.Empty, string.Empty, true });
            AddReportRow(dtReport, new object[] { 161, 180, "Fees", "TechnicalInterest", false });
            AddReportRow(dtReport, new object[] { 146, 180, "Fees", "OrgType", false });
            AddReportRow(dtReport, new object[] { 177, 0, string.Empty, string.Empty, true });
            AddReportRow(dtReport, new object[] { 116, 136, "CumUser", "TechnicalInterest", false });
            AddReportRow(dtReport, new object[] { 101, 136, "CumUser", "OrgType", false });
            AddReportRow(dtReport, new object[] { 133, 0, string.Empty, string.Empty, true });
            AddReportRow(dtReport, new object[] { 226, 0, "CumUser", "DemGender", false });
            AddReportRow(dtReport, new object[] { 232, 0, "CumUser", "DemEthnic", false });
            AddReportRow(dtReport, new object[] { 238, 0, "CumUser", "DemRace", false });
            AddReportRow(dtReport, new object[] { 247, 0, "CumUser", "DemDisability", false });

            ExcelLite.SetLicense("EL6N-Z669-AZZG-3LS7");
            ExcelFile SpreadSheet = new ExcelFile();
            SpreadSheet.LoadXls(xlsBaseFilePath);
            ExcelWorksheet ws = SpreadSheet.Worksheets["UM SSEL"];

            int lastVal, useCol = 0, useRow;
            double hardTotals; //for recording internal time without formula
            foreach (DataRow drReport in dtReport.Rows)
            {
                //write dates
                for (int j = 0; j < 12; j++)
                {
                    ws.Cells[Convert.ToInt32(drReport["BaseRow"]), j + 1].Value = aggStartDate.AddMonths(j);
                    if (aggStartDate.AddMonths(j) == repDate) useCol = j + 1;
                }

                if (!Convert.ToBoolean(drReport["DateOnly"]))
                {
                    hardTotals = 0;
                    if (drReport["RepType"].ToString() == "CumUser")
                    {
                        fdr = ds.Tables["CumUser"].Select(string.Format("TableName = '{0}'", drReport["ByType"]));
                        for (int j = 0; j < fdr.Length; j++)
                        {
                            useRow = drReport.Field<int>("BaseRow") + fdr[j].Field<int>("Value");
                            if (aggStartDate == repDate)
                                ws.Cells[useRow, useCol].Value = fdr[j]["Count"];
                            else
                            {
                                lastVal = Convert.ToInt32(ws.Cells[useRow, 1].Value);
                                for (int i = 2; i < useCol; i++)
                                {
                                    string form = ws.Cells[useRow, i].Formula;
                                    if (!string.IsNullOrEmpty(form))
                                        lastVal += Convert.ToInt32(form.Substring(form.IndexOf("+") + 1));
                                }
                                ws.Cells[useRow, useCol].Formula = "=" + Convert.ToChar(64 + useCol) + (useRow + 1).ToString() + "+" + (Convert.ToInt32(fdr[j]["Count"]) - lastVal).ToString();
                            }

                            hardTotals += fdr[j].Field<double>("Count");
                        }

                        if (Convert.ToInt32(drReport["TotalRow"]) != 0)
                            ws.Cells[Convert.ToInt32(drReport["TotalRow"]), useCol].Value = hardTotals;
                    }
                    else
                    {
                        foreach (DataRow dr in ds.Tables[string.Format("{0}_{1}", drReport["ByType"], drReport["RepType"])].Rows)
                        {
                            useRow = drReport.Field<int>("BaseRow") + dr.Field<int>(string.Format("{0}ID", drReport["ByType"]));
                            ws.Cells[useRow, useCol].Value = dr[drReport["RepType"].ToString()];
                            hardTotals += Convert.ToDouble(dr[drReport["RepType"].ToString()]);
                        }

                        ws.Cells[Convert.ToInt32(drReport["TotalRow"]), useCol].Value = hardTotals;
                    }
                }
            }

            SpreadSheet.SaveXls(xlsReportPath);
            SpreadSheet = null;
            System.GC.Collect();

            return xlsReportPath;
        }

        private void AddReportRow(DataTable dt, object[] array)
        {
            DataRow dr = dt.NewRow();
            dr.ItemArray = array;
            dt.Rows.Add(dr);
        }
    }
}