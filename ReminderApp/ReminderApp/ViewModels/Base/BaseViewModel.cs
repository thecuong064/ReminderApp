using Plugin.Connectivity;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReminderApp.Services.SQLiteService;
using ReminderApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ReminderApp.ViewModels.Base
{
    public class BaseViewModel : BindableBase, INavigationAware
    {
        #region Properties

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }
        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        private string _headerTitle;
        public string HeaderTitle
        {
            get => _headerTitle;
            set => SetProperty(ref _headerTitle, value);
        }

        private bool _isLiveStreamAvailable;
        public bool IsConferenceAvailable
        {
            get => _isLiveStreamAvailable;
            set => SetProperty(ref _isLiveStreamAvailable, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public static bool IsProfilePageShowed = false;

        public INavigationService Navigation { get; private set; }
        public IPageDialogService DialogService { get; private set; }
        public ISqLiteService SqLiteService { get; private set; }

        #endregion

        #region Constructors

        public BaseViewModel(INavigationService navigationService = null,
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null)
        {
            if (navigationService != null) Navigation = navigationService;
            if (dialogService != null) DialogService = dialogService;
            if (sqliteService != null) SqLiteService = sqliteService;
            BackCommand = new DelegateCommand(async () => await BackExecute());

            //Set default value for IsRefreshing
            IsRefreshing = false;

            //Use andicator for iOS and refresh loading for Android
            //if (Device.RuntimePlatform == Device.Android)
            //{
            //    RefreshColor = Color.Black;
            //    ActivityIndicatorColor = Color.Transparent;
            //}
            //else
            //{
            //    RefreshColor = Color.Transparent;
            //}
        }

        #endregion

        #region Navigate

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigated from");
#endif
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigating to");
#endif
        }

        public async virtual void OnNavigatedTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigated to");
#endif
            //Force portrait
            //Xamarin.Forms.DependencyService.Get<INativeUtilities>().ForcePortrait();
            if (parameters != null)
            {
                var navMode = parameters.GetNavigationMode();
                switch (navMode)
                {
                    case NavigationMode.New: OnNavigatedNewTo(parameters); break;
                    case NavigationMode.Back: OnNavigatedBackTo(parameters); break;
                }
            }

        }

        public virtual void OnNavigatedNewTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigate new to");
#endif
        }

        public virtual void OnNavigatedBackTo(INavigationParameters parameters)
        {
#if DEBUG
            Debug.WriteLine("Navigate back to");
#endif
        }

        #endregion

        #region OnAppear / Disappear

        public virtual void OnAppear()
        {

        }

        public virtual void OnFirstTimeAppear()
        {

        }

        public virtual void OnDisappear()
        {

        }

        #endregion

        #region BackCommand

        public ICommand BackCommand { get; }

        protected virtual async Task BackExecute()
        {
            await CheckNotBusy(async () =>
            {
                await Navigation.GoBackAsync(animated: false);
            });
        }

        #endregion

        #region BackButtonPress

        /// <summary>
        /// //false is default value when system call back press
        /// </summary>
        /// <returns></returns>
        public virtual bool OnBackButtonPressed()
        {
            //false is default value when system call back press
            //return false;
            BackExecute();

            return true;
        }

        /// <summary>
        /// called when page need override soft back button
        /// </summary>
        public virtual void OnSoftBackButtonPressed() { }

        #endregion

        #region CheckBusy

        protected async Task CheckNotBusy(Func<Task> function)
        {
            if (App.IsNotBusy)
            {
                App.IsNotBusy = false;
                try
                {
                    await function();
                }
                catch (Exception e)
                {
#if DEBUG
                    Debug.WriteLine(e);
#endif
                }
                finally
                {
                    App.IsNotBusy = true;
                }
            }
        }

        #endregion

        #region CheckInternetConnection

        public bool IsInternetConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        #endregion

        #region Check Permission

        protected async void CheckPermission(Action action)
        {
            await CheckNotBusy(async () =>
            {
                //var camera = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                //var storage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                //if (storage == PermissionStatus.Granted)
                //{
                //    action();
                //}
                //else
                //{
                //    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                //}
            });
        }

        #endregion
    }
}
