using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api
{
    class ClientConfig
    {
        private const string Base = "http://localhost:5000/api";

        private const string DefaultUserLoginUrl = Base + "/auth/login";
        private const string DefaultDeckUploadUrl = Base + "/deck/upload";
        private const string DefaultTwitchLinkUrl = Base + "/auth/twitch_session";

        public string UserLoginUrl { get; set; } = DefaultUserLoginUrl;

        public string DeckUploadUrl { get; set; } = DefaultDeckUploadUrl;

        public string TwitchLinkUrl { get; set; } = DefaultTwitchLinkUrl;
    }
}
