using Hearthstone_Deck_Tracker.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeckHistoryPlugin
{
    public class DeckHistoryPlugin : IPlugin
    {
        public string Name => "Deck History";

        public string Description => "A plugin that tracks and uploads your played decks history. Connects to the Twitch extension to display recently played decks to your viewer!";

        public string ButtonText => "Configure";

        public string Author => "Becelot";

        public Version Version => new Version(0, 0, 1);

        public MenuItem MenuItem
        {
            get
            {
                MenuItem item = new MenuItem();
                item.Header = "Deck History";
                return item;
            }
        }


        public void OnButtonPress()
        {

        }

        public void OnLoad()
        {

        }

        public void OnUnload()
        {

        }

        public void OnUpdate()
        {
        }
    }
}
