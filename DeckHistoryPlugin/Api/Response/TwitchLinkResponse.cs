using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api.Response
{
    public class TwitchLinkResponse : Response
    {
        [JsonProperty("auth_url")]
        public string AuthUrl { get; set; }
    }
}
