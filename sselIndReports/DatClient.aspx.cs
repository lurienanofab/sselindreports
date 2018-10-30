using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using sselIndReports.AppCode;
using sselIndReports.AppCode.BLL;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class DatClient : ReportPage
    {
        private DataSet dsReport;

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.Executive | ClientPrivilege.Administrator | ClientPrivilege.FinancialAdmin; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                dsReport = CacheManager.Current.CacheData();
                if (dsReport == null)
                    Response.Redirect("~");
                else if (dsReport.DataSetName != "DatClient")
                    Response.Redirect("~");
            }
            else
            {
                dsReport = new DataSet("DatClient");

                //get Role info
                DA.Command()
                    .Param("TableName", "Role")
                    .FillDataSet(dsReport, "dbo.Global_Select", "Role");

                //get UserType info
                DA.Command()
                    .Param("TableName", "Community")
                    .FillDataSet(dsReport, "dbo.Global_Select", "Communities");

                //grab all departments
                DA.Command()
                    .Param("Action", "All")
                    .FillDataSet(dsReport, "dbo.Department_Select", "Department");

                CacheManager.Current.CacheData(dsReport);

                UpdateClientDDL();
            }
        }

        protected void ReportButton_Click(object sender, EventArgs e)
        {
            MakeClientReport();
        }

        protected void pp1_SelectedPeriodChanged(object sender, EventArgs e)
        {
            UpdateClientDDL();
            ClearReport();
        }
        private bool IsDDLUserEmptyOrZero()
        {
            if ("" == ddlUser.SelectedValue || "0" == ddlUser.SelectedValue)
                return true;
            else
                return false;
        }
        private void UpdateClientDDL()
        {
            int selectedClientId = 0;

            if (!IsDDLUserEmptyOrZero())
            {
                selectedClientId = Convert.ToInt32(ddlUser.SelectedValue);
            }

            PopulateUserDropDownList(ddlUser, pp1.SelectedPeriod, btnReport, true);
            // The following is to generate report
            DateTime sDate = pp1.SelectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            //need to empty tables
            if (dsReport.Tables.Contains("ClientOrg")) dsReport.Tables.Remove(dsReport.Tables["ClientOrg"]);
            if (dsReport.Tables.Contains("ClientManager")) dsReport.Tables.Remove(dsReport.Tables["ClientManager"]);
            if (dsReport.Tables.Contains("Org")) dsReport.Tables.Remove(dsReport.Tables["Org"]);
            if (dsReport.Tables.Contains("Account")) dsReport.Tables.Remove(dsReport.Tables["Account"]);
            if (dsReport.Tables.Contains("ClientAccount")) dsReport.Tables.Remove(dsReport.Tables["ClientAccount"]);
            if (dsReport.Tables.Contains("Client")) dsReport.Tables.Remove(dsReport.Tables["Client"]);
            if (dsReport.Tables.Contains("DemCitizen")) dsReport.Tables.Remove(dsReport.Tables["DemCitizen"]);
            if (dsReport.Tables.Contains("DemDisability")) dsReport.Tables.Remove(dsReport.Tables["DemDisability"]);
            if (dsReport.Tables.Contains("DemEthnic")) dsReport.Tables.Remove(dsReport.Tables["DemEthnic"]);
            if (dsReport.Tables.Contains("DemGender")) dsReport.Tables.Remove(dsReport.Tables["DemGender"]);
            if (dsReport.Tables.Contains("DemRace")) dsReport.Tables.Remove(dsReport.Tables["DemRace"]);

            //display name column is appended to facilitate manager display
            DA.Command()
                .Param("Action", "AllActive")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataSet(dsReport, "dbo.ClientOrg_Select", "ClientOrg");

            dsReport.Tables["ClientOrg"].PrimaryKey = new[] { dsReport.Tables["ClientOrg"].Columns["ClientOrgID"] };

            //Manager info
            DA.Command()
                .Param("Action", "AllActive")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataSet(dsReport, "dbo.ClientManager_Select", "ClientManager");

            //get Org info
            DA.Command()
                .MapSchema()
                .Param("Action", "AllActive")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataSet(dsReport, "dbo.Org_Select", "Org");

            dsReport.Tables["Org"].PrimaryKey = new[] { dsReport.Tables["Org"].Columns["OrgID"] };

            //get Account info
            DA.Command()
                .Param("Action", "AllActive")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataSet(dsReport, "dbo.Account_Select", "Account");

            dsReport.Tables["Account"].PrimaryKey = new[] { dsReport.Tables["Account"].Columns["AccountID"] };

            //get ClientAccount info
            DA.Command()
                .Param("Action", "AllActive")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataSet(dsReport, "dbo.ClientAccount_Select", "ClientAccount");

            //Client info - gets put into ddl, not needed in dataset
            DA.Command()
                .Param("Action", "All")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .FillDataSet(dsReport, "dbo.Client_Select", "Client");

            dsReport.Tables["Client"].PrimaryKey = new DataColumn[] { dsReport.Tables["Client"].Columns["ClientID"] };

            //fill in demographics RBL - could be prettier...

            var command = DA.Command().MapSchema(); //select command

            command.Param("Action", "All");

            command.Param("DemType", "DemCitizen");
            command.FillDataSet(dsReport, "dbo.Dem_Select", "DemCitizen");
            dsReport.Tables["DemCitizen"].PrimaryKey = new[] { dsReport.Tables["DemCitizen"].Columns["DemCitizenID"] };

            command.Param("DemType", "DemDisability");
            command.FillDataSet(dsReport, "dbo.Dem_Select", "DemDisability");
            dsReport.Tables["DemDisability"].PrimaryKey = new[] { dsReport.Tables["DemDisability"].Columns["DemDisabilityID"] };

            command.Param("DemType", "DemEthnic");
            command.FillDataSet(dsReport, "dbo.Dem_Select", "DemEthnic");
            dsReport.Tables["DemEthnic"].PrimaryKey = new[] { dsReport.Tables["DemEthnic"].Columns["DemEthnicID"] };

            command.Param("DemType", "DemGender");
            command.FillDataSet(dsReport, "dbo.Dem_Select", "DemGender");
            dsReport.Tables["DemGender"].PrimaryKey = new[] { dsReport.Tables["DemGender"].Columns["DemGenderID"] };

            command.Param("DemType", "DemRace");
            command.FillDataSet(dsReport, "dbo.Dem_Select", "DemRace");
            dsReport.Tables["DemRace"].PrimaryKey = new[] { dsReport.Tables["DemRace"].Columns["DemRaceID"] };

            CacheManager.Current.CacheData(dsReport);
        }

        private string GetOrgName(int orgId)
        {
            var org = DA.Current.Single<Org>(orgId);

            if (org == null)
            {
                // real mystery here, it just doesn't exist
                return string.Format("unknown:{0}", orgId);
            }
            else
            {
                if (org.Active)
                    return org.OrgName; //this will probably never happen because the DataRow should have been found, this is here only to cover all possibilities
                else
                    return string.Format("{0} <span style=\"color: #ff0000;\">(INACTIVE)</span>", org.OrgName);
            }
        }

        private void ClearReport()
        {
            HtmlTable myTable = (HtmlTable)FindControlRecursive("tblCliRep");
            myTable.Rows.Clear();
        }
        private void MakeClientReport()
        {
            //ClearReport();
            if (IsDDLUserEmptyOrZero()) // (ddlUser.SelectedValue == "") 
            {
                trowName.Visible = false;
                trowPrivs.Visible = false;
                trowDem.Visible = false;
                trowType.Visible = false;
                return;
            }

            trowName.Visible = true;
            trowPrivs.Visible = true;
            trowDem.Visible = true;
            trowType.Visible = true;

            int clientId = Convert.ToInt32(ddlUser.SelectedValue);
            DataRow cdr = dsReport.Tables["Client"].Rows.Find(clientId);

            HtmlTable myTable = (HtmlTable)FindControlRecursive("tblCliRep");

            if (null == cdr)
            {
                myTable.Rows.Clear();
                HtmlTableRow tblRowE = new HtmlTableRow();
                HtmlTableCell tblCellE = new HtmlTableCell();
                Label lblInfoE = new Label();

                lblInfoE.Text = "--no data--";

                lblInfoE.CssClass = "LabelText";
                tblCellE.Controls.Add(lblInfoE);
                tblRowE.Cells.Add(tblCellE);
                myTable.Rows.Insert(0, tblRowE);
                return;
            }

            //first, show the full name and site user name
            lblClientName.Text = GetClientName(cdr);
            lblClientName.ForeColor = System.Drawing.Color.Red;

            //then, list site privileges, need to check each bit individually
            lblPrivs.Text = GetSitePrivileges(cdr);

            //show the demograhic info
            lblDem.Text = GetDemographicInfo(cdr);

            //show Usertype
            lblUserType.Text = GetUserType(cdr);

            //need dates to get memberships at specified time
            DateTime sDate = pp1.SelectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            //starting with row 4 (fifth row), add info per org
            int rowCntr = 4;

            HtmlTableRow tblRow;
            HtmlTableCell tblCell;
            Label lblInfo;

            DataRow codr;
            DataRow[] codrs = dsReport.Tables["ClientOrg"].Select(string.Format("ClientID = {0}", clientId));
            for (int i = 0; i < codrs.Length; i++)
            {
                codr = codrs[i];

                //2008-02-07 Add billing type info
                tblRow = new HtmlTableRow();
                tblCell = new HtmlTableCell();
                lblInfo = new Label();

                lblInfo.Text = "-------------------------------------------------------------";
                lblInfo.CssClass = "LabelText";
                tblCell.Controls.Add(lblInfo);
                tblRow.Cells.Add(tblCell);
                myTable.Rows.Insert(rowCntr, tblRow);

                rowCntr += 1;

                tblRow = new HtmlTableRow();
                tblCell = new HtmlTableCell();
                lblInfo = new Label();

                lblInfo.Text = "<b>Organization:</b> " + GetOrgName(codr.Field<int>("OrgID"));
                lblInfo.CssClass = "ReportHeader";
                tblCell.Controls.Add(lblInfo);
                tblRow.Cells.Add(tblCell);
                myTable.Rows.Insert(rowCntr, tblRow);

                rowCntr += 1;

                tblRow = new HtmlTableRow();
                tblCell = new HtmlTableCell();
                lblInfo = new Label();

                string strDept = dsReport.Tables["Department"].Select(string.Format("DepartmentID = {0}", codr["DepartmentID"]))[0]["Department"].ToString();
                string strRole = dsReport.Tables["Role"].Select(string.Format("RoleID = {0}", codr["RoleID"]))[0]["Role"].ToString();

                lblInfo.Text = "<b>Department:</b> " + strDept + ", <b>Role:</b> " + strRole;
                lblInfo.CssClass = "LabelText";
                tblCell.Controls.Add(lblInfo);
                tblRow.Cells.Add(tblCell);
                myTable.Rows.Insert(rowCntr, tblRow);

                rowCntr += 1;

                tblRow = new HtmlTableRow();
                tblCell = new HtmlTableCell();
                lblInfo = new Label();

                string phone = codr.Field<string>("Phone");
                if (string.IsNullOrEmpty(phone))
                    lblInfo.Text = "<b>Phone:</b> none on record" + ", <b>Email:</b> " + codr["Email"].ToString();
                else
                    lblInfo.Text = "<b>Phone:</b> " + phone + ", <b>Email:</b> " + codr["Email"].ToString();

                lblInfo.CssClass = "LabelText";
                tblCell.Controls.Add(lblInfo);
                tblRow.Cells.Add(tblCell);
                myTable.Rows.Insert(rowCntr, tblRow);

                rowCntr += 1;

                string strMgrs = "<b>Managers:</b> ";
                if (Convert.ToBoolean(codr["IsManager"]))
                    strMgrs = "Client is a manager for this Organization";
                else
                {
                    DataRow[] cmdrs = dsReport.Tables["ClientManager"].Select(string.Format("ClientOrgID = {0}", codr["ClientOrgID"]));
                    if (cmdrs.Length == 0)
                        strMgrs = "No managers assigned";
                    else
                        strMgrs = string.Join("; ", cmdrs.Select(dr => dr["DisplayName"].ToString()));
                }

                tblRow = new HtmlTableRow();
                tblCell = new HtmlTableCell();
                lblInfo = new Label();

                lblInfo.Text = strMgrs;
                lblInfo.CssClass = "LabelText";
                tblCell.Controls.Add(lblInfo);
                tblRow.Cells.Add(tblCell);
                myTable.Rows.Insert(rowCntr, tblRow);

                rowCntr += 1;

                //2008-02-07 Add billing type info
                tblRow = new HtmlTableRow();
                tblCell = new HtmlTableCell();
                lblInfo = new Label();

                lblInfo.Text = "<b>Billing Type:</b> " + AppCode.BLL.BillingTypeManager.GetBillingTypeName(Convert.ToInt32(codr["ClientOrgID"]));
                lblInfo.CssClass = "LabelText";
                tblCell.Controls.Add(lblInfo);
                tblRow.Cells.Add(tblCell);
                myTable.Rows.Insert(rowCntr, tblRow);

                rowCntr += 1;

                string strAccts = "<b>Accounts:</b> ";
                DataRow[] cadrs = dsReport.Tables["ClientAccount"].Select(string.Format("ClientOrgID = {0}", codr["ClientOrgID"]));
                if (cadrs.Length == 0)
                    strAccts = "No accounts assigned<br>";
                else
                {
                    for (int j = 0; j < cadrs.Length; j++)
                    {
                        DataRow dr = dsReport.Tables["Account"].Rows.Find(cadrs[j]["AccountID"]);
                        if (dr == null)
                        {
                            int accountId = Convert.ToInt32(cadrs[j]["AccountID"]);
                            Account acct = DA.Current.Single<Account>(accountId);
                            strAccts += string.Format(@"{0} <span style=""color: #FF0000;"">[inactive]</span><br />", acct.Name);
                        }
                        else
                            strAccts += string.Format("{0}<br />", dr["Name"]);
                    }
                }

                tblRow = new HtmlTableRow();
                tblCell = new HtmlTableCell();
                lblInfo = new Label();

                lblInfo.Text = strAccts;
                lblInfo.CssClass = "LabelText";
                tblCell.Controls.Add(lblInfo);
                tblRow.Cells.Add(tblCell);
                myTable.Rows.Insert(rowCntr, tblRow);

                rowCntr += 1;
            }
        }

        private string GetClientName(DataRow dr)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(dr["MName"].ToString()))
                result = string.Format("{0} {1} ({2} {3})", dr["FName"], dr["LName"], dr["UserName"], dr["ClientID"]);
            else if (dr["MName"].ToString().Length == 1)
                result = string.Format("{0} {1}. {2} ({3} {4})", dr["FName"], dr["MName"], dr["LName"], dr["UserName"], dr["ClientID"]);
            else
                result = string.Format("{0} {1} {2} ({3} {4})", dr["FName"], dr["MName"], dr["LName"], dr["UserName"], dr["ClientID"]);

            return result;
        }

        private string GetSitePrivileges(DataRow dr)
        {
            string result = "<b>Privileges: </b>";

            ClientPrivilege val = (ClientPrivilege)dr.Field<int>("Privs");

            string list = val.ToString(); //creates a comma separated list of enum names

            if (list == null)
                result += "none";
            else
                result += list;

            return result;
        }

        private string GetDemographicInfo(DataRow dr)
        {
            string result = string.Empty;
            string citizen = "<b>US Citizenship: </b> " + dsReport.Tables["DemCitizen"].Rows.Find(dr["DemCitizenID"])["DemCitizen"].ToString() + "<br />";
            string gender = "<b>Gender: </b> " + dsReport.Tables["DemGender"].Rows.Find(dr["DemGenderID"])["DemGender"].ToString() + "<br />";
            string race = "<b>Race: </b> " + dsReport.Tables["DemRace"].Rows.Find(dr["DemRaceID"])["DemRace"].ToString() + "<br />";
            string ethnic = "<b>Ethnicity: </b> " + dsReport.Tables["DemEthnic"].Rows.Find(dr["DemEthnicID"])["DemEthnic"].ToString() + "<br />";
            string disability = "<b>Disability: </b> " + dsReport.Tables["DemDisability"].Rows.Find(dr["DemDisabilityID"])["DemDisability"].ToString() + "<br />";
            string techInterest = "<b>Technical Interest: </b> " + ClientDA.GetTechnicalInterestByClientID(Convert.ToInt32(dr["ClientID"])) + "<br />";
            result = citizen + gender + race + ethnic + disability + techInterest;
            return result;
        }

        private string GetUserType(DataRow dr)
        {
            string result = "<b>Communities: </b>";
            string[] list = MakeCommStrArray(Convert.ToInt32(dr["Communities"]));
            if (list == null)
                result += "none";
            else
                result += string.Join(", ", list);
            return result;
        }

        private string[] MakeCommStrArray(int c)
        {
            string[] result = dsReport.Tables["Communities"].AsEnumerable().Where(dr => (c & dr.Field<int>("CommunityFlag")) > 0).Select(dr => dr.Field<string>("Community")).ToArray();
            return result;
        }
    }
}