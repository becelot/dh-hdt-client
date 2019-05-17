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

        public async Task<LoginResponse> PostLogin(LoginRequest loginData) =>
            await PostData<LoginRequest, LoginResponse>(loginData, _config.UserLoginUrl);

        public async Task<UploadDeckResponse> PostUploadDeck(UploadDeckRequest deckData) => 
            await PostData<UploadDeckRequest, UploadDeckResponse>(deckData, _config.DeckUploadUrl);

        public async Task<TwitchLinkResponse> PostTwitchLink(TwitchLinkRequest authData) =>
            await PostData<TwitchLinkRequest, TwitchLinkResponse>(authData, _config.TwitchLinkUrl);

        public async Task<Response> PostData<Request, Response>(Request deckData, string url)
        {
            var data = JsonConvert.SerializeObject(deckData);
            using (var response = await _webClient.PostJsonAsync(url, data, false))
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
                return JsonConvert.DeserializeObject<Response>(reader.ReadToEnd());
        }
    }
}
