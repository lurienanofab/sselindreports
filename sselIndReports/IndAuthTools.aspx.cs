﻿using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using sselIndReports.AppCode;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace sselIndReports
{
    public partial class IndAuthTools : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.Executive | ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (CurrentUser.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Staff | ClientPrivilege.Executive))
                {
                    chkActive.Visible = true;
                    LoadUserList();
                }
                else
                    ddlUser.Items.Insert(0, new ListItem(CacheManager.Current.CurrentUser.DisplayName, CacheManager.Current.CurrentUser.ClientID.ToString()));

                ddlUser.SelectedValue = CacheManager.Current.CurrentUser.ClientID.ToString();

                RetrieveData();
            }
        }

        private void LoadUserList()
        {
            //client info - gets put into ddl, not needed in dataset
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@ClientID", CacheManager.Current.CurrentUser.ClientID);

                if (CurrentUser.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Staff))
                {
                    dba.AddParameter("@Privs", (int)ClientPrivilege.PhysicalAccess);
                    if (chkActive.Checked)
                    {
                        dba.AddParameter("@SDate", new DateTime(1999, 1, 1));
                        dba.AddParameter("@EDate", DateTime.Now.AddDays(1));
                    }
                    dba.AddParameter("@Action", "All");
                }
                else if (CurrentUser.HasPriv(ClientPrivilege.Executive))
                {
                    //for executive-only person, we only show the people he/she manages
                    dba.AddParameter("@Action", "ByMgr");
                }

                using (var reader = dba.ExecuteReader("Client_Select"))
                {
                    ddlUser.DataSource = reader;
                    ddlUser.DataTextField = "DisplayName";
                    ddlUser.DataValueField = "ClientID";
                    ddlUser.DataBind();
                }
            }
        }

        protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            RetrieveData();
        }

        private void RetrieveData()
        {
            lblUserMessage.Text = string.Empty;
            if (ddlUser.SelectedValue == "0")
            {
                lblUserMessage.Text = "&larr; Required";
                return;
            }

            DataTable dtAuthTools;

            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.AddParameter("@Action", "AuthTools");
                dba.AddParameter("@ClientID", ddlUser.SelectedValue);
                dtAuthTools = dba.FillDataTable("sselScheduler_Select");
            }

            int i = 0;
            DataRow dr;
            string lastLab = string.Empty;
            string lastProcTech = string.Empty;
            while (i < dtAuthTools.Rows.Count)
            {
                if (dtAuthTools.Rows[i]["LabName"].ToString() != lastLab)
                {
                    lastLab = dtAuthTools.Rows[i]["LabName"].ToString();
                    dr = dtAuthTools.NewRow();
                    dr["LabName"] = lastLab;
                    dr["ProcessTechName"] = DBNull.Value;
                    dr["ResourceName"] = DBNull.Value;
                    dtAuthTools.Rows.InsertAt(dr, i);
                    i += 1;
                }

                if (dtAuthTools.Rows[i]["ProcessTechName"].ToString() != lastProcTech)
                {
                    lastProcTech = dtAuthTools.Rows[i]["ProcessTechName"].ToString();
                    dr = dtAuthTools.NewRow();
                    dr["LabName"] = lastLab;
                    dr["ProcessTechName"] = lastProcTech;
                    dr["ResourceName"] = DBNull.Value;
                    dtAuthTools.Rows.InsertAt(dr, i);
                    i += 1;
                }

                i += 1;
            }

            if (dtAuthTools.Rows.Count > 0)
            {
                dgAuthTools.Visible = true;
                litNoData.Text = string.Empty;
                dgAuthTools.DataSource = dtAuthTools;
                dgAuthTools.DataBind();
            }
            else
            {
                dgAuthTools.Visible = false;
                litNoData.Text = @"<div style=""padding: 5px; color: #808080; border: solid 1px #AAAAAA; font-style: italic;"">This client is not authorized on any tools.</div>";
            }
        }

        protected void dgAuthTools_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                Literal litAuthTool = (Literal)e.Item.FindControl("litAuthTool");
                TableRow tr = (TableRow)litAuthTool.Parent.Parent;
                if (drv["ProcessTechName"] == DBNull.Value)  //new lab row
                {
                    tr.CssClass = "OutlineLevel1";
                    litAuthTool.Text = string.Format("<div class=\"lab-name\">{0}</div>", drv["LabName"]);
                }
                else if (drv["ResourceName"] == DBNull.Value) //new processtech row
                {
                    tr.CssClass = "OutlineLevel2";
                    litAuthTool.Text = string.Format("<div class=\"proc-tech-name\">{0}</div>", drv["ProcessTechName"]);
                }
                else
                {
                    if (e.Item.ItemType == ListItemType.Item)
                        tr.CssClass = "Item";
                    else
                        tr.CssClass = "AlternatingItem";

                    litAuthTool.Text = string.Format("<div class=\"resource-name\">{0}</div>", drv["ResourceName"]);
                }
            }
        }

        protected void chkActive_CheckedChanged(object sender, EventArgs e) //Handles chkActive.CheckedChanged
        {
            LoadUserList();
            ddlUser.Items.Insert(0, new ListItem("-- Select --", "0"));
            ddlUser.ClearSelection();
        }
    }
}