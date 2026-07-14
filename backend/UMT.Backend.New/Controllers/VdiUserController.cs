using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using UMT.Backend.Services;

namespace UMT.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VdiController : ControllerBase
    {
        private readonly MySqlConnectionFactory _factory;

        public VdiController(MySqlConnectionFactory factory)
        {
            _factory = factory;
        }

        // ✅ Safe user extraction (matches old logic)
        private string GetCurrentUser()
        {
            var identity = User?.Identity?.Name ?? HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(identity))
                return "UNKNOWN";

            return identity.Split('\\', '/').LastOrDefault() ?? "UNKNOWN";
        }

        // ✅ Admin check
        private async Task<bool> IsAdmin()
        {
            var user = GetCurrentUser();

            if (string.IsNullOrWhiteSpace(user) || user == "UNKNOWN")
                return false;

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            var sql = $"SELECT COUNT(*) FROM {_factory.TableName("mst_cooper_admins")} WHERE LOWER(UserId) = LOWER(@u)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", user);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
        }

        private IActionResult ForbiddenAdminOnly()
        {
            return Ok(new List<object>());
        }

        // ✅ GET ALL (EXACT SAME STRUCTURE AS 4.6.2)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!await IsAdmin())
                    return ForbiddenAdminOnly();

                var list = new List<object>();

                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                string sql = $"SELECT * FROM {_factory.TableName("mst_vdi_user_detail")}";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                int index = 1;

                while (await reader.ReadAsync())
                {
                    string userId = Convert.ToString(reader["UserId"]) ?? "";
                    string domain = Convert.ToString(reader["Domain"]) ?? "";
                    string region = Convert.ToString(reader["Region"]) ?? "";

                    string? createdDate = reader["CreatedDate"] != DBNull.Value
                        ? Convert.ToDateTime(reader["CreatedDate"]).ToString("o")
                        : null;

                    string? modifiedDate = reader["ModifiedDate"] != DBNull.Value
                        ? Convert.ToDateTime(reader["ModifiedDate"]).ToString("o")
                        : null;

                    list.Add(new
                    {
                        id = "vdi-" + index.ToString("D3"),

                        fullName = userId,
                        email = userId + "@cooperstandard.com",
                        domain = domain,
                        region = region,

                        hostname = "HOST-" + Guid.NewGuid().ToString("N").Substring(0, 6),
                        status = "Active",
                        lastSeen = DateTime.UtcNow.ToString("o"),

                        createdDate = createdDate,
                        createdBy = Convert.ToString(reader["CreatedBy"]),
                        modifiedDate = modifiedDate,
                        modifiedBy = Convert.ToString(reader["ModifiedBy"])
                    });

                    index++;
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ ADD
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] VdiRequest model)
        {
            try
            {
                if (!await IsAdmin())
                    return ForbiddenAdminOnly();

                if (model == null ||
                    string.IsNullOrWhiteSpace(model.UserId) ||
                    string.IsNullOrWhiteSpace(model.Domain) ||
                    string.IsNullOrWhiteSpace(model.Region))
                {
                    return BadRequest(new { message = "Invalid input" });
                }

                string userId = model.UserId.Trim();
                string domain = model.Domain.Trim();
                string region = model.Region.Trim();

                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                string currentUser = GetCurrentUser();
                DateTime now = DateTime.Now;

                string table = _factory.TableName("mst_vdi_user_detail");

                // ✅ duplicate check
                var check = new MySqlCommand(
                    $"SELECT COUNT(*) FROM {table} WHERE LOWER(UserId)=LOWER(@u)", conn);
                check.Parameters.AddWithValue("@u", userId);

                if (Convert.ToInt32(await check.ExecuteScalarAsync()) > 0)
                {
                    return Conflict(new { message = "VDI user already exists" });
                }

                var cmd = new MySqlCommand(
                    $"INSERT INTO {table} (UserId,Domain,Region,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy) " +
                    $"VALUES (@u,@d,@r,@cd,@cb,@md,@mb)", conn);

                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@d", domain);
                cmd.Parameters.AddWithValue("@r", region);
                cmd.Parameters.AddWithValue("@cd", now);
                cmd.Parameters.AddWithValue("@cb", currentUser);
                cmd.Parameters.AddWithValue("@md", now);
                cmd.Parameters.AddWithValue("@mb", currentUser);

                await cmd.ExecuteNonQueryAsync();

                return Ok(new
                {
                    id = "vdi-" + now.Ticks,

                    fullName = userId,
                    email = userId + "@cooperstandard.com",
                    domain = domain,
                    region = region,

                    hostname = "HOST-" + Guid.NewGuid().ToString("N").Substring(0, 6),
                    status = "Active",
                    lastSeen = DateTime.UtcNow.ToString("o"),

                    createdDate = now.ToString("o"),
                    createdBy = currentUser,
                    modifiedDate = now.ToString("o"),
                    modifiedBy = currentUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ UPDATE
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, [FromBody] VdiRequest model)
        {
            try
            {
                if (!await IsAdmin())
                    return ForbiddenAdminOnly();

                if (string.IsNullOrWhiteSpace(userId) ||
                    model == null ||
                    string.IsNullOrWhiteSpace(model.Domain) ||
                    string.IsNullOrWhiteSpace(model.Region))
                {
                    return BadRequest(new { message = "Invalid input" });
                }

                string routeUserId = userId.Trim();
                string domain = model.Domain.Trim();
                string region = model.Region.Trim();

                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                string currentUser = GetCurrentUser();
                DateTime now = DateTime.Now;

                string table = _factory.TableName("mst_vdi_user_detail");

                var cmd = new MySqlCommand(
                    $"UPDATE {table} SET Domain=@d, Region=@r, ModifiedDate=@md, ModifiedBy=@mb WHERE LOWER(UserId)=LOWER(@u)",
                    conn);

                cmd.Parameters.AddWithValue("@u", routeUserId);
                cmd.Parameters.AddWithValue("@d", domain);
                cmd.Parameters.AddWithValue("@r", region);
                cmd.Parameters.AddWithValue("@md", now);
                cmd.Parameters.AddWithValue("@mb", currentUser);

                int rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    return NotFound(new { message = "Record not found" });

                return Ok(new
                {
                    fullName = routeUserId,
                    email = routeUserId + "@cooperstandard.com",
                    domain = domain,
                    region = region,

                    hostname = "HOST-" + Guid.NewGuid().ToString("N").Substring(0, 6),
                    status = "Active",
                    lastSeen = DateTime.UtcNow.ToString("o"),

                    modifiedDate = now.ToString("o"),
                    modifiedBy = currentUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

        // ✅ DELETE
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                if (!await IsAdmin())
                    return ForbiddenAdminOnly();

                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { message = "UserId is required" });

                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                var cmd = new MySqlCommand(
                    $"DELETE FROM {_factory.TableName("mst_vdi_user_detail")} WHERE LOWER(UserId)=LOWER(@u)",
                    conn);

                cmd.Parameters.AddWithValue("@u", userId.Trim());

                int rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                    return NotFound(new { message = "Record not found" });

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.ToString() });
            }
        }

    }

    public class VdiRequest
    {
        public string? UserId { get; set; }
        public string? Domain { get; set; }
        public string? Region { get; set; }
    }
}
