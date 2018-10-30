using LNF.Repository;
using System.Data;

namespace sselIndReports.AppCode.DAL
{
    public static class OrgDA
    {
        public static DataTable GetAllOrgs()
        {
            return DA.Command().Param("Action", "All").FillDataTable("dbo.Org_Select");
        }
    }
}
