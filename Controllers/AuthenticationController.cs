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
                    return Unauthorized("Access token is missing or invalid.");
                }

                // Log để debug
                _logger.LogInformation($"Attempting to verify token and get user info");

                var userInfo = await verifier.GetThirdPartyTokenInfoAsync(accessToken);
                if (userInfo == null)
                {
                    _logger.LogWarning("Failed to get user info from token");
                    return Unauthorized("Invalid third-party token.");
                }

                _logger.LogInformation($"Getting phone number with code length: {request.PhoneNumber?.Length}");
                var phoneNumber = await GetPhoneFromZaloAsync(accessToken, request.PhoneNumber);

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    _logger.LogError("Failed to get phone number from Zalo");
                    return BadRequest("Failed to retrieve phone number");
                }

                var pg = await _context.PG.FirstOrDefaultAsync(p => p.Phone == phoneNumber);

                if (pg == null)
                {
                    var pgNew = new PG
                    {
                        FirstName = userInfo.Name,
                        LastName = "",
                        Phone = phoneNumber,
                        DateOfBirth = DateTime.TryParse(userInfo.Birthday, out var dob)
                              ? DateTime.SpecifyKind(dob, DateTimeKind.Utc)
                              : (DateTime?)null,
                        Gender = "Male",
                        Status = 0,
                        ZaloId = userInfo.Id,
                        AvatarUrl = userInfo.Picture?.Data?.Url ?? "", // Fix NullReference
                        QrCode = userInfo.Id,
                    };

                    _context.PG.Add(pgNew);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"PG created successfully with phone: {phoneNumber}");

                    return Ok(new
                    {
                        Success = true,
                        Result = new
                        {
                            ID = pgNew.Id,
                            Phone = pgNew.Phone,
                            Name = pgNew.FirstName,
                            ZaloId = pgNew.ZaloId,
                            pgNew.Status
                        }
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Result = new
                    {
                        ID = pg.Id,
                        pg.Phone,
                        Name = pg.FirstName,
                        pg.ZaloId,
                        pg.Status
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ThirdPartyLogin");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error",
                    Detail = ex.Message // Remove this in production
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
