using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api.Request
{
    class UploadGameRequest
    {
        [JsonProperty("opponent_name")]
        public String OpponentName { get; set; }

        [JsonProperty("opponent_deck")]
        public String OpponentDeck { get; set; }

        [JsonProperty("time")]
        public String Time { get; set; }

        [JsonProperty("result")]
        public int Result { get; set; }
    }
}
