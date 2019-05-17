using DeckHistoryPlugin.Api;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeckHistoryPlugin.FlyoutControls
{
    /// <summary>
    /// Interaction logic for OptionsFlyout.xaml
    /// </summary>
    public partial class OptionsFlyout : UserControl, INotifyPropertyChanged
    {
        private bool _showAuthenticationSpinner = false;
        private bool _showLoginHint = false;
        private string _loginHint = "Some login hint";

        public OptionsFlyout()
        {
            InitializeComponent();
            ApiWrapper.LoggedIn += () => UpdateAccount();
            ApiWrapper.LoggedOut += () => UpdateAccount();
        }

        private void UpdateAccount()
        {
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(LoggedInUser));

            ShowLoginHint = false;
        }

        #region ViewModel
        public bool IsAuthenticated => Account.Instance.IsAuthenticated;

        public string LoggedInUser => Account.Instance.Username;

        public bool ShowLoginHint
        {
            get
            {
                return _showLoginHint;
            }
            set
            {
                if (_showLoginHint == value) return;
                _showLoginHint = value;
                OnPropertyChanged();
            }
        }

        public bool IsAuthenticating
        {
            get
            {
                return _showAuthenticationSpinner;
            }
            set
            {
                if (_showAuthenticationSpinner == value) return;
                _showAuthenticationSpinner = value;
                OnPropertyChanged();
            }
        }



        public string LoginHint
        {
            get
            {
                return _loginHint;
            }
            set
            {
                if (_loginHint == value) return;
                _loginHint = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Try to login with the provided credentials
        /// </summary>
        public ICommand LoginCommand => new Command(async () =>
        {
            // verfiy username
            var username = Username.Text;
            if (String.IsNullOrEmpty(username))
            {
                return;
            }

            // verify password
            var password = Password.Password;
            if (String.IsNullOrEmpty(password))
            {
                return;
            }

            IsAuthenticating = true;

            // Try to login with provided credentials
            string result = "";
            try
            {
                result = await ApiWrapper.Login(username, password);
            }
            catch (Exception)
            {
                // A backend error occured
                result = "Could not connect to DeckHistory server.";
            }

            // Show hints
            IsAuthenticating = false;
            LoginHint = result;
            ShowLoginHint = true;
        });

        public ICommand LogoutCommand => new Command(() => ApiWrapper.Logout());
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
