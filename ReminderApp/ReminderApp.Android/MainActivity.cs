using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Prism;
using Prism.Ioc;
using ReminderApp.Droid.Controls;
using ReminderApp.Droid.Services.SQLiteService;
using ReminderApp.Services.SQLiteService;

namespace ReminderApp.Droid
{
    [Activity(Label = "Reminder", 
        Icon = "@mipmap/ic_launcher",
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            RequestedOrientation = ScreenOrientation.Portrait;
            base.OnCreate(savedInstanceState);
            Init(savedInstanceState);

            //this.Window.AddFlags(WindowManagerFlags.Fullscreen); //this line is show status bar on Android.

            //this.Window.ClearFlags(WindowManagerFlags.Fullscreen); //this line is hide status bar on Android.

            LoadApplication(new App(new AndroidInitializer()));
        }

        #region Init

        public void Init(Bundle bundle)
        {
            Xamarin.Essentials.Platform.Init(this, bundle);
            Xamarin.Forms.Forms.Init(this, bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);   
            BottomTabBarConfig();
        }

        #endregion

        #region BottomTabBarConfig

        private void BottomTabBarConfig()
        {
            var stateList = new Android.Content.Res.ColorStateList(
                new int[][] {
                        new int[] { Android.Resource.Attribute.StateChecked
                    },
                        new int[] { Android.Resource.Attribute.StateEnabled
                    }
                },
                new int[] {
                    ContextCompat.GetColor(this.BaseContext, Resource.Color.selectedTabColor), // Selected Color
                    ContextCompat.GetColor(this.BaseContext, Resource.Color.unselectedTabColor) // Unselected Color
                });

            CustomTabbedPageRenderer.BackgroundColor = new Color(ContextCompat.GetColor(this.BaseContext, Resource.Color.bottomTabBackgroundColor));
            //BottomTabbedRenderer.Typeface = Typeface.CreateFromAsset(Assets, "your-font-here.ttf");
            CustomTabbedPageRenderer.FontSize = 10;
            CustomTabbedPageRenderer.IconSize = 25;
            CustomTabbedPageRenderer.ItemTextColor = stateList;
            CustomTabbedPageRenderer.ItemIconTintList = stateList;
            CustomTabbedPageRenderer.ItemSpacing = 5;
            CustomTabbedPageRenderer.ItemPadding = new Xamarin.Forms.Thickness(4);
            CustomTabbedPageRenderer.BottomBarHeight = 55;
            CustomTabbedPageRenderer.ItemAlign = ItemAlignFlags.Center;
        }

        #endregion

        #region OnBackPressed

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        #endregion

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IDatabaseConnection>(new DatabaseConnection());
        }
    }
}

