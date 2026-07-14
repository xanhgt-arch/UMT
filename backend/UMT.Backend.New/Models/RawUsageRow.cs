using MySqlConnector;

namespace UMT.Backend.Models
{
    public class RawUsageRow
    {
        public int SrNo { get; set; }
        public string? ApplicationName { get; set; }
        public string? Functionality { get; set; }
        public string? CadTool { get; set; }
        public string? UserID { get; set; }
        public string? MachineID { get; set; }
        public string? Domain { get; set; }
        public string? Region { get; set; }
        public string? ProductLine { get; set; }

        // ✅ FIXED TYPES
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }

        public string? Status { get; set; }

        public bool? IsVDI { get; set; }
        public bool? IsProd { get; set; }

        public string? CustomerName { get; set; }
        public string? CadLaunchPath { get; set; }

        public static RawUsageRow FromReader(MySqlDataReader reader)
        {
            return new RawUsageRow
            {
                SrNo = ToInt32(reader["SrNo"]),
                ApplicationName = ToStringOrNull(reader["ApplicationName"]),
                Functionality = ToStringOrNull(reader["Functionality"]),
                CadTool = ToStringOrNull(reader["CadTool"]),
                UserID = ToStringOrNull(reader["UserID"]),
                MachineID = ToStringOrNull(reader["MachineID"]),
                Domain = ToStringOrNull(reader["Domain"]),
                Region = ToStringOrNull(reader["Region"]),
                ProductLine = ToStringOrNull(reader["ProductLine"]),

                // ✅ SAFE Date Parsing
                StartTime = ToDateTimeOrNull(reader["StartTime"]),
                StopTime = ToDateTimeOrNull(reader["StopTime"]),

                Status = ToStringOrNull(reader["Status"]),

                // ✅ SAFE Boolean Mapping
                IsVDI = ToBoolOrNull(reader["IsVDI"]),
                IsProd = ToBoolOrNull(reader["IsProd"]),

                CustomerName = HasColumn(reader, "CustomerName")
                    ? ToStringOrNull(reader["CustomerName"])
                    : null,

                CadLaunchPath = ToStringOrNull(reader["CadLaunchPath"])
            };
        }

        // ================= HELPERS =================

        private static bool HasColumn(MySqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static int ToInt32(object value)
        {
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private static string? ToStringOrNull(object value)
        {
            return value == null || value == DBNull.Value ? null : Convert.ToString(value);
        }

        private static DateTime? ToDateTimeOrNull(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            if (value is DateTime dt)
                return dt;

            DateTime parsed;
            if (DateTime.TryParse(Convert.ToString(value), out parsed))
                return parsed;

            return null;
        }

        private static bool? ToBoolOrNull(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            var str = value.ToString();

            if (str == "1" || str?.ToLower() == "true")
                return true;

            if (str == "0" || str?.ToLower() == "false")
                return false;

            return null;
        }
    }
}