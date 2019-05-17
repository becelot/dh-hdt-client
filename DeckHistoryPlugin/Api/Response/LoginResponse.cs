using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api.Response
{
    public class LoginResponse : Response
    {

        [JsonProperty("api_key")]
        public string UploadToken { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
