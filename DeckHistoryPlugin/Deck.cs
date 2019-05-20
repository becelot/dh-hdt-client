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
using MirrorDeck = HearthMirror.Objects.Deck;
using TrackerDeck = Hearthstone_Deck_Tracker.Hearthstone.Deck;

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
        /// Import the deck from the tracked decklist
        /// </summary>
        /// <param name="selectedDeck">The deck that is currently selected</param>
        /// <returns>An instance of a deck that can be serialized</returns>
        private static Deck FromTracker(TrackerDeck selectedDeck)
        {
            Deck result = new Deck();

            // detect if an instance was provided
            if (selectedDeck == null)
            {
                throw new ArgumentException("No deck was provided");
            }

            try
            {
                // convert cards in selected deck to (dbfid, count) format
                foreach (var card in selectedDeck.Cards)
                {
                    result.CardDbfIds.Add(HearthDb.Cards.All[card.Id].DbfId, card.Count);
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
                result.HeroDbfId = MapClassNameToHero(selectedDeck.Class);
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
                result.Name = selectedDeck.Name;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Could not parse the name of the deck");
            }

            try
            {
                result.Format = selectedDeck.IsWildDeck ? HearthDb.Enums.FormatType.FT_WILD : HearthDb.Enums.FormatType.FT_STANDARD;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Could not parse deck format");
            }

            return result;
        }

        /// <summary>
        /// Import the deck from a HearthMirror deck
        /// </summary>
        /// <param name="selectedDeck">The selected deck detected by HearthMirror</param>
        /// <returns>An instance of a deck that can be serialized</returns>
        private static Deck FromMirror(MirrorDeck selectedDeck)
        {
            Deck result = new Deck();

            // detect if an instance was provided
            if (selectedDeck == null)
            {
                throw new ArgumentException("No deck was provided");
            }

            try
            {
                // convert cards in selected deck to (dbfid, count) format
                selectedDeck.Cards.ForEach(card =>
                {
                    result.CardDbfIds.Add(HearthDb.Cards.All[card.Id].DbfId, card.Count);
                });
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
                result.HeroDbfId = HearthDb.Cards.All[selectedDeck.Hero].DbfId;
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
                result.Name = selectedDeck.Name;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Could not parse the name of the deck");
            }

            try
            {
                result.Format = selectedDeck.IsWild ? HearthDb.Enums.FormatType.FT_WILD : HearthDb.Enums.FormatType.FT_STANDARD;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception("Could not parse deck format");
            }

            return result;
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

            // Try to import from mirror first, as it provides more information
            try { return FromMirror(selectedDeck); } catch (Exception) { }

            // Fallback to tracker, if the mirror instance was not parseable
            try { return FromTracker(activeDeck); } catch (Exception) { }

            // if neither source is in a valid state, quit
            throw new Exception("Used deck could not be detected");
        }
    }
}
