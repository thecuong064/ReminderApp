using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using Prism.DryIoc;
using ReminderApp.Constants;
using ReminderApp.Utilities;
using ReminderApp.Services.SQLiteService;
using ReminderApp.ViewModels;
using ReminderApp.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ReminderApp
{
    public partial class App : PrismApplication
    {
        #region Properties 

        public new static App Current => Application.Current as App;
        public static double ScreenWidth;
        public static double ScreenHeight;
        public static bool IsNotBusy = true;

        private ISqLiteService _sqLiteService;

        public static string NotiTitle;
        public static string NotiMessage;

        #endregion

        #region OnStart

        protected override void OnStart()
        {
            //AppCenter.Start($"android={SdkKeyConstants.AppCenterAndroid};" +
            //    "uwp={Your UWP App secret here};" +
            //    $"ios={SdkKeyConstants.AppCenteriOS}",
            //    typeof(Analytics), typeof(Crashes));

            base.OnStart();
        }

        #endregion

        #region Constructor

        public App() : this(null) { }

        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        #endregion

        #region OnInitialized

        protected async override void OnInitialized()
        {
            InitApiKey();
            InitDatabase();
            InitializeComponent();

            await NavigationService.NavigateAsync(new Uri($"{UriConstants.NavigationHomeUri}{PageManager.MainPage}"));
        }

        #endregion

        #region Init

        private void InitApiKey()
        {
        }

        #region InitDatabase

        private void InitDatabase()
        {
            var connectionService = DependencyService.Get<IDatabaseConnection>();
            _sqLiteService = new SqLiteService(connectionService);
        }

        #endregion

        #endregion

        #region RegisterTypes

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            #region Interface

            //containerRegistry.Register<IHttpRequest, HttpRequest>();
            containerRegistry.Register<ISqLiteService, SqLiteService>();

            #endregion

            containerRegistry.RegisterForNavigation<NavigationPage>(PageManager.NavigationPage);

            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>(PageManager.MainPage);
            containerRegistry.RegisterForNavigation<UpcomingEventsPage, UpcomingEventsPageViewModel>(PageManager.UpcomingEventsPage);
            containerRegistry.RegisterForNavigation<PastEventsPage, PastEventsPageViewModel>(PageManager.PastEventsPage);
            containerRegistry.RegisterForNavigation<EventDetailPage, EventDetailPageViewModel>(PageManager.EventDetailPage);
            containerRegistry.RegisterForNavigation<ShowEventDetailPage, ShowEventDetailPageViewModel>(PageManager.ShowEventDetailPage);
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>(PageManager.SettingsPage);
        }

        #endregion
    }
}
