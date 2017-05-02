using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;

namespace sselIndReports.AppCode.DAL
{
    public static class OrgDA
    {
        public static DataTable GetAllOrgs()
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                return dba.ApplyParameters(new { Action = "All" }).FillDataTable("Org_Select");
        }
    }
}
