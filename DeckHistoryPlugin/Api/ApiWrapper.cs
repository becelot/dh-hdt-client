using DeckHistoryPlugin.Api.Response;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api
{
    internal class ApiWrapper
    {
        private static readonly DeckHistoryClient Client = new DeckHistoryClient();

        public static event Action LoggedIn;
        public static event Action LoggedOut;


        public static async Task<string> Login(string username, string password)
        {
            if (Account.Instance.IsAuthenticated)
            {
                LoggedIn?.Invoke();
                return "Already authenticated";
            }

            LoginResponse response;
            try
            {
                response = await Client.PostLogin(new Request.LoginRequest { Username = username, Password = password });
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Webrequest to obtain login authorization failed");
            }

            switch (response.Status)
            {
                case 400:
                case 422:
                    return response.Message;
            }

            Account.Instance.Username = response.Username;
            Account.Instance.UploadToken = response.UploadToken;
            Account.Instance.TwitchId = response.TwitchId;
            Account.Save();

            LoggedIn?.Invoke();

            return "Successfully logged in!";
        }

        public static bool Logout()
        {
            var res = Account.Instance.Logout();
            if (res)
                LoggedOut?.Invoke();
            return res;
        }

        public static async Task<bool> UploadDeck(string deckName, string deckCode)
        {
            if (!Account.Instance.IsAuthenticated)
            {
                return false;
            }

            UploadDeckResponse response;
            try
            {
                response = await Client.PostUploadDeck(
                    new Request.UploadDeckRequest
                    {
                        DeckName = deckName,
                        DeckCode = deckCode,
                        ClientKey = Account.Instance.UploadToken
                    }
                );
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Webrequest to obtain login authorization failed");
            }

            return response.Status == 200;
        }
    }
}
