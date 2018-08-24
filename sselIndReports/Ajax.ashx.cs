using Newtonsoft.Json;
using sselIndReports.AppCode.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        private string GetRequestBody(HttpContext context)
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }

        public void ProcessRequest(HttpContext context)
        {
            string json;

            string command = context.Request.QueryString["command"];

            if (!string.IsNullOrEmpty(command))
            {
                json = HandleCommand(command, context);
            }
            else
            {
                DatatablesRequest req = JsonConvert.DeserializeObject<DatatablesRequest>(GetRequestBody(context));

                if (!string.IsNullOrEmpty(req.Command))
                {
                    if (req.Command == "GetOrganizationReport")
                        json = Json(context, GetOrganizationReport(req));
                    else
                        json = Error(context, $"Unknown command: {req.Command}");
                }
                else
                    json = Error(context, "Missing parameter: command");
            }

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

                return Json(context, GetOrganizationReportItems(year, month, orgId));
            }
            else
            {
                return Error(context, "Unknown command");
            }
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

        private IEnumerable<OrganizationReportItem> GetOrganizationReportItems(int year, int month, int orgId)
        {
            var dt = AccountDA.GetAccountDetailsByOrgID(year, month, orgId);

            var result = new List<OrganizationReportItem>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new OrganizationReportItem()
                {
                    AccountName = dr.Field<string>("Name"),
                    ShortCode = dr.Field<string>("ShortCode").Trim(),
                    ProjectNumber = dr.Field<string>("Project Number"),
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

        private DatatablesResponse<OrganizationReportItem> GetOrganizationReport(DatatablesRequest req)
        {
            var data = GetOrganizationReportItems(req.Year, req.Month, req.OrgID);

            var result = new DatatablesResponse<OrganizationReportItem>()
            {
                Data = data,
                Draw = req.Draw,
                Error = string.Empty,
                RecordsFiltered = data.Count(),
                RecordsTotal = data.Count()
            };

            return result;
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
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("orgId")]
        public int OrgID { get; set; }

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