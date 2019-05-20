using DeckHistoryPlugin.Api;
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

        private int MapClassNameToHero(string className)
        {
            switch(className)
            {
                case "Mage": return 637;
                case "Hunter": return 31;
                case "Warrior": return 7;
                case "Shaman": return 1066;
                case "Druid": return 274;
                case "Rogue": return 930;
                case "Paladin": return 671;
                case "Warlock": return 893;
                case "Priest": return 813;
                default: return 0;
            }
        }

        /// <summary>
        /// Generates a deckcode for the deck that was last played during the game
        /// </summary>
        /// <returns></returns>
        private string GenerateDeckCode()
        {
            // Retrieve the currently played deck
            var game = Hearthstone_Deck_Tracker.API.Core.Game;
            var selectedDeck = game.CurrentSelectedDeck;
            var activeDeck = DeckList.Instance.ActiveDeckVersion;

            // Convert the deck to dbf_id format
            Dictionary<int, int> card_dbf = new Dictionary<int, int>();
            try
            {
                // if a deck was recorded
                if (selectedDeck != null)
                {
                    selectedDeck.Cards.ForEach(card =>
                    {
                        card_dbf.Add(HearthDb.Cards.All[card.Id].DbfId, card.Count);
                    });
                }
                else if (activeDeck != null)
                {
                    // try to fallback to active deck if selected deck was not detected by HearthMirror
                    foreach (var card in activeDeck.Cards)
                    {
                        card_dbf.Add(HearthDb.Cards.All[card.Id].DbfId, card.Count);
                    }
                } else
                {
                    // otherwise, we cannot process the deck
                    throw new Exception();
                }
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
                    HeroDbfId = selectedDeck != null ? HearthDb.Cards.All[selectedDeck.Hero].DbfId : MapClassNameToHero(activeDeck.Class),
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
            var uploadTask = Task.Run<bool>(async () => await ApiWrapper.UploadDeck(Hearthstone_Deck_Tracker.API.Core.Game.CurrentSelectedDeck != null ? Hearthstone_Deck_Tracker.API.Core.Game.CurrentSelectedDeck.Name : DeckList.Instance.ActiveDeckVersion.Name, deckCode));
            uploadTask.Wait();

            // and remember the uploaded deck, if the upload was successfull
            if (uploadTask.Result)
            {
                Config.Instance.LastDeckcodeUploaded = deckCode;
                Config.Save();
            }
        }
    }
}
