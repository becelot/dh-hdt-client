using HearthDb.Deckstrings;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearthDeck = HearthDb.Deckstrings.Deck;

namespace DeckHistoryPlugin
{
    public class Deck : HearthDeck
    {
        /// <summary>
        /// Maps the class name to the default basic hero
        /// </summary>
        /// <param name="className">Name of the class</param>
        /// <returns>Basic hero in dbfId format</returns>
        private static int MapClassNameToHero(string className)
        {
            switch (className)
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
                default: throw new Exception($"Class {className} could not be mapped to a DbfID");
            }
        }

        /// <summary>
        /// Generates a deckcode for the deck that was last played during the game
        /// </summary>
        /// <returns></returns>
        public string GenerateDeckCode()
        {
            // Extract deckcode
            string deckcode;
            try
            {
                deckcode = DeckSerializer.Serialize(this, false);
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
        /// Generates a new deck instance from the current context
        /// </summary>
        /// <returns></returns>
        public static Deck FromContext()
        {
            Deck result = new Deck();

            // Retrieve the currently played deck
            var game = Hearthstone_Deck_Tracker.API.Core.Game;
            var selectedDeck = game.CurrentSelectedDeck;
            var activeDeck = DeckList.Instance.ActiveDeckVersion;

            // if neither source is in a valid state, quit
            if (selectedDeck == null && activeDeck == null)
            {
                throw new Exception("Used deck could not be detected");
            }

            // Convert the deck to dbf_id format
            try
            {
                // if a deck was recorded
                if (selectedDeck != null)
                {
                    selectedDeck.Cards.ForEach(card =>
                    {
                        result.CardDbfIds.Add(HearthDb.Cards.All[card.Id].DbfId, card.Count);
                    });
                }
                else
                {
                    // try to fallback to active deck if selected deck was not detected by HearthMirror
                    foreach (var card in activeDeck.Cards)
                    {
                        result.CardDbfIds.Add(HearthDb.Cards.All[card.Id].DbfId, card.Count);
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error(exc);
                throw new Exception("Exception while parsing the currently played deck");
            }

            // Extract the hero
            try
            {
                // Extract hero from mirror
                if (selectedDeck != null)
                {
                    result.HeroDbfId = HearthDb.Cards.All[selectedDeck.Hero].DbfId;
                }
                else
                {
                    result.HeroDbfId = MapClassNameToHero(activeDeck.Class);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Could not parse the used hero");
            }

            // Extract deck name
            try
            {
                // Extract hero from mirror
                if (selectedDeck != null)
                {
                    result.Name = selectedDeck.Name;
                }
                else
                {
                    result.Name = activeDeck.Name;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Could not parse the name of the deck");
            }

            try
            {
                if (selectedDeck != null)
                {
                    result.Format = selectedDeck.IsWild ? HearthDb.Enums.FormatType.FT_WILD : HearthDb.Enums.FormatType.FT_STANDARD;
                }
                else
                {
                    result.Format = activeDeck.IsWildDeck ? HearthDb.Enums.FormatType.FT_WILD : HearthDb.Enums.FormatType.FT_STANDARD;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Could not parse deck format");
            }

            return result;
        }
    }
}
