using ReminderApp.ViewModels;
using ReminderApp.ViewModels.Base;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ReminderApp.Views.Base
{
    public class BasePage : ContentPage
    {
        public BasePage()
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.White;
        }

        #region Properties

        private bool _isAppeared;
        protected BaseViewModel ViewModel;

        #endregion

        protected override void OnBindingContextChanged()
        {
            if (BindingContext != null)
                ViewModel = (BaseViewModel)BindingContext;
        }

        protected override void OnAppearing()
        {
            try
            {
                if (ViewModel == null && BindingContext != null)
                    ViewModel = (BaseViewModel)BindingContext;

                if (!_isAppeared)
                    ViewModel?.OnFirstTimeAppear();

                _isAppeared = true;
                ViewModel?.OnAppear();

                //for iOS only
                On<iOS>().SetUseSafeArea(true);
                var safeInsets = On<iOS>().SafeAreaInsets();
                safeInsets.Left = 20;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

        protected override void OnDisappearing()
        {
            ViewModel?.OnDisappear();
        }

        #region BackButtonPress

        protected override bool OnBackButtonPressed()
        {
            var bindingContext = BindingContext as BaseViewModel;
            var result = bindingContext?.OnBackButtonPressed() ?? base.OnBackButtonPressed();
            return result;
        }


        public void OnSoftBackButtonPressed()
        {
            var bindingContext = BindingContext as BaseViewModel;
            bindingContext?.OnSoftBackButtonPressed();
        }

        public bool NeedOverrideSoftBackButton { get; set; } = false;

        #endregion
    }
}
