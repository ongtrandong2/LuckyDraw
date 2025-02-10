using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGAdmin.Data;
using PGAdmin.Models;
using PGAdmin.Services;

namespace PGAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckInController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckIn([FromBody] CheckInRequest checkInRequest)
        {
            if (checkInRequest == null)
            {
                return BadRequest("Invalid data.");
            }

            // Validate the token to get the user id
            var verifier = new ThirdPartyTokenVerifier();
            var zaloId = await verifier.GetZaloIdFromToken(checkInRequest.AccessToken);

            if (zaloId == "")
            {
                return Unauthorized("Invalid token.");
            }

            // Retrieve the user by ID
            var user = await _context.PG.FirstOrDefaultAsync(p => p.ZaloId == zaloId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Create CheckIn object
            var checkIn = new CheckIn
            {
                PgId = user.Id,
                CheckinLocation = checkInRequest.Latitude + "," + checkInRequest.Longitude,
                //CheckinTime = DateTimeOffset.FromUnixTimeMilliseconds(checkInRequest.Time).DateTime.ToUniversalTime(),
                CheckinTime = DateTime.SpecifyKind(
                    DateTimeOffset.FromUnixTimeMilliseconds(checkInRequest.Time).DateTime,
                    DateTimeKind.Utc),
                Note = checkInRequest.Note
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            var response = new { message = "Check-in successfully created.", checkInId = checkIn.Id };
            return Ok(response);
        }

        [HttpPost("Out")]
        public async Task<IActionResult> CreateCheckout([FromBody] CheckInRequest checkoutRequest)
        {
            if (checkoutRequest == null)
            {
                return BadRequest("Invalid data.");
            }

            // Validate the token to get the user id
            var verifier = new ThirdPartyTokenVerifier();
            var zaloId = await verifier.GetZaloIdFromToken(checkoutRequest.AccessToken);

            if (zaloId == "")
            {
                return Unauthorized("Invalid token.");
            }

            // Retrieve the user by ID
            var user = await _context.PG.FirstOrDefaultAsync(p => p.ZaloId == zaloId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Get the first CheckIn today for the user
            var checkIn = await _context.CheckIns
                .Where(c => c.PgId == user.Id && c.CheckinTime.Value.Date == DateTime.UtcNow.Date)
                .OrderBy(c => c.CheckinTime)
                .FirstOrDefaultAsync();

            if (checkIn == null)
            {
                return NotFound("No check-in found for today.");
            }

            // Set the CheckoutTime and CheckoutLocation
            checkIn.CheckoutTime = DateTime.SpecifyKind(
                DateTimeOffset.FromUnixTimeMilliseconds(checkoutRequest.Time).DateTime,
                DateTimeKind.Utc);
            checkIn.CheckoutLocation = checkoutRequest.Latitude + "," + checkoutRequest.Longitude;

            _context.CheckIns.Update(checkIn);
            await _context.SaveChangesAsync();

            var response = new { message = "Checkout successfully recorded.", checkInId = checkIn.Id };
            return Ok(response);
        }

        public class CheckInRequest
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public long Time { get; set; }
            public string Note { get; set; }
            public string AccessToken { get; set; }
        }
    }
}
