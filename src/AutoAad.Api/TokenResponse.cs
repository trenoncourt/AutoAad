using Newtonsoft.Json;

namespace AutoAad.Api
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        [JsonProperty("expires_in")]
        public int ExpireInMinutes { get; set; }
    }
}