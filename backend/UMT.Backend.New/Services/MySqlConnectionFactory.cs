using MySqlConnector;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace UMT.Backend.Services
{
    public class MySqlConnectionFactory
    {
        private readonly string _connectionString;

        public MySqlConnectionFactory(IConfiguration config)
        {
            string Get(string key) =>
                config[key] ?? Environment.GetEnvironmentVariable(key);

            var server = Get("DB_HOST");
            var user = Get("DB_USER");
            var production = Get("PRODUCTION");
            var database = Get("DB_NAME");

            string password = production switch
            {
                "Tata" => Get("DB_PASSWORD_TATA"),
                "Test" => Get("DB_PASSWORD_TEST"),
                "Prod" => Get("DB_PASSWORD_PROD"),
                _ => ""
            };

            uint port = GetPort(Get("DB_PORT"));

            var builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Port = port,
                UserID = user,
                Password = password,
                Database = database,
                Pooling = true,
                MinimumPoolSize = 0,
                MaximumPoolSize = 10,
                ConnectionTimeout = 15,
                DefaultCommandTimeout = 120,
                SslMode = MySqlSslMode.None
            };

            _connectionString = builder.ConnectionString;
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<QueryTable> QueryTableAsync(string sql)
        {
            var columns = new List<string>();
            var rows = new List<Dictionary<string, object>>();

            using var connection = CreateConnection();
            await connection.OpenAsync();

            using var command = new MySqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                foreach (var column in columns)
                    row[column] = reader[column] == DBNull.Value ? null : reader[column];

                rows.Add(row);
            }

            return new QueryTable(columns, rows);
        }

        public string TableName(string table)
        {
            return $"`{table}`"; // DB name optional in Core (conn already scoped)
        }

        private static uint GetPort(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 3306;

            if (!uint.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var port)
                || port == 0 || port > 65535)
                throw new InvalidOperationException("DB_PORT must be valid.");

            return port;
        }
    }

    public class QueryTable
    {
        public List<string> Columns { get; set; }
        public List<Dictionary<string, object>> Rows { get; set; }

        public QueryTable(List<string> columns, List<Dictionary<string, object>> rows)
        {
            Columns = columns;
            Rows = rows;
        }
    }
}