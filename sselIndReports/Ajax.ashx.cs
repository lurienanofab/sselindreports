using LNF.Repository;
using LNF.Web;
using Newtonsoft.Json;
using sselIndReports.AppCode.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace sselIndReports
{
    /// <summary>
    /// Summary description for Ajax
    /// </summary>
    public class Ajax : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string json;
            string command = context.Request.QueryString["command"];

            if (!string.IsNullOrEmpty(command))
                json = HandleCommand(command, context);
            else
                json = Error(context, "Missing parameter: command");

            context.Response.Write(json);
        }

        private string HandleCommand(string command, HttpContext context)
        {
            if (command == "GetOrganizationReport")
            {
                if (!int.TryParse(context.Request.QueryString["orgId"], out int orgId))
                    return Error(context, "Invalid parameter value: orgId");

                if (!int.TryParse(context.Request.QueryString["year"], out int year))
                    return Error(context, "Invalid parameter value: year");

                if (!int.TryParse(context.Request.QueryString["month"], out int month))
                    return Error(context, "Invalid parameter value: month");

                if (IsDatatableRequest(context))
                {
                    var req = context.Request.GetDocumentContents<DatatablesRequest>();
                    return Json(context, GetOrganizationReport(req, year, month, orgId));
                }
                else
                    return Json(context, GetOrganizationReport(year, month, orgId));
            }
            else
                return Error(context, "Unknown command");
        }

        private bool IsDatatableRequest(HttpContext context)
        {
            return context.Request.QueryString["dt"] == "true";
        }

        private IEnumerable<OrganizationReportItem> GetOrganizationReportItems(DataTable dt)
        {
            var result = new List<OrganizationReportItem>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new OrganizationReportItem()
                {
                    AccountName = dr.Field<string>("AccountName"),
                    ShortCode = dr.Field<string>("ShortCode").Trim(),
                    ProjectNumber = dr.Field<string>("ProjectNumber"),
                    DisplayName = dr.Field<string>("DisplayName"),
                    Phone = dr.Field<string>("Phone"),
                    Email = dr.Field<string>("Email"),
                    IsManager = dr.Field<bool>("IsManager"),
                    IsFinManager = dr.Field<bool>("IsFinManager"),
                    FundingSourceName = dr.Field<string>("FundingSourceName"),
                    TechnicalFieldName = dr.Field<string>("TechnicalFieldName")
                });
            }

            return result;
        }

        private IEnumerable<OrganizationReportItem> GetOrganizationReport(int year, int month, int orgId)
        {
            var dt = AccountDA.GetAccountDetailsByOrgID(year, month, orgId);
            return GetOrganizationReportItems(dt);
        }

        private DatatablesResponse<OrganizationReportItem> GetOrganizationReport(DatatablesRequest req, int year, int month, int orgId)
        {
            var dt = GetAccountDetailsByOrgID(req, year, month, orgId, out int recordsTotal, out int recordsFiltered);

            var data = GetOrganizationReportItems(dt);

            var result = new DatatablesResponse<OrganizationReportItem>()
            {
                Data = data,
                Draw = req.Draw,
                Error = string.Empty,
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            return result;
        }

        public static DataTable GetAccountDetailsByOrgID(DatatablesRequest req, int year, int month, int orgId, out int recordsTotal, out int recordsFiltered)
        {
            DateTime sDate = new DateTime(year, month, 1);
            DateTime eDate = sDate.AddMonths(1);

            var command = DA.Command(CommandType.Text);

            string columns = @"
                    a.[Name] AS AccountName
			        , RTRIM(LTRIM(a.ShortCode)) AS ShortCode
			        , RIGHT(a.Number, 7) AS ProjectNumber
			        , c.LName + ', ' + c.FName AS DisplayName
			        , co.Phone
			        , co.Email
			        , co.IsManager
			        , co.IsFinManager
			        , fs.FundingSource AS FundingSourceName
			        , tf.TechnicalField AS TechnicalFieldName";

            string select = @"
                    DECLARE @alog_account table (Record int NOT NULL PRIMARY KEY)
					DECLARE @alog_clientorg table (Record int NOT NULL PRIMARY KEY)
					DECLARE @alog_client table (Record int NOT NULL PRIMARY KEY)

		            INSERT @alog_account
		            SELECT DISTINCT Record
		            FROM dbo.ActiveLog
		            WHERE TableName = 'Account'
			            AND EnableDate < @eDate
			            AND ((DisableDate IS NULL) OR (DisableDate > @sDate))

					INSERT @alog_clientorg
		            SELECT DISTINCT Record
		            FROM dbo.ActiveLog
		            WHERE TableName = 'ClientOrg'
			            AND EnableDate < @eDate
			            AND ((DisableDate IS NULL) OR (DisableDate > @sDate))

					INSERT @alog_client
		            SELECT DISTINCT Record
		            FROM dbo.ActiveLog
		            WHERE TableName = 'Client'
			            AND EnableDate < @eDate
			            AND ((DisableDate IS NULL) OR (DisableDate > @sDate))

		            SELECT
			            {0}
		            FROM dbo.Account a
		            FULL JOIN dbo.ClientAccount ca ON ca.AccountID = a.AccountID
		            FULL JOIN dbo.ClientOrg co ON co.ClientOrgID = ca.ClientOrgID
		            FULL JOIN dbo.Client c ON c.ClientID = co.ClientID
		            LEFT JOIN dbo.FundingSource fs ON fs.FundingSourceID = a.FundingSourceID
		            LEFT JOIN dbo.TechnicalField tf ON tf.TechnicalFieldID = a.TechnicalFieldID";

            string where = @"
		            WHERE a.OrgID = @OrgID
		            AND EXISTS
		            (
			            SELECT Record
			            FROM @alog_account
			            WHERE a.AccountID = Record
		            )
		            AND EXISTS
		            (
			            SELECT Record
			            FROM @alog_clientorg
			            WHERE co.ClientOrgID = Record
		            )
		            AND EXISTS
		            (
			            SELECT Record
			            FROM @alog_client
			            WHERE c.ClientID = Record
		            )";

            string sql = string.Format(select, "COUNT(*)") + where;

            command
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .Param("OrgID", orgId);

            recordsTotal = command.ExecuteScalar<int>(sql);

            if (req.Search != null && !string.IsNullOrEmpty(req.Search.Value))
            {
                command.Param("Search", "%" + req.Search.Value + "%");

                where += $@"
                    AND (
                        a.[Name] LIKE @Search
                        OR RTRIM(LTRIM(a.ShortCode)) LIKE @Search
                        OR RIGHT(a.Number, 7) LIKE @Search
                        OR c.LName + ', ' + c.FName LIKE @Search
                        OR co.Phone LIKE @Search
                        OR co.Email LIKE @Search
                        OR CASE WHEN co.IsManager = 1 THEN 'true' ELSE 'false' END LIKE @Search
                        OR CASE WHEN co.IsFinManager = 1 THEN 'true' ELSE 'false' END LIKE @Search
                        OR fs.FundingSource LIKE @Search
                        OR tf.TechnicalField LIKE @Search
                    )";

                sql = string.Format(select, "COUNT(*)") + where;
                recordsFiltered = command.ExecuteScalar<int>(sql);
            }
            else
            {
                recordsFiltered = recordsTotal;
            }

            string orderBy = " ORDER BY ";
            string comma = string.Empty;

            if (req.Order != null)
            {
                foreach (var ord in req.Order)
                {
                    if (req.Columns.Count() > ord.Column)
                    {
                        var col = req.Columns.ElementAt(ord.Column);

                        if (col.Orderable)
                        {
                            orderBy += comma + col.Name + " " + ord.Direction;
                            comma = ", ";
                        }
                    }
                }
            }

            if (orderBy == " ORDER BY ")
                orderBy += "AccountName ASC, DisplayName ASC";

            sql = string.Format(select, columns) + where + orderBy;

            command.Param("Skip", req.Start);
            sql += " OFFSET @Skip ROWS";

            if (req.Length > 0)
            {
                command.Param("Take", req.Length);
                sql += " FETCH NEXT @Take ROWS ONLY";
            }

            return command.FillDataTable(sql);
        }

        private string Error(HttpContext context, string message, int statusCode = 400)
        {
            context.Response.StatusCode = 400;
            return JsonConvert.SerializeObject(new { error = message });
        }

        private string Json(HttpContext context, object obj)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            return JsonConvert.SerializeObject(obj);
        }

        public bool IsReusable => false;
    }

    public class OrganizationReportItem
    {
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public string ProjectNumber { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
        public bool IsFinManager { get; set; }
        public string FundingSourceName { get; set; }
        public string TechnicalFieldName { get; set; }
    }

    public class DatatablesResponse<T>
    {
        [JsonProperty("draw")]
        public int Draw { get; set; }

        [JsonProperty("recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonProperty("recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class DatatablesRequest
    {
        [JsonProperty("draw")]
        public int Draw { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("columns")]
        public IEnumerable<DatatablesColumn> Columns { get; set; }

        [JsonProperty("order")]
        public IEnumerable<DatatablesOrder> Order { get; set; }

        [JsonProperty("search")]
        public DatatablesSearch Search { get; set; }
    }

    public class DatatablesColumn
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("searchable")]
        public bool Searchable { get; set; }

        [JsonProperty("orderable")]
        public bool Orderable { get; set; }

        [JsonProperty("serach")]
        public DatatablesSearch Search { get; set; }
    }

    public class DatatablesOrder
    {
        [JsonProperty("column")]
        public int Column { get; set; }

        [JsonProperty("dir")]
        public string Direction { get; set; }
    }

    public class DatatablesSearch
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("regex")]
        public bool Regex { get; set; }
    }
}