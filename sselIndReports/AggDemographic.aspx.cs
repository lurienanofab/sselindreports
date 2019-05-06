using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Web;
using sselIndReports.AppCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class AggDemographic : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ContextBase.Updated(false);
                ShowReport();
            }
        }

        protected void pp1_SelectedPeriodChanged(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void ShowReport()
        {
            DateTime sDate = pp1.SelectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            Dictionary<string, DataGrid> grids = new Dictionary<string, DataGrid>();
            grids.Add("DemGender", dgGender);
            grids.Add("DemRace", dgRace);
            grids.Add("DemEthnic", dgEthnic);
            grids.Add("DemDisability", dgDisability);
            grids.Add("DemCitizen", dgCitizen);

            foreach (var kvp in grids)
            {
                DataGrid dg = kvp.Value;
                dg.DataSource = null;
                dg.DataBind();
            }

            if (sDate > DateTime.Now.Date) return;

            //draw from RoomData - need to upadte tool first due to apportionment
            if (sDate <= DateTime.Now && eDate > DateTime.Now && !ContextBase.Updated())
            {
                try
                {
                    // to handle missing data error
                    WriteData writeData = new WriteData();
                    writeData.UpdateTables(BillingCategory.Tool | BillingCategory.Room, UpdateDataType.DataClean | UpdateDataType.Data, DateTime.Now.FirstOfMonth(), 0);
                }
                catch { }

                ContextBase.Updated(true);
            }

            //client info
            var dtClient = DA.Command()
                .Param("Action", "All")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .Param("Privs", (int)ClientPrivilege.LabUser)
                .FillDataTable("dbo.Client_Select");

            //room info
            var dtRoom = DA.Command()
                .Param("Action", "PassbackRooms")
                .FillDataTable("dbo.Room_Select");

            foreach (var kvp in grids)
            {
                string type = kvp.Key;

                var ds = DA.Command()
                    .Param("Action", "DemReport")
                    .Param("demType", type)
                    .Param("sDate", sDate)
                    .Param("eDate", eDate)
                    .FillDataSet("dbo.Dem_Select");

                var dtDemInfo = ds.Tables[0];
                var dtDemCat = ds.Tables[1];

                var dtDemRep = new DataTable();
                dtDemRep.Columns.Add("RoomID", typeof(int));
                dtDemRep.Columns.Add("Room", typeof(string));
                dtDemRep.Columns.Add("Total", typeof(double));

                foreach (DataRow drDemCat in dtDemCat.Rows)
                    dtDemRep.Columns.Add(drDemCat[type].ToString(), typeof(double));

                foreach (DataRow drRoom in dtRoom.Rows)
                {
                    DataRow nr = dtDemRep.NewRow();
                    nr["RoomID"] = drRoom["RoomID"];
                    nr["Room"] = drRoom["Room"];
                    dtDemRep.Rows.Add(nr);
                }

                foreach (DataRow drDemRep in dtDemRep.Rows)
                {
                    drDemRep["Total"] = 0.0;
                    foreach (DataRow drDemCat in dtDemCat.Rows)
                    {
                        DataRow[] fdr = dtDemInfo.Select(string.Format("RoomID = {0} AND {1} = '{2}'", drDemRep["RoomID"], type, drDemCat[type]));
                        if (fdr.Length > 0)
                        {
                            drDemRep[drDemCat[type].ToString()] = fdr[0]["Hours"];
                            drDemRep["Total"] = drDemRep.Field<double>("Total") + fdr[0].Field<double>("Hours");
                        }
                        else
                            drDemRep[drDemCat[type].ToString()] = 0.0;
                    }
                }

                dtDemRep.Columns.Remove(dtDemRep.Columns["RoomID"]);

                DataGrid dg = kvp.Value;
                dg.DataSource = dtDemRep;
                dg.DataBind();

                ds = null;
                dtDemRep = null;
            }
        }

        protected void dgItemBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ((TableCell)e.Item.Controls[0]).Width = Unit.Pixel(130);
                DataRowView drv = (DataRowView)e.Item.DataItem;
                for (int i = 1; i < drv.Row.Table.Columns.Count; i++)
                {
                    TableCell tc = (TableCell)e.Item.Controls[i];
                    tc.Width = Unit.Pixel(80);
                    tc.Text = string.Format("{0:#,##0.0}", drv[i]);
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            ContextBase.Session.Remove("Updated");
            ContextBase.RemoveCacheData(); //remove anything left in cache
            Response.Redirect("~");
        }
    }
}