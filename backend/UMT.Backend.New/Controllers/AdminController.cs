using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using UMT.Backend.Services;

namespace UMT.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly MySqlConnectionFactory _factory;

        public AdminController(MySqlConnectionFactory factory)
        {
            _factory = factory;
        }

        // ✅ SAME PATTERN AS AUTH CONTROLLER
        private string GetUser()
        {
            string identity = "";

            if (User != null &&
                User.Identity != null &&
                !string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                identity = User.Identity.Name;
            }

            if (string.IsNullOrWhiteSpace(identity) &&
                HttpContext != null &&
                HttpContext.User != null &&
                HttpContext.User.Identity != null &&
                !string.IsNullOrWhiteSpace(HttpContext.User.Identity.Name))
            {
                identity = HttpContext.User.Identity.Name;
            }

            if (string.IsNullOrWhiteSpace(identity))
                return "UNKNOWN";

            identity = identity.Trim();

            int index = identity.LastIndexOf('\\');

            if (index < 0)
                index = identity.LastIndexOf('/');

            if (index >= 0 && index < identity.Length - 1)
                return identity.Substring(index + 1);

            return identity;
        }

        private async Task<bool> IsAdmin()
        {
            var user = GetUser();

            if (string.IsNullOrWhiteSpace(user) || user == "UNKNOWN")
                return false;

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            string sql = $"SELECT COUNT(*) FROM {_factory.TableName("mst_cooper_admins")} WHERE LOWER(UserID)=LOWER(@u)";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", user.Trim());

            int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return count > 0;
        }

        private IActionResult ForbiddenAdminOnly()
        {
            return Ok(new List<object>());
        }

        // ✅ GET ALL (MATCHES OLD FORMAT)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!await IsAdmin())
                    return ForbiddenAdminOnly();

                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                var cmd = new MySqlCommand($"SELECT * FROM {_factory.TableName("mst_cooper_admins")}", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                var list = new List<object>();
                int i = 1;

                while (await reader.ReadAsync())
                {
                    list.Add(new
                    {
                        id = $"adm-{i++:D3}",
                        fullName = Convert.ToString(reader["UserID"]),
                        addedBy = Convert.ToString(reader["AddedBy"]),
                        addedOn = reader["AddedOn"] != DBNull.Value
                            ? Convert.ToDateTime(reader["AddedOn"]).ToString("o")
                            : null
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ ADD (MATCHES OLD RESPONSE)
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AdminRequest model)
        {
            try
            {
                if (!await IsAdmin())
                    return ForbiddenAdminOnly();

                if (model == null || string.IsNullOrWhiteSpace(model.UserId))
                    return BadRequest(new { message = "UserID is required" });

                string fullName = model.UserId.Trim();
                string currentUser = GetUser();

                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                // ✅ Duplicate check
                var check = new MySqlCommand(
                    $"SELECT COUNT(*) FROM {_factory.TableName("mst_cooper_admins")} WHERE LOWER(UserID)=LOWER(@u)",
                    conn);

                check.Parameters.AddWithValue("@u", fullName);

                if (Convert.ToInt32(await check.ExecuteScalarAsync()) > 0)
                    return Conflict(new { message = "Admin already exists" });

                var now = DateTime.UtcNow;

                var cmd = new MySqlCommand(
                    $"INSERT INTO {_factory.TableName("mst_cooper_admins")} (UserID, AddedOn, AddedBy) VALUES (@u,@d,@b)",
                    conn);

                cmd.Parameters.AddWithValue("@u", fullName);
                cmd.Parameters.AddWithValue("@d", now);
                cmd.Parameters.AddWithValue("@b", currentUser);

                await cmd.ExecuteNonQueryAsync();

                return Ok(new
                {
                    id = $"adm-{now.Ticks}",
                    fullName = fullName,
                    addedBy = currentUser,
                    addedOn = now.ToString("o")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ DELETE
        [HttpDelete("{fullName}")]
        public async Task<IActionResult> Delete(string fullName)
        {
            try
            {
                if (!await IsAdmin())
                    return ForbiddenAdminOnly();

                if (string.IsNullOrWhiteSpace(fullName))
                    return BadRequest(new { message = "UserID is required" });

                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                var cmd = new MySqlCommand(
                    $"DELETE FROM {_factory.TableName("mst_cooper_admins")} WHERE LOWER(UserID)=LOWER(@u)",
                    conn);

                cmd.Parameters.AddWithValue("@u", fullName.Trim());

                int rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    return NotFound(new { message = "Record not found" });

                return Ok(new { message = "Deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }
    }

    public class AdminRequest
    {
        public string? UserId { get; set; }
    }
}