using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Text;
using UMT.Backend.Services;

namespace UMT.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly MySqlConnectionFactory _factory;
        private readonly RawSessionsService _service;

        public ExportController(MySqlConnectionFactory factory, RawSessionsService service)
        {
            _factory = factory;
            _service = service;
        }

        private static readonly Dictionary<string, string> ApplicationTables =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "3d-trim", "mst_3dtrim_data" },
                { "multiple-3d-operations", "mst_multi3d_details" },
                { "naming-tool", "mst_namingtool_data" },
                { "point-chart", "mst_pointchart_data" },
                { "profile-checker", "mst_profilechecker_data" },
                { "smart-cvt", "mst_smartcvt_data" },
                { "section-manager", "mst_sectionmanager_data" }
            };

        // ✅ EXPORT MAIN TABLE
        [HttpGet("mst-usage-tool")]
        public async Task<IActionResult> ExportMstUsageTool()
        {
            try
            {
                var table = await _factory.QueryTableAsync(
                    $"SELECT * FROM {_factory.TableName("mst_tool_usage")}");

                if (table.Rows.Count == 0)
                    return NotFound(new { message = "No data found" });

                var csv = CsvWriter.Write(table.Columns, table.Rows);

                return File(Encoding.UTF8.GetBytes(csv), "text/csv", "mst_usage_tool.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ EXPORT APPLICATION DATA
        [HttpGet("application-data/{applicationKey}")]
        public async Task<IActionResult> ExportApplicationData(string applicationKey)
        {
            try
            {
                if (!ApplicationTables.TryGetValue(applicationKey ?? "", out var table))
                    return BadRequest(new { message = "Invalid application" });

                var query = Request.Query.ToDictionary(
                    q => q.Key,
                    q => q.Value.ToList(),
                    StringComparer.OrdinalIgnoreCase);

                var sql = new StringBuilder();
                sql.Append($"SELECT * FROM {_factory.TableName(table)} WHERE 1=1");

                var parameters = new List<MySqlParameter>();

                // ✅ CAD filter
                if (query.TryGetValue("cad", out var cad))
                {
                    sql.Append(" AND CadTool = @cad");
                    parameters.Add(new MySqlParameter("@cad", cad.First()));
                }

                // ✅ Date filter
                ApplyDateFilter(sql, parameters, query);

                sql.Append(" ORDER BY StartTime DESC");

                var result = await ExecuteQuery(sql.ToString(), parameters);

                if (result.Rows.Count == 0)
                    return NotFound(new { message = "No data found" });

                var csv = CsvWriter.Write(result.Columns, result.Rows);

                return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"{table}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ JSON DOWNLOAD
        [HttpGet("raw-sessions-json")]
        public async Task<IActionResult> DownloadJson()
        {
            try
            {
                await _service.GenerateJson();

                string basePath =
                    Environment.GetEnvironmentVariable("DASHBOARD_STATIC_DIR")
                    ?? AppDomain.CurrentDomain.BaseDirectory;

                var filePath = Path.Combine(basePath, "raw-sessions.json");

                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { message = "File not found" });

                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                return File(bytes, "application/json", "raw-sessions.json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ COMPACT JSON
        [HttpGet("raw-sessions-compact")]
        public async Task<IActionResult> DownloadCompactJson()
        {
            try
            {
                await _service.GenerateJson();

                string basePath =
                    Environment.GetEnvironmentVariable("DASHBOARD_STATIC_DIR")
                    ?? AppDomain.CurrentDomain.BaseDirectory;

                var filePath = Path.Combine(basePath, "raw-sessions-compact.json");

                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { message = "File not found" });

                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                return File(bytes, "application/json", "raw-sessions-compact.json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ COMMON QUERY EXECUTION
        private async Task<QueryTable> ExecuteQuery(string sql, List<MySqlParameter> parameters)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(sql, conn);

            foreach (var p in parameters)
                cmd.Parameters.Add(p);

            using var reader = await cmd.ExecuteReaderAsync();

            var columns = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .ToList();

            var rows = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();

                foreach (var col in columns)
                    row[col] = reader[col] == DBNull.Value ? null : reader[col];

                rows.Add(row);
            }

            return new QueryTable(columns, rows);
        }

        // ✅ DATE FILTER
        private static void ApplyDateFilter(
            StringBuilder sql,
            List<MySqlParameter> parameters,
            Dictionary<string, List<string>> query)
        {
            if (!query.TryGetValue("range", out var values))
                return;

            var range = values.FirstOrDefault();
            var now = DateTime.Now;

            DateTime from = range switch
            {
                "currentMonth" => new DateTime(now.Year, now.Month, 1),
                "lastMonth" => new DateTime(now.Year, now.Month, 1).AddMonths(-1),
                "thisYear" => new DateTime(now.Year, 1, 1),
                _ => DateTime.MinValue
            };

            sql.Append(" AND StartTime >= @from");
            parameters.Add(new MySqlParameter("@from", from));
        }
    }
}