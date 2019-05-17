using DeckHistoryPlugin.Api.Request;
using DeckHistoryPlugin.Api.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api
{
    internal class DeckHistoryClient
    {
        private readonly WebClient _webClient;
        private readonly ClientConfig _config;

        public DeckHistoryClient(ClientConfig config = null)
        {
            _webClient = new WebClient("DH/HDT");
            _config = config ?? new ClientConfig();
        }

        public async Task<LoginResponse> PostLogin(LoginRequest loginData)
        {
            var data = JsonConvert.SerializeObject(loginData);
            using (var response = await _webClient.PostJsonAsync(_config.UserLoginUrl, data, false))
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
                return JsonConvert.DeserializeObject<LoginResponse>(reader.ReadToEnd());
        }
    }
}
