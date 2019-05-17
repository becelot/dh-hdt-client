using DeckHistoryPlugin.FlyoutControls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeckHistoryPlugin
{
    public class DeckHistoryPlugin : IPlugin
    {
        private Flyout optionsFlyout = null;

        private GameMonitor gameMonitor = new GameMonitor();

        private ICommand ShowConfigurationDialog => new Command(() => optionsFlyout.IsOpen = true);

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
                item.Command = ShowConfigurationDialog;
                return item;
            }
        }


        public void OnButtonPress()
        {
            ShowConfigurationDialog.Execute(this);
        }

        public void OnLoad()
        {
            var mainWindow = (MetroWindow)Hearthstone_Deck_Tracker.API.Core.MainWindow;

            optionsFlyout = new Flyout()
            {
                Content = new OptionsFlyout(),
                Header = "Deck History Plugin",
                Width = 600
            };

            mainWindow.Flyouts.Items.Add(optionsFlyout);

            GameEvents.OnGameEnd.Add(this.gameMonitor.OnGameEnd);
        }

        public void OnUnload()
        {
            var mainWindow = (MetroWindow)Hearthstone_Deck_Tracker.API.Core.MainWindow;

            mainWindow.Flyouts.Items.Remove(optionsFlyout);
            optionsFlyout = null;
        }

        public void OnUpdate()
        {
        }
    }
}
