using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api
{
    public class Account : INotifyPropertyChanged
    {
        public static string AccountFilePath => Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.DataDir, "dh_account");

        private static readonly JsonSerializer<Account> Serializer = new JsonSerializer<Account>(AccountFilePath, true);
        private static readonly Lazy<Account> Data = new Lazy<Account>(Serializer.Load);

        public static Account Instance => Data.Value;

        private static Account Load()
        {
            if (!File.Exists(AccountFilePath))
            {
                return new Account();
            }

            return new JsonSerializer<Account>(AccountFilePath, true).Load();
        }

        public static bool Save()
        {
            return Serializer.Save(Data.Value);
        }

        public String Username { get; set; }

        [JsonIgnore]
        public bool IsAuthenticated
        {
            get
            {
                return !String.IsNullOrEmpty(UploadToken);
            }
        }

        private String _uploadToken { get; set; }

        public String UploadToken
        {
            get
            {
                return _uploadToken;
            }
            set
            {
                if (_uploadToken == value) return;
                _uploadToken = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        public String TwitchId { get; set; }

        public bool Logout()
        {
            Username = null;
            UploadToken = null;
            return Account.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
