using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using ReminderApp.Enums;
using ReminderApp.Utilities;
using System;
using System.Threading.Tasks;

namespace ReminderApp.Popups
{
    public partial class ConfirmationPopup : PopupPage
    {
        #region Constructor

        public ConfirmationPopup()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private static ConfirmationPopup _instance;

        public static bool IsAppearing { get; private set; }

        protected TaskCompletionSource<object> Proccess;

        public static ConfirmationPopup Instance => _instance ?? (_instance = new ConfirmationPopup());

        #endregion

        #region Show

        #region For nofitication

        public async Task ShowNotification(string message = null, string messageIconSource = null)
        {
            await LoadingPopup.Instance.Hide();
            IsAppearing = true;

            //Hide and show correct view
            okButton.IsVisible = true;
            TwoButtonsView.IsVisible = false;

            messageLabel.Text = message;

            if (messageIconSource != null)
            {
                messageIcon.Source = messageIconSource;
                messageIcon.IsVisible = true;
            }
            else
            {
                messageIcon.IsVisible = false;
            }

            Proccess = new TaskCompletionSource<object>();

            await DeviceExtension.BeginInvokeOnMainThreadAsync(async () =>
            {
                await PopupNavigation.Instance.PushAsync(this, animate: false);
            });
            var result = await GetResult();
            await PopupNavigation.Instance.PopAllAsync(animate: false);
            IsAppearing = false;
        }

        #endregion

        #region For confirmation (yes/no)

        public async Task<bool> ShowConfirmation(string message = null, 
            string messageIconSource = null, 
            ConfirmPopupType type = ConfirmPopupType.Normal)
        {
            await LoadingPopup.Instance.Hide();
            IsAppearing = true;

            switch (type)
            {
                case ConfirmPopupType.Normal:
                    {
                        acceptButton.HeightRequest = 150;
                        acceptButton.WidthRequest = 150;
                        declineButton.HeightRequest = 150;
                        declineButton.WidthRequest = 150;
                        break;
                    }
                case ConfirmPopupType.Decline:
                    {
                        acceptButton.HeightRequest = 120;
                        acceptButton.WidthRequest = 120;
                        declineButton.HeightRequest = 150;
                        declineButton.WidthRequest = 150;
                        break;
                    }
                case ConfirmPopupType.Accept:
                    {
                        acceptButton.HeightRequest = 150;
                        acceptButton.WidthRequest = 150;
                        declineButton.HeightRequest = 120;
                        declineButton.WidthRequest = 120;
                        break;
                    }
            }

            //Hide and show correct view
            okButton.IsVisible = false;
            TwoButtonsView.IsVisible = true;

            messageLabel.Text = message;

            if (messageIconSource != null)
            {
                messageIcon.Source = messageIconSource;
                messageIcon.IsVisible = true;
            }
            else
            {
                messageIcon.IsVisible = false;
            }

            Proccess = new TaskCompletionSource<object>();

            await DeviceExtension.BeginInvokeOnMainThreadAsync(async () =>
            {
                await PopupNavigation.Instance.PushAsync(this, animate: false);
            });
            var result = await GetResult();
            await PopupNavigation.Instance.PopAllAsync(animate: false);
            IsAppearing = false;

            return (bool)result;
        }

        #endregion

        #endregion

        #region GetResult

        public Task<object> GetResult()
        {
            return Proccess.Task;
        }

        #endregion

        #region OnBackButtonPressed

        protected override bool OnBackButtonPressed()
        {
            Proccess.SetResult(false);  //cmt this line to lock the hardware back button
            return true;
        }

        #endregion

        #region Button Tapped

        private void acceptButton_Tapped(object sender, EventArgs e)
        {
            Proccess.SetResult(true);
        }

        private void declineButton_Tapped(object sender, EventArgs e)
        {
            Proccess.SetResult(false);
        }

        #endregion

        #region OnBackgroundTapped

        private void OnBackgroundTapped(object sender, EventArgs e)
        {
            Proccess.SetResult(false);
        }

        #endregion
    }
}