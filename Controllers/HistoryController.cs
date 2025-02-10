using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGAdmin.Data;
using PGAdmin.Models.Reward;

namespace PGAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{zaloID}")]
        public async Task<ActionResult<List<RewardOrderDetail>>> GetRewardOrderHistoryByZaloIDAsync(string zaloID)
        {
            var user = await _context.PG.FirstOrDefaultAsync(p => p.ZaloId == zaloID);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // Fetch reward orders by ZaloID
            var rewardOrders = _context.RewardOrderDetails
                .Include(ro => ro.RewardOrder)
                .Where(ro => ro.RewardOrder.PgId == user.Id)
                .Select(ro => new
                {
                    ro.Id,
                    ro.GiftId,
                    ro.GiftName,
                    ro.CreatedAt,
                    ro.ReceivingAt,
                    ro.Status
                }).OrderByDescending(p => p.CreatedAt)
                .ToList();

            if (rewardOrders == null || !rewardOrders.Any())
            {
                return NotFound(new { message = "No reward orders found for this ZaloID" });
            }

            return Ok(rewardOrders);
        }

        [HttpPost("Confirm/{id}")]
        public async Task<IActionResult> UpdateReceivingAt(int id)
        {
            // Fetch the RewardOrderDetail by Id
            var rewardOrderDetail = await _context.RewardOrderDetails.Include(r => r.RewardOrder).FirstOrDefaultAsync(rod => rod.Id == id);

            if (rewardOrderDetail == null)
            {
                return NotFound(new { message = "Reward order detail not found." });
            }

            var rewardOrder = rewardOrderDetail.RewardOrder;

            // Update the ReceivingAt and set Status to 2
            rewardOrderDetail.ReceivingAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            rewardOrderDetail.Status = 2;

            // Save the changes to the database
            _context.RewardOrderDetails.Update(rewardOrderDetail);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reward order detail updated successfully." });
        }
    }
}
