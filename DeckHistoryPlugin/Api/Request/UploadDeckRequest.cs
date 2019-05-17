using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api.Request
{
    class UploadDeckRequest
    {
        [JsonProperty("deckname")]
        public string DeckName { get; set; }

        [JsonProperty("deckstring")]
        public string DeckCode { get; set; }

        [JsonProperty("client_key")]
        public string ClientKey { get; set; }
    }
}
