using Prism.Navigation;
using Prism.Services;
using ReminderApp.Services.SQLiteService;
using ReminderApp.ViewModels.Base;

namespace ReminderApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Constructor

        public MainPageViewModel(INavigationService navigationService = null,
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null) : base(navigationService, dialogService, sqliteService)
        {

        }

        #endregion
    }
}
