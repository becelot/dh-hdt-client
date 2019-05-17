using HearthDb.Deckstrings;
using HearthDb.Enums;
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
        /// Generates a deckcode for the deck that was last played during the game
        /// </summary>
        /// <returns></returns>
        private string GenerateDeckCode()
        {
            // Retrieve the currently played deck
            var game = Hearthstone_Deck_Tracker.API.Core.Game;
            var selectedDeck = game.CurrentSelectedDeck;

            // Convert the deck to dbf_id format
            Dictionary<int, int> card_dbf = new Dictionary<int, int>();
            try
            {
                selectedDeck.Cards.ForEach(card =>
                {
                    card_dbf.Add(HearthDb.Cards.All[card.Id].DbfId, card.Count);
                });
            }
            catch (Exception exc)
            {
                Log.Error(exc);
                throw new Exception("Exception while parsing the currently played deck");
            }

            // Convert the deck to a serializable format
            Deck deck;
            try
            {
                deck = new Deck
                {
                    HeroDbfId = HearthDb.Cards.All[game.CurrentSelectedDeck.Hero].DbfId,
                    CardDbfIds = card_dbf,
                    Format = (FormatType)game.CurrentFormat
                };
            }
            catch (Exception exc)
            {
                Log.Error(exc);
                throw new Exception("Exception while constructing HearthMirror decklist");
            }

            // Extract deckcode
            string deckcode;
            try
            {
                deckcode = DeckSerializer.Serialize(deck, false);
            }
            catch (Exception exc)
            {
                Log.Error(exc);
                throw new Exception("Exception while serializing played deck");
            }

            // And return it
            Hearthstone_Deck_Tracker.Utility.Logging.Log.Info(deckcode);
            return deckcode;
        }

        /// <summary>
        /// Called whenever a game ends. Detects if the game was played with a different deck than before, and uploads the results accordingly.
        /// </summary>
        internal void OnGameEnd()
        {
            string deckCode = GenerateDeckCode();

            // if deckcode equals the previously played one, then return
            if (deckCode.Equals(Config.Instance.LastDeckcodeUploaded))
            {
                return;
            }

            // otherwise, we need to upload the new decklist to the backend
            Config.Instance.LastDeckcodeUploaded = deckCode;
            Config.Save();
        }
    }
}
