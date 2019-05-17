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
        internal void OnGameEnd()
        {
            Log.Info("Game ended");
        }
    }
}
