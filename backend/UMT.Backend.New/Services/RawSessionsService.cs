using MySqlConnector;
using Newtonsoft.Json;
using System.Globalization;
using UMT.Backend.Models;

namespace UMT.Backend.Services
{
    public class RawSessionsService
    {
        private readonly MySqlConnectionFactory _factory;

        private static readonly object _lock = new object();

        public RawSessionsService(MySqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task GenerateJson()
        {
            lock (_lock)
            {
                GenerateJsonInternal().GetAwaiter().GetResult();
            }

            await Task.CompletedTask;
        }

        private async Task GenerateJsonInternal()
        {
            var rows = await LoadData();

            var compact = rows
                .Select((r, i) => ToCompactRowV2(r, i + 1))
                .ToList();

            var rawJson = rows;
            var domains = await GetDomainsFromDb();

            string basePath =
                Environment.GetEnvironmentVariable("DASHBOARD_STATIC_DIR")
                ?? AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            await WriteJsonAtomic(Path.Combine(basePath, "raw-sessions.json"), rawJson);
            await WriteJsonAtomic(Path.Combine(basePath, "raw-sessions-compact.json"), compact);
            await WriteJsonAtomic(Path.Combine(basePath, "domains.json"), domains);
        }

        private async Task<List<RawUsageRow>> LoadData()
        {
            var rows = new List<RawUsageRow>();

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            // The legacy ChartsController materialized ToolTracking in its
            // natural/SrNo order and then applied a stable ascending
            // StartTime sort.  The frontend export also sorts by startMs,
            // so this query must preserve SrNo order for equal timestamps.
            // Ordering DESC here reverses those ties and makes otherwise
            // identical export rows swap positions.
            string sql = $"SELECT * FROM {_factory.TableName("mst_tool_usage")} ORDER BY SrNo ASC";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                rows.Add(RawUsageRow.FromReader(reader));
            }

            return rows;
        }

        // ✅ ATOMIC WRITE (CRITICAL FIX)
        private async Task WriteJsonAtomic<T>(string fullPath, T data)
        {
            var tempPath = fullPath + ".tmp";

            var json = JsonConvert.SerializeObject(data);

            await File.WriteAllTextAsync(tempPath, json);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            File.Move(tempPath, fullPath);
        }

        private object[] ToCompactRowV2(RawUsageRow row, int index)
        {
            DateTime.TryParse(row.StartTime?.ToString(), out var start);
            DateTime.TryParse(row.StopTime?.ToString(), out var stop);

            bool validStart = start > DateTime.MinValue;
            bool validStop = stop > DateTime.MinValue;

            long startMs = validStart
                ? new DateTimeOffset(start).ToUnixTimeMilliseconds()
                : 0;

            long? stopMs = validStop
                ? new DateTimeOffset(stop).ToUnixTimeMilliseconds()
                : null;

            return new object[]
            {
                $"sess-{index:D4}",
                row.SrNo,
                row.ApplicationName,
                row.Functionality,
                row.CadTool,
                row.UserID,
                row.MachineID,
                row.Domain,
                row.Region,
                row.ProductLine,

                validStart ? start.ToString("yyyy-MM-dd HH:mm:ss.fff") : null,
                validStop ? stop.ToString("yyyy-MM-dd HH:mm:ss.fff") : null,

                startMs,
                stopMs,

                validStart ? start.Year : 0,
                validStart ? start.Month - 1 : 0,
                validStart ? start.Day - 1 : 0,

                validStart ? start.ToString("yyyy-MM") : null,
                validStart ? start.ToString("MMM ''yy") : null,

                row.Status,

                // IsVDI is a bool? (mapped via ToBoolOrNull in RawUsageRow), so
                // bool.ToString() yields "True"/"False" and `?.ToString() == "1"`
                // was always false -> every row exported as "Non-VDI" (IsVDI FALSE
                // in the CSV) regardless of the DB. Compare the bool value directly,
                // same as the IsProd fix below.
                row.IsVDI == true ? "VDI" : "Non-VDI",

                // IsProd is already a bool? (mapped via ToBoolOrnull in
                // RawUsageRow). COmpare teh value directly - bool.ToString()
                // yeilds "True"/"False", so `?.ToString() == "1"` was always
                //false and dropped every row from the production-only charts.
                row.IsProd,

                row.CustomerName,

                // Keep the existing timestamp fields unchanged. These two
                // appended fields reproduce the legacy client report's
                // server-side DateTime date conversion for CSV export only.
                validStart ? start.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : null,
                validStop ? stop.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : null
            };
        }

        private async Task<List<object>> GetDomainsFromDb()
        {
            var list = new List<object>();

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            string sql = $"SELECT DISTINCT Domain, Region FROM {_factory.TableName("mst_domain_details")}";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            int index = 1;

            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    id = $"dom-{index:D3}",
                    technicalDomain = reader["Domain"],
                    corporateGroup = reader["Domain"],
                    region = reader["Region"],
                    users = 1,
                    active = true
                });

                index++;
            }

            return list;
        }
    }
}
