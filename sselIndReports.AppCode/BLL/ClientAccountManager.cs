using LNF.Cache;
using sselIndReports.AppCode.DAL;
using System;
using System.Data;
using System.Web;

namespace sselIndReports.AppCode.BLL
{
    public static class ClientAccountManager
    {
        public static DataTable GetClientAccountTable(int ManagerOrgID, string AccountDisplay)
        {
		    if (ManagerOrgID > 0)
            {
			    DataTable dtDisplay; 
			    if (Convert.ToInt32(HttpContext.Current.Session["ClientAccount_ManagerOrgID"]) == ManagerOrgID)
                {
				    //Change the Account Display Type
				    dtDisplay = (DataTable)HttpContext.Current.Session["ClientAccount_dtDisplay"];
			    }
                else
                {
				    //First time requesting data using by this ManagerOrgID
				    dtDisplay = ConstructRealTable(ManagerOrgID);
			    }

			    DataTable dtAccounts = (DataTable)HttpContext.Current.Session["ClientAccount_dtAccounts"];

			    if (AccountDisplay == "Name")
                {
				    foreach (DataRow dr in dtAccounts.Rows)
					    dtDisplay.Columns[dtAccounts.Rows.IndexOf(dr) + 1].ColumnName = dr["Name"].ToString();
			    }
                else if (AccountDisplay == "Number")
                {
				    foreach (DataRow dr in dtAccounts.Rows)
					    dtDisplay.Columns[dtAccounts.Rows.IndexOf(dr) + 1].ColumnName = dr["Number"].ToString();
			    }
                else if (AccountDisplay == "Project")
                {
				    int i = 2;
				    foreach (DataRow dr in dtAccounts.Rows)
                    {
					    try
                        {
						    dtDisplay.Columns[dtAccounts.Rows.IndexOf(dr) + 1].ColumnName = dr["Project"].ToString();
					    }
                        catch
                        {
						    dtDisplay.Columns[dtAccounts.Rows.IndexOf(dr) + 1].ColumnName = dr["Project"].ToString() + " - " + i.ToString();
						    i += 1;
					    }
				    }
			    }
                else if (AccountDisplay == "ShortCode")
                {
				    int i = 2;
				    foreach (DataRow dr in dtAccounts.Rows)
                    {
					    try
                        {
						    dtDisplay.Columns[dtAccounts.Rows.IndexOf(dr) + 1].ColumnName = dr["ShortCode"].ToString();
					    }
                        catch
                        {
						    dtDisplay.Columns[dtAccounts.Rows.IndexOf(dr) + 1].ColumnName = dr["ShortCode"].ToString() + " - " + i.ToString();
						    i += 1;
					    }
				    }
			    }

			    return dtDisplay;
		    }
            else
			    return null;
	    }

	    private static DataTable ConstructRealTable(int ManagerOrgID)
        {
		    DataSet ds = AccountDA.GetClientAccountDataSet(ManagerOrgID);

		    DataTable dtAccounts = ds.Tables[0];
		    DataTable dtClientOrgs = ds.Tables[1];
		    DataTable dtAssociation = ds.Tables[2];

		    // Construct a table to display
		    DataTable dtDisplay = new DataTable("DisplayTable");

		    dtDisplay.Columns.Add("DisplayName", typeof(string));

		    //Add all Accounts as columns
		    foreach (DataRow dr in dtAccounts.Rows)
			    dtDisplay.Columns.Add("chk" + dr["AccountID"].ToString(), typeof(string));

		    //Populate the data
		    string strColumn = string.Empty;
		    foreach (DataRow dr in dtClientOrgs.Rows)
            {
			    DataRow ndr = dtDisplay.NewRow();
			    ndr["DisplayName"] = dr["DisplayName"];
                DataRow[] adr = dtAssociation.Select(string.Format("ClientOrgID = {0}", dr["ClientOrgID"]));
			    foreach (DataRow assocdr in adr)
                {
				    strColumn = string.Format("chk{0}", assocdr["AccountID"]);
				    ndr[strColumn] = true;
			    }
			    dtDisplay.Rows.Add(ndr);
		    }

		    //Save the necessary data into session for future reuse if user changes Account Display
		    HttpContext.Current.Session["ClientAccount_dtAccounts"] = dtAccounts;
		    HttpContext.Current.Session["ClientAccount_dtDisplay"] = dtDisplay;
		    //We need to use the session variable to determine if new manager is selected.  This saves resource if user only changes Account Display type
		    HttpContext.Current.Session["ClientAccount_ManagerOrgID"] = ManagerOrgID;
		    return dtDisplay;
	    }

	    public static DataTable GetActiveManagers(int currentUserClientId)
        {
		    if (HttpContext.Current.User.IsInRole("Administrator"))
			    return AccountDA.GetActiveManagers(-1);
		    else
            {
			    //Executieves only see him/herself
                return AccountDA.GetActiveManagers(currentUserClientId);
		    }
	    }

        public static DataTable GetManagersByPeriod(int sYear, int sMonth, int NumMonths, int ChargeTypeID)
        {
            DateTime sp = new DateTime(sYear, sMonth, 1);
            DateTime ep = sp.AddMonths(NumMonths);
            return GetManagersByPeriod(sp, ep, ChargeTypeID);
        }

        public static DataTable GetManagersByPeriod(DateTime StartPeriod, int ChargeTypeID)
        {
            DateTime EndPeriod = DateTime.Now.Date;
            return GetManagersByPeriod(StartPeriod, EndPeriod, ChargeTypeID);
        }

        public static DataTable GetManagersByPeriod(DateTime StartPeriod, DateTime EndPeriod, int ChargeTypeID)
        {
            if (StartPeriod > EndPeriod) return null;
            return AccountDA.GetManagersByPeriod(StartPeriod, EndPeriod, ChargeTypeID);
        }

	    public static DataTable GetAllAccountsbyManager(int ManagerOrgID)
        {
            return ClientDA.GetAllAccountsByClientOrgID(ManagerOrgID);
	    }
    }
}
