using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Web;
using sselIndReports.AppCode;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class AggSumUsage : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CacheManager.Current.Updated(false);
                cblPriv.Items.LoadPrivs();
            }
        }

        protected void pp1_SelectedPeriodChanged(object sender, EventArgs e)
        {
            ShowLabAccess();
        }

        protected void rbUserOrg_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowLabAccess();
        }

        protected void cblPriv_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowLabAccess();
        }

        private void ShowLabAccess()
        {
            dgAccess.DataSource = null;
            dgAccess.DataBind();

            ClientPrivilege selectedPriv = cblPriv.Items.CalculatePriv();
            if (selectedPriv == 0)
            {
                return;
            }

            if (pp1.SelectedPeriod > DateTime.Now)
            {
                return;
            }

            if (pp1.SelectedPeriod <= DateTime.Now && pp1.SelectedPeriod.AddMonths(1) > DateTime.Now && !CacheManager.Current.Updated())
            {
                try //to handle missing data error
                {
                    WriteData writeData = new WriteData();
                    writeData.UpdateTables(new string[] { "Tool", "Room" }, 0, 0, DateTime.Now, UpdateDataType.DataClean | UpdateDataType.Data, true, false);
                }
                catch { }

                CacheManager.Current.Updated(true);
            }

            //get all access data for period
            DataSet ds;
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                int orgId = Convert.ToInt32(Session["OrgID"]);
                int userOrgId = Convert.ToInt32(rblUserOrg.SelectedValue);
                dba
                    .AddParameter("@Period", pp1.SelectedPeriod)
                    .AddParameter("@Privs", (int)selectedPriv)
                    .AddParameterIf("@SelOrg", userOrgId >= 0, userOrgId)
                    .AddParameterIf("@OrgID", CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Executive), orgId);
                ds = dba.FillDataSet("AggUsage_Select");
            }

            DataTable dtRaw = ds.Tables[0];
            DataTable dtRoom = ds.Tables[1];

            DataTable dtUsage = new DataTable();
            dtUsage.Columns.Add("ClientID", typeof(int));
            dtUsage.Columns.Add("DisplayName", typeof(string));
            dtUsage.Columns.Add("Manager", typeof(string));
            dtUsage.Columns.Add("URLdata", typeof(string));

            foreach (DataRow dr in dtRoom.Rows)
            {
                dtUsage.Columns.Add(EntriesColumn(dr), typeof(double));
                if (Convert.ToBoolean(dr["PassbackRoom"]))
                    dtUsage.Columns.Add(HoursColumn(dr), typeof(double));
            }

            int dateInt; //used to prevent casual snooping into the indDet page
            int temp = 0; //store a temporary value to test if it's zero
            DataRow nr;
            DataRow[] fdr;
            foreach (DataRow dr in dtRaw.Rows)
            {
                fdr = dtUsage.Select(string.Format("ClientID = {0}", dr["ClientID"]));

                if (fdr.Length == 0) //add row to table
                {
                    nr = dtUsage.NewRow();
                    nr["ClientID"] = dr["ClientID"];
                    nr["DisplayName"] = dr["DisplayName"];
                    nr["Manager"] = dr["Manager"];
                    dateInt = 200 * (pp1.SelectedYear + Convert.ToInt32(dr["ClientID"])) + 3 * pp1.SelectedMonth + (Convert.ToInt32(dr["ClientID"]) % 10);
                    nr["URLdata"] = string.Format("URLdata={0:0000}{1}", dr["ClientID"], dateInt);
                    dtUsage.Rows.Add(nr);
                }
                else
                    nr = fdr[0];

                bool isPassbackRoom = false;
                DataRow[] rows = dtRoom.Select(string.Format("RoomID = {0}", dr["RoomID"]));
                isPassbackRoom = Convert.ToBoolean(rows[0]["PassbackRoom"]);

                //2008-10-22 Sanrine wants to see non antipassback room, now we have DC lab and Organics Bay
                if (Convert.ToDouble(dr["Hours"]) > 0 && isPassbackRoom)
                {
                    nr[EntriesColumn(dr)] = dr["Entries"];
                    nr[HoursColumn(dr)] = dr["Hours"];
                }
                else if (!isPassbackRoom)
                {
                    //for non Passbackroom, the Hours entry is null, Entries values are still valid
                    temp = Convert.ToInt32(dr["Entries"]);
                    if (temp != 0)
                        nr[EntriesColumn(dr)] = temp;
                }
            }

            //this is the footer row
            nr = dtUsage.NewRow();
            nr["ClientID"] = 0;
            nr["DisplayName"] = "zzzzzzzz";
            nr["Manager"] = string.Empty;
            nr["URLdata"] = string.Empty;
            foreach (DataRow dr in dtRoom.Rows)
            {
                nr[EntriesColumn(dr)] = dtUsage.Compute(string.Format("SUM({0})", EntriesColumn(dr)), string.Empty);
                if (Convert.ToBoolean(dr["PassbackRoom"]))
                    nr[HoursColumn(dr)] = dtUsage.Compute(string.Format("SUM({0})", HoursColumn(dr)), string.Empty);
            }
            dtUsage.Rows.Add(nr);

            BoundField displayNameCol = new BoundField();
            displayNameCol.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            displayNameCol.HeaderStyle.Font.Bold = true;
            displayNameCol.HeaderText = "Lab User";
            displayNameCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            dgAccess.Columns.Add(displayNameCol);

            BoundField managerCol = new BoundField();
            managerCol.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            managerCol.HeaderStyle.Font.Bold = true;
            managerCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            managerCol.HtmlEncode = false;
            managerCol.HeaderText = "Manager";
            managerCol.DataField = "Manager";
            dgAccess.Columns.Add(managerCol);

            foreach (DataRow dr in dtRoom.Rows)
            {
                BoundField entryCol = new BoundField();
                entryCol.HeaderStyle.Width = Unit.Pixel(120);
                entryCol.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                entryCol.HeaderStyle.Font.Bold = true;
                entryCol.ItemStyle.Width = Unit.Pixel(120);
                entryCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                entryCol.DataFormatString = "{0:0.#}";
                entryCol.HeaderText = dr["Room"].ToString() + " Entries";
                entryCol.DataField = EntriesColumn(dr);
                dgAccess.Columns.Add(entryCol);

                if (Convert.ToBoolean(dr["PassbackRoom"]))
                {
                    BoundField hoursCol = new BoundField();
                    hoursCol.HeaderStyle.Width = Unit.Pixel(120);
                    hoursCol.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    hoursCol.HeaderStyle.Font.Bold = true;
                    hoursCol.ItemStyle.Width = Unit.Pixel(120);
                    hoursCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    hoursCol.DataFormatString = "{0:0.##}";
                    hoursCol.HeaderText = dr["Room"].ToString() + " Hours";
                    hoursCol.DataField = HoursColumn(dr);
                    dgAccess.Columns.Add(hoursCol);
                }
            }

            dtUsage.DefaultView.Sort = "DisplayName";
            dgAccess.DataSource = dtUsage;
            dgAccess.DataBind();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            CacheManager.Current.RemoveSessionValue("Updated");
            Response.Redirect("~/");
        }

        protected void dgAccess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int cStart = 0;
                DataRowView drv = (DataRowView)e.Row.DataItem;
                if (Convert.ToInt32(drv["ClientID"]) == 0)
                {
                    e.Row.Cells[0].Text = "Totals";
                    e.Row.Cells[0].ColumnSpan = 2;
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells.RemoveAt(1);
                    e.Row.Font.Bold = true;
                    e.Row.BackColor = System.Drawing.Color.LightPink;
                }
                else
                {
                    cStart = 1;
                    HyperLink userUrl = new HyperLink();
                    userUrl.Text = drv["DisplayName"].ToString();
                    userUrl.NavigateUrl = "~/IndSumUsage.aspx?" + drv["URLdata"].ToString();
                    e.Row.Cells[0].Controls.Add(userUrl);
                }

                for (int i = cStart; i < e.Row.Cells.Count; i += 2)
                {
                    //e.Item.Cells[i].CssClass = "CategoryBorder";
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i > 0 && (i % 2) == 1)
                    {
                        //e.Item.Cells[i].CssClass = "HeaderWithRight"
                    }
                    else
                    {
                        //e.Item.Cells[i].CssClass = "HeaderWithoutRight"
                    }
                }
            }
        }

        private string EntriesColumn(DataRow dr)
        {
            return string.Format("Entries{0}", dr["RoomID"]);
        }

        private string HoursColumn(DataRow dr)
        {
            return string.Format("Hours{0}", dr["RoomID"]);
        }
    }
}