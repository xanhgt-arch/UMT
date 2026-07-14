using System.Text;

namespace UMT.Backend.Services
{
    public static class CsvWriter
    {
        public static string Write(
            IEnumerable<string> columns,
            IEnumerable<Dictionary<string, object>> rows)
        {
            var sb = new StringBuilder();

            // ✅ Header row
            sb.AppendLine(string.Join(",", columns.Select(Escape)));

            // ✅ Data rows
            foreach (var row in rows)
            {
                var values = columns.Select(col =>
                {
                    return row.TryGetValue(col, out var value)
                        ? Escape(value?.ToString())
                        : "";
                });

                sb.AppendLine(string.Join(",", values));
            }

            return sb.ToString();
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // ✅ Proper CSV escaping
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}