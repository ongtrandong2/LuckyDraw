using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGAdmin.Data;
using PGAdmin.Models.Campaign;
using PGAdmin.Models.Reward;
using PGAdmin.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PGAdmin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CampaignController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string ZaloAccessToken = "P3SuSOf4_3zZDpHsYa3wEsitDtUANAHF8reXLDTxc6LKN3urzKwr35eu1Xck2Oj98YCFOyurdanUCni8qZgkE3mIAYAD8ReS654DMB9nYM0CP5Xyiqt6G2j5ItcARFjWF4HeUwjhvcuYNab3c5RTHpPKSdAL8-zMBn1y4emapnuV508YcHQBR1019MYd58PzD1q-PDGHeIjC9pO_fG623YaNF1Q41RKVP30SMz0GkN1vCt57urlq36idPY303ya_G1eC1iiAapPAB2GWyW-H0cv-A1RMLxqOUaSh3Pfof0GDTX08vq2OF0r9LIwaRUGI6q91FumpZZSS721_ds29HmvN57cnLzf0Ao5J2vuEtq402KHFXWI87m0kJ5Es38Xz7IKULF0Dh4j8Asa2-osWBsGP6cTbsKNyVObSzpK";

        public CampaignController(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        [HttpGet("active")]
        public IActionResult GetActiveCampaign()
        {
            DateTime currentDate = DateTime.UtcNow;

            var activeCampaign = _context.Campaigns.FirstOrDefault(c =>
                c.StartDate <= currentDate &&
                c.EndDate >= currentDate &&
                c.Status == 1);

            if (activeCampaign == null)
            {
                return BadRequest("No active campaign found.");
            }

            return Ok(activeCampaign);
        }

       

        [HttpGet("products")]
        public IActionResult GetAllProducts()
        {
            var products = _context.Products.ToList();

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }

        [HttpPost("SpinCount")]
        public IActionResult CalculateSpinCount([FromBody] List<ProductInput> productInputs)
        {
            DateTime currentDate = DateTime.UtcNow;

            var activeCampaign = _context.Campaigns
                .Include(c => c.GiftRules)
                    .ThenInclude(gr => gr.GiftConditions)
                        .ThenInclude(gc => gc.GiftProducts)
                .FirstOrDefault(c => c.StartDate <= currentDate && c.EndDate >= currentDate && c.Status == 1);

            if (activeCampaign == null)
            {
                return BadRequest("No active campaign found.");
            }

            int spinCount = GetCampaignSpinCount(activeCampaign, productInputs, activeCampaign.Scheme);

            return Ok(new { SpinCount = spinCount });
        }

        private int GetCampaignSpinCount(Campaign campaign, List<ProductInput> productInputs, SchemeType schemeType)
        {

            return schemeType == SchemeType.MaxSpin ? GetMaxSpin(campaign,productInputs) : GetSumSpin(campaign, productInputs);
        }

        private int GetMaxSpin(Campaign campaign, List<ProductInput> productInputs)
        {
            int maxSpinCount = 0;

            var productQuantityMap = productInputs.ToDictionary(p => p.ProductId, p => p.Quantity);

            foreach (var giftRule in campaign.GiftRules)
            {
                if (giftRule.Type == 1)
                {
                    var products = giftRule.GiftConditions.Select(gc => gc.GiftProducts
                    .OrderBy(gp => gp.ProductId)
                    .FirstOrDefault()).ToList();

                    bool conditionMet = true;
                    foreach (var product in products)
                    {
                        if (product == null || !productQuantityMap.ContainsKey(product.ProductId))
                        {
                            conditionMet = false;
                            continue;
                        }
                        if (product.Quantity > productQuantityMap[product.ProductId])
                        {
                            conditionMet = false;
                            continue;
                        }
                    }

                    if (conditionMet && maxSpinCount < giftRule.SpinCount)
                    {
                        maxSpinCount = giftRule.SpinCount;
                    }
                }
                else if (giftRule.Type == 2)
                {
                    double totalPrice = 0;

                    var productIds = productInputs.Select(pi => pi.ProductId).Distinct().ToList();

                    var selectedProducts = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();

                    var productDictionary = selectedProducts.ToDictionary(p => p.Id);

                    foreach (var productInput in productInputs)
                    {
                        // Check if the product exists in the dictionary
                        if (productDictionary.TryGetValue(productInput.ProductId, out var product))
                        {
                            totalPrice += productInput.Quantity * product.UnitPrice;
                        }
                    }
                    if (totalPrice >= giftRule.MinAmount && totalPrice <= giftRule.MaxAmount && maxSpinCount < giftRule.SpinCount)
                    {
                        maxSpinCount = giftRule.SpinCount;
                    }
                }

            }
            return maxSpinCount;
        }

        private int GetSumSpin(Campaign campaign, List<ProductInput> productInputs)
        {
            int totalSpinCount = 0;
            var productQuantityMap = productInputs.ToDictionary(p => p.ProductId, p => p.Quantity);

            foreach (var giftRule in campaign.GiftRules)
            {
                int spinCountForRule = 0;

                if (giftRule.Type == 1)
                {

                    foreach (var condition in giftRule.GiftConditions)
                    {
                        foreach (var product in condition.GiftProducts)
                        {
                            if (productQuantityMap.ContainsKey(product.ProductId))
                            {
                                int multiplier = productQuantityMap[product.ProductId] / product.Quantity;
                                spinCountForRule += multiplier * giftRule.SpinCount;
                            }
                        }
                    }

                }
                else if (giftRule.Type == 2) 
                {
                    double totalPrice = 0;
                    var productIds = productInputs.Select(pi => pi.ProductId).Distinct().ToList();

                    var selectedProducts = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();
                    var productDictionary = selectedProducts.ToDictionary(p => p.Id);

                    foreach (var productInput in productInputs)
                    {
                        if (productDictionary.TryGetValue(productInput.ProductId, out var product))
                        {
                            totalPrice += productInput.Quantity * product.UnitPrice;
                        }
                    }
                    if (totalPrice >= giftRule.MinAmount && totalPrice <= giftRule.MaxAmount)
                    {
                        spinCountForRule = giftRule.SpinCount;
                    }
                }

                totalSpinCount += spinCountForRule;
            }

            return totalSpinCount;
        }


        [HttpPost("CreatePublicRewardOrder")]
        public async Task<ActionResult<RewardOrder>> CreatePublicRewardOrder([FromBody] RewardOrderPublicRequest request)
        {
            DateTime currentDate = DateTime.UtcNow;

            // Get the current active campaign that allows Public QR access
            var activeCampaign = _context.Campaigns
                .FirstOrDefault(c =>
                    c.StartDate <= currentDate &&
                    c.EndDate >= currentDate &&
                    c.Status == 1 &&
                    c.PublicQrAccess == true); // Check for PublicQrAccess

            if (activeCampaign == null)
            {
                return BadRequest("No active campaign with public QR access found.");
            }

            // Assign the maxSpinWheel based on the campaign's NumberOfQrAccessScanPerUser
            int maxSpinWheel = 1;

            // Validate access token and get ZaloID
            var verifier = new ThirdPartyTokenVerifier();
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is missing or invalid.");
            }
            var userInfo = await verifier.GetThirdPartyTokenInfoAsync(accessToken);
            if (userInfo == null)
            {
                return Unauthorized("Invalid third-party token.");
            }

            var phoneNumber = await GetPhoneFromZaloAsync(accessToken, request.PhoneNumber);

            var totalSpin = _context.RewardOrders.Where(ro => ro.CustomerZaloId == userInfo.Id && ro.CampaignId == activeCampaign.Id)
                .Count();

            if (!string.IsNullOrEmpty(request.PgId))
            {
                if (totalSpin >= activeCampaign.NumberOfPGQrAccessScanPerUser)
                {
                    return Ok(new { Success = false, Reason = "NO_REMAINING_SPIN" });
                }
            } else if (totalSpin >= activeCampaign.NumberOfQrAccessScanPerUser)
            {
                return Ok(new { Success = false, Reason = "NO_REMAINING_SPIN" });
            }


            // Create RewardOrder
            var rewardOrder = new RewardOrder
            {
                CreatedAt = DateTime.UtcNow,
                PgId = null,
                CustomerZaloId = userInfo.Id,
                CustomerName = userInfo.Name,
                CustomerPhone = phoneNumber,
                CampaignId = activeCampaign.Id,
                SpintCount = maxSpinWheel, // Assign maxSpinWheel from campaign
                Products = new List<RewardOrderProduct>(),
                Details = new List<RewardOrderDetail>()
            };

            if (!string.IsNullOrEmpty(request.PgId))
            {
                var pg = await _context.PG.FirstOrDefaultAsync(p => p.ZaloId == request.PgId);
                if (pg != null)
                {
                    // Assign PgId to rewardOrder if PG exists
                    rewardOrder.PgId = pg.Id;
                }
            }

            // Save RewardOrder to database
            _context.RewardOrders.Add(rewardOrder);
            await _context.SaveChangesAsync();

            return Ok(new { CampaignId = activeCampaign.Id, OrderId = rewardOrder.Id, SpinCount = maxSpinWheel });
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


        [HttpPost("CreateRewardOrder")]
        public async Task<ActionResult<RewardOrder>> CreateRewardOrder([FromBody] RewardOrderRequest request)
        {
            DateTime currentDate = DateTime.UtcNow;

            var activeCampaign = _context.Campaigns
                .Include(c => c.GiftRules)
                    .ThenInclude(gr => gr.GiftConditions)
                        .ThenInclude(gc => gc.GiftProducts).FirstOrDefault(c =>
                c.StartDate <= currentDate &&
                c.EndDate >= currentDate &&
                c.Status == 1);

            if (activeCampaign == null)
            {
                return BadRequest("No active campaign found.");
            }

            int spinCount = GetCampaignSpinCount(activeCampaign, request.Products, activeCampaign.Scheme);

            // Validate access token and get ZaloID
            var verifier = new ThirdPartyTokenVerifier();
            var zaloId = await verifier.GetZaloIdFromToken(request.AccessToken);

            if (zaloId == "")
            {
                return Unauthorized("Invalid token.");
            }

            var user = await _context.PG.FirstOrDefaultAsync(p => p.ZaloId == zaloId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Get products' details from provided ProductIds
            var productIds = request.Products.Select(p => p.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();
            if (products == null || !products.Any())
            {
                return NotFound("Products not found");
            }

            // Create RewardOrder
            var rewardOrder = new RewardOrder
            {
                CreatedAt = DateTime.UtcNow,
                PgId = user.Id,
                CampaignId = activeCampaign.Id,
                SpintCount = spinCount,
                Products = new List<RewardOrderProduct>(),
                Details = new List<RewardOrderDetail>()
            };

            // Map products to RewardOrderProduct
            foreach (var item in request.Products)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    rewardOrder.Products.Add(new RewardOrderProduct
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        Price = (decimal)product.UnitPrice,
                        RewardOrder = rewardOrder
                    });
                }
            }

            // Save RewardOrder to database
            _context.RewardOrders.Add(rewardOrder);
            await _context.SaveChangesAsync();

            return Ok(new {Success = true, CampaignId = activeCampaign.Id, OrderId = rewardOrder.Id, SpinCount = spinCount });
        }

        [HttpPost("VerifyOrder/{id}")]
        public async Task<ActionResult> GetOrderById(int id, [FromBody] RewardOrderPublicRequest request)
        {
            var verifier = new ThirdPartyTokenVerifier();
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is missing or invalid.");
            }
            var userInfo = await verifier.GetThirdPartyTokenInfoAsync(accessToken);
            if (userInfo == null)
            {
                return Unauthorized("Invalid third-party token.");
            }

            var phoneNumber = await GetPhoneFromZaloAsync(accessToken, request.PhoneNumber);
            // Retrieve the RewardOrder along with its related products and details
            var rewardOrder = await _context.RewardOrders
                .Include(p => p.Campaign).ThenInclude(c => c.CampaignGifts)
                .Include(p => p.Details)
                .FirstOrDefaultAsync(ro => ro.Id == id);

            if (rewardOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            var availableGifts = rewardOrder.Campaign.CampaignGifts
                                     .Where(cg => cg.Quantity > 0)
                                     .Select(cg => new
                                     {
                                         Name = cg.Name,
                                         ImageUrl = cg.ImageUrl,
                                         Id = cg.Id
                                     })
                                     .ToList();


            // Map the entity to a DTO for a cleaner response (optional, but recommended)
            var rewardOrderDto = new
            {
                Id = rewardOrder.Id,
                CreatedAt = rewardOrder.CreatedAt,
                CustomerName = rewardOrder.CustomerName,
                CustomerPhone = rewardOrder.CustomerPhone,
                PgId = rewardOrder.PgId,
                CampaignId = rewardOrder.CampaignId,
                CampaignName = rewardOrder.Campaign.Name,
                CampaignDescription = rewardOrder.Campaign.Description,
                SpinCount = rewardOrder.SpintCount,
                RemainingSpintCount = Math.Max(rewardOrder.SpintCount - rewardOrder.Details.Count, 0), // Calculate remaining spin countrewardOrder,
                Gifts = availableGifts
            };

            // Update order with Zalo user information
            if (rewardOrder.CustomerZaloId == null) {
                rewardOrder.CustomerName = userInfo.Name;
                rewardOrder.CustomerPhone = phoneNumber;
                rewardOrder.CustomerZaloId = userInfo.Id;
                await _context.SaveChangesAsync();
            }
            

            return Ok(rewardOrderDto);
        }

        private async Task<IActionResult> SendMessage(string Phone, string CustomerName, string CustomerId, string GiftName)
        {
            var url = "https://business.openapi.zalo.me/message/template";

            var requestData = new
            {
                phone = "84" + Phone.Substring(1),
                template_id = "405463",
                template_data = new
                {
                    ten_KH = CustomerName,
                    ma_KH = CustomerId,
                    gift_name = GiftName,
                    ngay_tao_don = DateTime.Now.ToString("dd/MM/yyyy")
                },
                tracking_id = "tracking_id"
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = jsonContent
                };

                request.Headers.Add("access_token", ZaloAccessToken);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(new { message = "Message sent successfully", response = result });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { message = "Error sending message", error = response.ReasonPhrase });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost("SendMessage/{detailId}")]
        public async Task<ActionResult> SendMessage(int detailId, [FromBody] RewardOrderPublicRequest request)
        {
            var verifier = new ThirdPartyTokenVerifier();
            var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is missing or invalid.");
            }
            var userInfo = await verifier.GetThirdPartyTokenInfoAsync(accessToken);
            if (userInfo == null)
            {
                return Unauthorized("Invalid third-party token.");
            }

            var phoneNumber = await GetPhoneFromZaloAsync(accessToken, request.PhoneNumber);
            
            var orderDetail = await _context.RewardOrderDetails
                .Include(rod => rod.RewardOrder)
                .FirstOrDefaultAsync(rod => rod.Id == detailId);

            if (orderDetail == null)
            {
                return NotFound($"Order detail with ID {detailId} not found.");
            }

            var rewardOrder = orderDetail.RewardOrder;

            var result = await SendMessage(rewardOrder.CustomerPhone, rewardOrder.CustomerName, rewardOrder.CustomerZaloId, orderDetail.GiftName);

            return Ok(new { message = "Message sent successfully", response = result }); ;
        }

        [HttpPost("AssignGift/{rewardOrderId}")]
        public async Task<IActionResult> AssignGiftAsync(int rewardOrderId)
        {
            var rewardOrder = await _context.RewardOrders
                .Include(ro => ro.Campaign)
                    .ThenInclude(c => c.CampaignGifts)
                .Include(ro => ro.Details)
                .FirstOrDefaultAsync(ro => ro.Id == rewardOrderId);

            if (rewardOrder == null)
            {
                return NotFound("Reward order not found.");
            }

            // Start a transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Step 1: Get available gifts where quantity > 0
                    var availableGifts = rewardOrder.Campaign.CampaignGifts
                        .Where(cg => cg.RemainingQuantity > 0)
                        .ToList();

                    if (!availableGifts.Any())
                    {
                        return BadRequest("No available gifts to assign.");
                    }

                    // Step 2: Randomly select a gift
                    var random = new Random();
                    var selectedGift = availableGifts[random.Next(availableGifts.Count)];

                    // Step 3: Reduce the quantity of the selected gift
                    var campaignGift = _context.CampaignGifts
                        .FirstOrDefault(cg => cg.Id == selectedGift.Id);

                    if (campaignGift != null && campaignGift.Quantity > 0)
                    {
                        campaignGift.RemainingQuantity -= 1;

                        // Step 4: Add RewardOrderDetail to RewardOrder
                        var newDetail = new RewardOrderDetail
                        {
                            RewardOrderId = rewardOrder.Id,
                            GiftId = selectedGift.Id,
                            GiftName = selectedGift.Name,
                            CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                            Status = 1
                        };

                        rewardOrder.Details.Add(newDetail);

                        // Save changes in the database
                        await _context.SaveChangesAsync();

                        // Commit the transaction
                        await transaction.CommitAsync();
                        return Ok(new { message = "Gift assigned successfully", gift = new { Id = selectedGift.Id, Name = selectedGift.Name, DetailOrderId = newDetail.Id } });
                    }

                    return BadRequest("Failed to assign gift.");
                }
                catch (Exception ex)
                {
                    // In case of an error, rollback the transaction
                    await transaction.RollbackAsync();
                    return BadRequest("Failed to assign gift.");
                    //return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

    }

    public class RewardOrderRequest
    {
        public string AccessToken { get; set; }
        public List<ProductInput> Products { get; set; }
    }

    public class RewardOrderPublicRequest
    {
        public string PhoneNumber { get; set; }
        public string? PgId { get; set; }
    }

    public class ProductInput
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

    
