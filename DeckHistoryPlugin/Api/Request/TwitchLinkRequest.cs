using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api.Request
{
    public class TwitchLinkRequest
    {
        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
