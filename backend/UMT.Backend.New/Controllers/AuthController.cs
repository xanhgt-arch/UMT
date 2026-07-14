using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using UMT.Backend.Services;

namespace UMT.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MySqlConnectionFactory _factory;

        public AuthController(MySqlConnectionFactory factory)
        {
            _factory = factory;
        }

        // ✅ Get identity (matches old fallback behavior)
        private string GetIdentityName()
        {
            string identity = "";

            if (User != null &&
                User.Identity != null &&
                !string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                identity = User.Identity.Name;
            }

            // ✅ Fallback (IMPORTANT for IIS)
            if (string.IsNullOrWhiteSpace(identity) &&
                HttpContext != null &&
                HttpContext.User != null &&
                HttpContext.User.Identity != null &&
                !string.IsNullOrWhiteSpace(HttpContext.User.Identity.Name))
            {
                identity = HttpContext.User.Identity.Name;
            }

            return identity ?? "";
        }

        // ✅ Extract userId (same logic)
        private string ExtractUserId(string identity)
        {
            if (string.IsNullOrWhiteSpace(identity))
                return "";

            identity = identity.Trim();

            int index = identity.LastIndexOf('\\');

            if (index < 0)
                index = identity.LastIndexOf('/');

            if (index >= 0 && index < identity.Length - 1)
                return identity.Substring(index + 1).Trim();

            return identity;
        }

        // ✅ Admin check
        private async Task<bool> IsUserAdmin(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            string sql = $"SELECT COUNT(1) FROM {_factory.TableName("mst_cooper_admins")} WHERE LOWER(UserId)=LOWER(@u)";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", userId.Trim());

            int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return count > 0;
        }

        // ✅ MATCHES 4.6.2 EXACT
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                string identityName = GetIdentityName();
                string userId = ExtractUserId(identityName);

                bool isAuthenticated =
                    User != null &&
                    User.Identity != null &&
                    User.Identity.IsAuthenticated;

                // ✅ EXACT SAME BEHAVIOR AS OLD
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Ok(new
                    {
                        message = "Windows user identity not found.",
                        identityName = identityName,
                        userId = "",
                        isAuthenticated = false,
                        isAdmin = false,
                        role = "User"
                    });
                }

                bool isAdmin = await IsUserAdmin(userId);

                return Ok(new
                {
                    identityName = identityName,
                    userId = userId,
                    isAuthenticated = isAuthenticated,
                    isAdmin = isAdmin,
                    role = isAdmin ? "Administrator" : "User"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.ToString()
                });
            }
        }
    }
}