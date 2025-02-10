using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PGAdmin.Services
{
    public class ThirdPartyTokenVerifier
    {
        private const string ZaloAppSecretKey = "P8RIXgwHs3Gh7M9j8J4S";
        //private const string ZaloAppSecretKey = "URdCs3jo3dfZO7TPXOWv";
        private const string ZaloApiUrl = "https://graph.zalo.me/v2.0/me";

        // Method to calculate HMAC-SHA256
        private static string CalculateHMacSHA256(string data, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public async Task<ZaloApiResponse> GetThirdPartyTokenInfoAsync(string accessToken)
        {
            try
            {
                // Calculate the appsecret_proof using HMAC-SHA256
                var appSecretProof = CalculateHMacSHA256(accessToken, ZaloAppSecretKey);

                // Prepare the HttpClient and set the headers and query parameters
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("access_token", accessToken);
                    client.DefaultRequestHeaders.Add("appsecret_proof", appSecretProof);

                    // Add the query parameters
                    var url = $"{ZaloApiUrl}?fields=id,name,birthday,picture";

                    // Send GET request
                    var response = await client.GetAsync(url);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response body as a string
                        var content = await response.Content.ReadAsStringAsync();

                        var result = JsonSerializer.Deserialize<ZaloApiResponse>(content);

                        return result;  // Return the deserialized object
                    }
                    else
                    {
                        // Handle failed response
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;  // Return null in case of failure
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;  // Return null in case of an exception
            }
        }


        // Verify the third-party token using the Zalo API
        public async Task<bool> VerifyThirdPartyTokenAsync(string accessToken)
        {
            try
            {
                // Use the previous method to get the response as an object
                var response = await GetThirdPartyTokenInfoAsync(accessToken);  // Reusing the previous function

                if (response != null)
                {
                    // Check if the 'id' field exists in the response
                    Console.WriteLine("User ID: " + response.Id);

                    // If we have a valid ID, consider the token valid
                    return true;  // Token is valid
                }
                else
                {
                    // Handle failed response
                    Console.WriteLine("Token is invalid or could not be verified.");
                    return false;  // Token is invalid
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;  // Handle any exceptions
            }
        }

        public async Task<string> GetZaloIdFromToken(string accessToken)
        {
            try
            {
                // Calculate the appsecret_proof using HMAC-SHA256
                var appSecretProof = CalculateHMacSHA256(accessToken, ZaloAppSecretKey);

                // Prepare the HttpClient and set the headers and query parameters
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("access_token", accessToken);
                    client.DefaultRequestHeaders.Add("appsecret_proof", appSecretProof);

                    // Add the query parameters
                    var url = $"{ZaloApiUrl}?fields=id,name,birthday,picture";

                    // Send GET request
                    var response = await client.GetAsync(url);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and parse the response body using System.Text.Json
                        var content = await response.Content.ReadAsStringAsync();

                        // Parse the JSON response without Newtonsoft.Json
                        using (JsonDocument doc = JsonDocument.Parse(content))
                        {
                            // You can access the properties here if needed
                            var root = doc.RootElement;
                            Console.WriteLine("Response Body: " + root.ToString());

                            // Example: Check if the 'id' field exists in the response
                            if (root.TryGetProperty("id", out var id))
                            {
                                Console.WriteLine("User ID: " + id.GetString());
                                return id.GetString() ?? "";  // Token is valid
                            }
                        }
                    }
                    else
                    {
                        // Handle failed response
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return "";  // Token is invalid
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return "";
            }
        }

        public async Task<string> GetZaloPhoneNumberFromToken(string accessToken, string code)
        {
            try
            {
                // Prepare the HttpClient and set the headers
                using (var client = new HttpClient())
                {
                    // Set headers: access_token, code, and secret_key
                    client.DefaultRequestHeaders.Add("access_token", accessToken);
                    client.DefaultRequestHeaders.Add("code", code);
                    client.DefaultRequestHeaders.Add("secret_key", ZaloAppSecretKey);

                    // API endpoint for Zalo to get user info
                    var url = "https://graph.zalo.me/v2.0/me/info";

                    // Send GET request
                    var response = await client.GetAsync(url);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and parse the response body using System.Text.Json
                        var content = await response.Content.ReadAsStringAsync();

                        // Parse the JSON response
                        using (JsonDocument doc = JsonDocument.Parse(content))
                        {
                            // Access the root element
                            var root = doc.RootElement;

                            // Check if 'data' property exists and contains a 'number' field
                            if (root.TryGetProperty("data", out var data) && data.TryGetProperty("number", out var number))
                            {
                                // Return the phone number if found
                                return number.GetString() ?? "";
                            }
                            else
                            {
                                // Handle case where 'number' is not found
                                Console.WriteLine("Phone number not found in response.");
                                return "";
                            }
                        }
                    }
                    else
                    {
                        // Handle failed response
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return "";  // Return empty string in case of failure
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Exception: {ex.Message}");
                return "";  // Return empty string in case of exception
            }
        }

    }
    public class ZaloApiResponse
    {
        [JsonPropertyName("birthday")]
        public string Birthday { get; set; }
        
        [JsonPropertyName("is_sensitive")]
        public bool IsSensitive { get; set; }
       
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("error")]
        public int Error { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("picture")]
        public PictureData Picture { get; set; }
    }

    public class PictureData
    {
        [JsonPropertyName("data")]
        public PictureDetails Data { get; set; }
    }

    public class PictureDetails
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

}
