using Hearthstone_Deck_Tracker.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin
{
    public class Config
    {
        public static string ConfigFilePath => Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.DataDir, "dh_config.json");

        private static readonly JsonSerializer<Config> Serializer = new JsonSerializer<Config>(ConfigFilePath, false);
        private static readonly Lazy<Config> Data = new Lazy<Config>(Serializer.Load);

        public static Config Instance => Data.Value;

        public string LastDeckcodeUploaded { get; set; }

        public static bool Save() => Serializer.Save(Data.Value);
    }
}
