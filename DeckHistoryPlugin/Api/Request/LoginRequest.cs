using Newtonsoft.Json;

namespace DeckHistoryPlugin.Api.Request
{
    class LoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
