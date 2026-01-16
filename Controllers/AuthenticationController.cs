using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGAdmin.Data;
using PGAdmin.Models;
using PGAdmin.Services;

namespace PGAdmin.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly ApplicationDbContext _context;

        public AuthenticationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ITokenService tokenService, ILogger<AuthenticationController> logger)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("ThirdPartyLogin")]
        public async Task<IActionResult> ThirdPartyLogin([FromBody] ThirdPartyLoginRequest request)
        {
            try
            {
                var verifier = new ThirdPartyTokenVerifier();
                var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogWarning("Access token is missing");
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Access token is missing or invalid."
                    });
                }

                _logger.LogInformation("Verifying Zalo token");

                var userInfo = await verifier.GetThirdPartyTokenInfoAsync(accessToken);

                // Kiểm tra kỹ userInfo
                if (userInfo == null)
                {
                    _logger.LogError("GetThirdPartyTokenInfoAsync returned null");
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Failed to verify Zalo token",
                        ErrorCode = "TOKEN_VERIFICATION_FAILED"
                    });
                }

                _logger.LogInformation($"Zalo API Response - Error: {userInfo.Error}, Message: {userInfo.Message}");

                // Kiểm tra error từ Zalo
                if (userInfo.Error != 0)
                {
                    _logger.LogError($"Zalo API Error: {userInfo.Error} - {userInfo.Message}");
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = userInfo.Message ?? "Zalo authentication failed",
                        ErrorCode = userInfo.Error
                    });
                }

                // Kiểm tra Id trước khi dùng
                if (string.IsNullOrEmpty(userInfo.Id))
                {
                    _logger.LogError("Zalo returned empty or null user ID");
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Could not retrieve user ID from Zalo",
                        ErrorCode = "EMPTY_USER_ID"
                    });
                }

                _logger.LogInformation($"Zalo user verified - ID: {userInfo.Id}, Name: {userInfo.Name}");

                // TÌM USER BẰNG ZALO ID (không dùng phone nữa)
                var pg = await _context.PG.FirstOrDefaultAsync(p => p.ZaloId == userInfo.Id);

                if (pg != null)
                {
                    _logger.LogInformation($"Existing user found - ID: {pg.Id}");
                    return Ok(new
                    {
                        Success = true,
                        Result = new
                        {
                            ID = pg.Id,
                            Phone = pg.Phone,
                            Name = pg.FirstName,
                            ZaloId = pg.ZaloId,
                            Status = pg.Status
                        }
                    });
                }

                // TẠO USER MỚI - Tạo phone number từ ZaloId
                _logger.LogInformation("Creating new user");

                var phoneNumber = "09" + userInfo.Id.Substring(Math.Max(0, userInfo.Id.Length - 8)).PadLeft(8, '0');

                // Đảm bảo phone unique
                var existingPhone = await _context.PG.AnyAsync(p => p.Phone == phoneNumber);
                if (existingPhone)
                {
                    // Nếu phone bị trùng, thêm timestamp
                    phoneNumber = "09" + DateTime.UtcNow.Ticks.ToString().Substring(0, 8);
                }

                var pgNew = new PG
                {
                    FirstName = userInfo.Name ?? "Zalo User",
                    LastName = "",
                    Phone = phoneNumber,
                    DateOfBirth = DateTime.TryParse(userInfo.Birthday, out var dob)
                          ? DateTime.SpecifyKind(dob, DateTimeKind.Utc)
                          : (DateTime?)null,
                    Gender = "Male",
                    Status = 0,
                    ZaloId = userInfo.Id,
                    AvatarUrl = userInfo.Picture?.Data?.Url ?? "",
                    QrCode = userInfo.Id,
                };

                _context.PG.Add(pgNew);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New user created - ID: {pgNew.Id}, Phone: {pgNew.Phone}");

                return Ok(new
                {
                    Success = true,
                    Result = new
                    {
                        ID = pgNew.Id,
                        Phone = pgNew.Phone,
                        Name = pgNew.FirstName,
                        ZaloId = pgNew.ZaloId,
                        Status = pgNew.Status
                    }
                });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error");
                var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError($"Inner exception: {innerMsg}");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Database error",
                    Detail = innerMsg
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ThirdPartyLogin");
                _logger.LogError($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error",
                    Detail = ex.Message
                });
            }
        }

        [HttpPost("UserInfo")]
        public async Task<IActionResult> ThirdPartyVerify()
        {
            // Step 1: Retrieve the Zalo access token from the request header
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is missing or invalid.");
            }

            // Step 1: Verify the third-party token (implement this as per the third-party API)
            var ZaloId = await VerifyThirdPartyZaloIdAsync(accessToken);
            if (String.IsNullOrEmpty(ZaloId))
            {
                return Unauthorized("Invalid third-party token.");
            }
            // Step 2: Check if the user exists in the database using the phone number
            var user = await _context.PG.FirstOrDefaultAsync(m => m.ZaloId == ZaloId);

            return Ok(new { Success = user != null });
        }

        private async Task<bool> VerifyThirdPartyTokenAsync(string token)
        {
            var verifier = new ThirdPartyTokenVerifier();
            bool isValid = await verifier.VerifyThirdPartyTokenAsync(token);
            // Logic to verify the third-party token
            // You can call an external API, etc. For now, let's assume it's always valid.
            return isValid; // Placeholder for actual token verification logic.
        }

        private async Task<string> VerifyThirdPartyZaloIdAsync(string token)
        {
            var verifier = new ThirdPartyTokenVerifier();
            string isValid = await verifier.GetZaloIdFromToken(token);
            // Logic to verify the third-party token
            // You can call an external API, etc. For now, let's assume it's always valid.
            return isValid; // Placeholder for actual token verification logic.
        }

        private async Task<string> GetPhoneFromZaloAsync(string token, string code)
        {
            var verifier = new ThirdPartyTokenVerifier();
            string phone = await verifier.GetZaloPhoneNumberFromToken(token, code);

            // Check if the phone number starts with "84" and has 11 digits
            if (!string.IsNullOrEmpty(phone) && phone.StartsWith("84") && phone.Length == 11)
            {
                // Replace the "84" prefix with "0"
                return "0" + phone.Substring(2); // Remove the "84" and prepend "0"
            }

            // If the phone number doesn't match the desired format, return it as is
            return phone;
        }
    }

    public class ThirdPartyLoginRequest
    {
        public string PhoneNumber { get; set; } // Phone number provided by the third-party provider
    }
}