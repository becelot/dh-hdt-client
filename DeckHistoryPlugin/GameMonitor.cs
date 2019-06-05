using DeckHistoryPlugin.Api;
using DeckHistoryPlugin.Api.Request;
using HearthDb.Deckstrings;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin
{
    public class GameMonitor
    {

        /// <summary>
        /// Called whenever a game ends. Detects if the game was played with a different deck than before, and uploads the results accordingly.
        /// </summary>
        internal void OnGameEnd()
        {
            try
            {
                // Try to get played deck
                Deck deck = Deck.FromContext();
                string deckCode = deck.GenerateDeckCode();

                // if deckcode equals the previously played one, then return
                /*
                if (deckCode.Equals(Config.Instance.LastDeckcodeUploaded))
                {
                    return;
                }*/

                var stats = Hearthstone_Deck_Tracker.Core.Game.CurrentGameStats;

                var uploadTask = Task.Run<bool>(async () => 
                    await ApiWrapper.UploadDeckWithResult(
                        deck.Name,
                        deckCode,
                        stats.OpponentName,
                        "",
                        stats.EndTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                        stats.Result
                    )
                );
                uploadTask.Wait();

                // and remember the uploaded deck, if the upload was successfull
                if (uploadTask.Result)
                {
                    Config.Instance.LastDeckcodeUploaded = deckCode;
                    Config.Save();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                return;
            }
        }
    }
}
