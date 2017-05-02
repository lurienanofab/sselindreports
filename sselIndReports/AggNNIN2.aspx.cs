using GemBox.ExcelLite;
using LNF.CommonTools;
using LNF.Models.Data;
using sselIndReports.AppCode;
using sselIndReports.AppCode.DAL;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class AggNNIN2 : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator; }
        }

        public string DebugText { get; set; }

        public string XlsFilePath
        {
            get
            {
                if (ConfigurationManager.AppSettings["XlsFilePath"] == null || string.IsNullOrEmpty(ConfigurationManager.AppSettings["XlsFilePath"]))
                    return HttpContext.Current.Server.MapPath("~/Spreadsheets/NNIN/");
                else
                    return ConfigurationManager.AppSettings["XlsFilePath"];
            }
        }

        public bool Debug
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["Debug"]);
            }
        }

        public string CurrentFilesPattern
        {
            get
            {
                return string.Format("AS*_{0}.xls", litCurrentThreshold.Text);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litWarning.Text = string.Empty;

            if (!Page.IsPostBack)
            {
                litCurrentThreshold.Text = "240";
                Refresh();
                litDebug.Text = Debug ? DebugText : string.Empty;
            }
        }

        private void Refresh()
        {
            DateTime aggStartDate = GetAggStartDateBasedOnGeneratedReports();
            ppAgg.SelectedPeriod = aggStartDate;

            DateTime repDate = GetReportDateBasedOnGeneratedReports(aggStartDate);
            ppRep.SelectedPeriod = repDate;

            DataTable dt = CurrentFiles();
            if (dt.Rows.Count > 0)
            {
                litFileError.Text = string.Empty;
                rptCurrentFiles.Visible = true;
                rptCurrentFiles.DataSource = dt;
                rptCurrentFiles.DataBind();
                btnDeleteCheckedFiles.Visible = true;
            }
            else
            {
                litFileError.Text = string.Format(@"<div class=""nodata"">No files were found for a threshold of {0} minute{1}</div>", litCurrentThreshold.Text, Convert.ToInt32(litCurrentThreshold.Text).Equals(1) ? string.Empty : "s");
                rptCurrentFiles.Visible = false;
                btnDeleteCheckedFiles.Visible = false;
            }
        }

        protected void ppAgg_SelectedPeriodChanged(object sender, EventArgs e)
        {
            DateTime aggStartDate = ppAgg.SelectedPeriod;
            DateTime repDate = GetReportDateBasedOnGeneratedReports(aggStartDate);
            ppRep.SelectedPeriod = repDate;
        }

        private DateTime GetAggStartDateBasedOnGeneratedReports()
        {
            FileSystemInfo newestFile = NewestFileName(2000, string.Format("AS*_{0}.xls", litCurrentThreshold.Text));

            DateTime result;

            if (newestFile == null)
                result = DateTime.Now.Date.AddMonths(-1);
            else
            {
                ParsedFileName pfn = ParsedFileName.Parse(newestFile.Name);
                result = pfn.AggStartDate;
            }

            return result;
        }

        private DateTime GetReportDateBasedOnGeneratedReports(DateTime aggStartDate)
        {
            FileSystemInfo newestFile = NewestFileName(ppRep.StartPeriod.Year, string.Format("AS{0:yyyyMM}_Report*_{1}.xls", aggStartDate, litCurrentThreshold.Text));

            DebugText += string.Format(@"<div style=""padding-left: 10px;"">newestFile = ""{0}""</div>", newestFile);

            DateTime repDate;
            if (newestFile == null || newestFile.LastWriteTime.Year == ppRep.StartPeriod.Year) // no files found, new agg start
            {
                repDate = aggStartDate;
                ppRep.Enabled = false;
            }
            else
            {
                ParsedFileName pfn = ParsedFileName.Parse(newestFile.Name);
                repDate = pfn.ReportDate.AddMonths(1);
                ppRep.Enabled = true;
            }

            return repDate; //the maximum allowable date for the report
        }

        protected void btnReport_Command(object sender, CommandEventArgs e)
        {
            litWarning.Text = string.Empty;

            DateTime aggStartDate = ppAgg.SelectedPeriod;
            DateTime repDate = ppRep.SelectedPeriod;

            //first thing check if repDate is more than one year after aggStartDate - this causes errors in MakeReport (useCol will be zero because 12 is hard-coded)
            if ((repDate - aggStartDate).TotalDays >= 365)
            {
                litWarning.Text = @"<div class=""warning"">The report date must be within one year of the aggregation start date.</div>";
                return;
            }

            //simple checks
            if (aggStartDate > repDate)
            {
                litWarning.Text = @"<div class=""warning"">The aggregation start date must precede the report date.</div>";
                return;
            }

            DateTime maxRepDate = GetReportDateBasedOnGeneratedReports(aggStartDate);
            if (repDate > maxRepDate)
            {
                litWarning.Text = string.Format(@"<div class=""warning"">The report is too far in the future. The maximum allowed period is {0:MMMM yyyy}.</div>", maxRepDate);
                return;
            }

            //find out if today is within 4 business days of beginning of month
            DateTime businessDay = Utility.NextBusinessDay(repDate.AddMonths(1));

            if (DateTime.Now.Date < businessDay)
                litWarning.Text = @"<div class=""warning"">It's not the fourth business day of the reporting period. You should know the risk of using premature NNIN report</div>";

            bool makeAggData = chkForceRefresh.Checked || NNINDA.CumulativeUserExists(repDate);

            string xlsReportName = string.Format("AS{0:yyyyMM}_Report{1:yyyyMM}_{2}.xls", aggStartDate, repDate, litCurrentThreshold.Text);

            string newfilepath = MakeReport(aggStartDate, repDate, makeAggData, xlsReportName);

            Refresh();
        }

        private string MakeReport(DateTime aggStartDate, DateTime repDate, bool makeAggData, string xlsReportName)
        {
            //if the month is the aggStart use the blank template. Otherwise, use last months
            string xlsBaseFileName;
            if (aggStartDate == repDate)
                xlsBaseFileName = "NNIN Data Blank.xls";
            else
                xlsBaseFileName = string.Format("AS{0:yyyyMM}_Report{1:yyyyMM}_{2}.xls", aggStartDate, repDate.AddMonths(-1), litCurrentThreshold.Text);

            string xlsBaseFilePath = XlsFilePath + xlsBaseFileName;
            string xlsReportPath = XlsFilePath + xlsReportName;

            DateTime period = repDate;
            DateTime sDate = period;
            DateTime eDate = sDate.AddMonths(1);

            DataSet ds = NNINDA.GetTablesWithMinimumMinutes(period, (int)ClientPrivilege.LabUser, makeAggData, Convert.ToDouble(litCurrentThreshold.Text) / 60);

            ds.Tables[0].TableName = "TechnicalInterest_Hours";
            ds.Tables[1].TableName = "OrgType_Hours";
            ds.Tables[2].TableName = "TechnicalInterest_Users";
            ds.Tables[3].TableName = "OrgType_Users";
            ds.Tables[4].TableName = "TechnicalInterest_Fees";
            ds.Tables[5].TableName = "OrgType_Fees";

            DataColumn[] pk;

            pk = new DataColumn[] { ds.Tables["TechnicalInterest_Fees"].Columns["TechnicalInterestID"] };
            ds.Tables["TechnicalInterest_Fees"].PrimaryKey = pk;

            pk = new DataColumn[] { ds.Tables["OrgType_Fees"].Columns["OrgTypeID"] };
            ds.Tables["OrgType_Fees"].PrimaryKey = pk;

            //get raw cumulative user data
            DataTable dtCumUser = NNINDA.GetCumulativeUserAggregateData(aggStartDate, repDate);

            DataTable dtReport = new DataTable();
            dtReport.Columns.Add("BaseRow", typeof(int));
            dtReport.Columns.Add("TotalRow", typeof(int));
            dtReport.Columns.Add("RepType", typeof(string)); //fees, hours, cum, 
            dtReport.Columns.Add("ByType", typeof(string)); //orgtype, techid, none
            dtReport.Columns.Add("DateOnly", typeof(bool)); //orgtype, techid, none

            AddReportRow(dtReport, 10, 0, string.Empty, string.Empty, true);
            AddReportRow(dtReport, 24, 43, "Hours", "TechnicalInterest", false);
            AddReportRow(dtReport, 11, 43, "Hours", "OrgType", false);
            AddReportRow(dtReport, 40, 0, string.Empty, string.Empty, true);
            AddReportRow(dtReport, 69, 88, "Users", "TechnicalInterest", false);
            AddReportRow(dtReport, 55, 88, "Users", "OrgType", false);
            AddReportRow(dtReport, 85, 0, string.Empty, string.Empty, true);
            AddReportRow(dtReport, 161, 180, "Fees", "TechnicalInterest", false);
            AddReportRow(dtReport, 146, 180, "Fees", "OrgType", false);
            AddReportRow(dtReport, 177, 0, string.Empty, string.Empty, true);
            AddReportRow(dtReport, 116, 136, "CumUser", "TechnicalInterest", false);
            AddReportRow(dtReport, 101, 136, "CumUser", "OrgType", false);
            AddReportRow(dtReport, 133, 0, string.Empty, string.Empty, true);
            AddReportRow(dtReport, 226, 0, "CumUser", "DemGender", false);
            AddReportRow(dtReport, 232, 0, "CumUser", "DemEthnic", false);
            AddReportRow(dtReport, 238, 0, "CumUser", "DemRace", false);
            AddReportRow(dtReport, 247, 0, "CumUser", "DemDisability", false);

            ExcelLite.SetLicense("EL6N-Z669-AZZG-3LS7");
            ExcelFile SpreadSheet = new ExcelFile();
            SpreadSheet.LoadXls(xlsBaseFilePath);
            ExcelWorksheet ws = SpreadSheet.Worksheets["UM SSEL"];

            DataRow[] fdr;
            int lastVal, useCol = 0, useRow;
            double hardTotals; //for recording internal time without formula
            foreach (DataRow drReport in dtReport.Rows)
            {
                //write dates
                for (int j = 0; j < 12; j++)
                {
                    ws.Cells[Convert.ToInt32(drReport["BaseRow"]), j + 1].Value = aggStartDate.AddMonths(j);
                    if (aggStartDate.AddMonths(j) == repDate)
                        useCol = j + 1;
                }

                if (!Convert.ToBoolean(drReport["DateOnly"]))
                {
                    hardTotals = 0;
                    if (drReport["RepType"].ToString() == "CumUser")
                    {
                        fdr = dtCumUser.Select(string.Format("TableName = '{0}'", drReport["ByType"]));
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
                                    if (!string.IsNullOrEmpty(ws.Cells[useRow, i].Formula))
                                    {
                                        string form = ws.Cells[useRow, i].Formula;
                                        lastVal += Convert.ToInt32(form.Substring(form.IndexOf("+") + 1));
                                    }
                                }
                                string formula = "=" + Convert.ToChar(64 + useCol) + (useRow + 1).ToString() + "+" + (Convert.ToInt32(fdr[j]["Count"]) - lastVal).ToString();
                                ws.Cells[useRow, useCol].Formula = formula;
                            }
                            hardTotals += Convert.ToDouble(fdr[j]["Count"]);
                        }
                        if (Convert.ToInt32(drReport["TotalRow"]) != 0)
                            ws.Cells[Convert.ToInt32(drReport["TotalRow"]), useCol].Value = hardTotals;
                    }
                    else
                    {
                        foreach (DataRow dr in ds.Tables[string.Format("{0}_{1}", drReport["ByType"], drReport["RepType"])].Rows)
                        {
                            useRow = drReport.Field<int>("BaseRow") + dr.Field<int>(drReport["ByType"].ToString() + "ID");
                            double val = Convert.ToDouble(dr[drReport["RepType"].ToString()]); //must use Convert.ToDouble here because dr[...] may be different types depending on the rep type (e.g. double or int)
                            ws.Cells[useRow, useCol].Value = val;
                            hardTotals += val;
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

        private void AddReportRow(DataTable dt, int baseRow, int totalRow, string repType, string byType, bool dateOnly)
        {
            DataRow ndr = dt.NewRow();
            ndr.SetField("BaseRow", baseRow);
            ndr.SetField("TotalRow", totalRow);
            ndr.SetField("RepType", repType);
            ndr.SetField("ByType", byType);
            ndr.SetField("DateOnly", dateOnly);
            dt.Rows.Add(ndr);
        }

        public FileSystemInfo NewestFileName(int startYear, string pattern)
        {
            FileSystemInfo result = null;
            DateTime newestFileDate = new DateTime(startYear, 1, 1); //init to earliest possible date
            DirectoryInfo dirInfo = new DirectoryInfo(XlsFilePath);
            DebugText += string.Format(@"<div style=""padding-left: 10px;"">Looking for: ""{0}""</div>", pattern);

            foreach (FileSystemInfo fileInfo in dirInfo.GetFileSystemInfos(pattern))
            {
                DebugText += string.Format(@"<div style=""padding-left: 20px;"">newestFileDate = ""{0:yyyy-MM-dd HH:mm:ss}""</div>", newestFileDate);
                DebugText += string.Format(@"<div style=""padding-left: 20px;"">myFile = [Name = ""{0}"", LastWriteTime = ""{1:yyyy-MM-dd HH:mm:ss}""]</div>", fileInfo.Name, fileInfo.LastWriteTime);
                if (fileInfo.LastWriteTime > newestFileDate)
                {
                    result = fileInfo;
                    newestFileDate = fileInfo.LastWriteTime;
                }
            }

            return result;
        }

        public DataTable CurrentFiles()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("CreationTime", typeof(string));
            dt.Columns.Add("LastWriteTime", typeof(string));

            DirectoryInfo dirInfo = new DirectoryInfo(XlsFilePath);
            int id = 0;
            foreach (FileSystemInfo fileInfo in dirInfo.GetFileSystemInfos(CurrentFilesPattern))
            {
                dt.Rows.Add(id, fileInfo.Name, fileInfo.CreationTime, fileInfo.LastWriteTime);
                id += 1;
            }

            return dt;
        }

        protected void btnDeleteCheckedFiles_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptCurrentFiles.Items)
            {
                CheckBox chk = (CheckBox)item.FindControl("chkCheckFile");
                HyperLink hyp = (HyperLink)item.FindControl("hypFileName");
                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        string filePath = XlsFilePath + hyp.Text;
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            litFileError.Text = string.Format(@"<div class=""file-error"">{0}</div>", ex.Message);
                        }
                    }
                }
            }

            Refresh();
        }

        protected string GetFileURL(object fileName)
        {
            if (fileName == DBNull.Value || string.IsNullOrEmpty(fileName.ToString())) return "#";
            string result = VirtualPathUtility.ToAbsolute("~/Spreadsheets/NNIN/" + fileName.ToString());
            return result;
        }

        protected void btnChangeThreshold_Click(object sender, EventArgs e)
        {
            int CurrentThreshold = 240;
            int temp;
            if (int.TryParse(txtMinMinutes.Text, out temp))
                CurrentThreshold = temp;
            litCurrentThreshold.Text = CurrentThreshold.ToString();
            txtMinMinutes.Text = string.Empty;
            Refresh();
        }
    }
}