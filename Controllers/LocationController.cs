using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace PGAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private const string ZaloAppSecretKey = "P8RIXgwHs3Gh7M9j8J4S";
        //private const string ZaloAppSecretKey = "URdCs3jo3dfZO7TPXOWv";

        private readonly HttpClient _httpClient;

        public LocationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("GetLocation")]
        public async Task<IActionResult> GetLocation([FromBody] LocationRequest request)
        {
            try
            {
                // Zalo Graph API URL
                string url = "https://graph.zalo.me/v2.0/me/info";

                // Prepare the headers for the request
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("access_token", request.UserAccessToken);
                requestMessage.Headers.Add("code", request.Code);
                requestMessage.Headers.Add("secret_key", ZaloAppSecretKey);

                // Make the request to the Zalo API
                var response = await _httpClient.SendAsync(requestMessage);

                // Ensure the request was successful
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error fetching location data from Zalo");
                }

                // Parse the response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Optionally, you can parse the response as a JSON object
                var locationData = JsonConvert.DeserializeObject(responseContent);

                // Return the location data to the client
                return Content(locationData.ToJson(), "application/json");
                //return Ok(locationData.ToJson());
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while fetching location data: " + ex.Message);
            }
        }
    }

    // Request model for receiving input data
    public class LocationRequest
    {
        public string UserAccessToken { get; set; }
        public string Code { get; set; }
    }
}
