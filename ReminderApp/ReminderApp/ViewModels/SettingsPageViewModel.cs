using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReminderApp.Enums;
using ReminderApp.Models;
using ReminderApp.Services.SQLiteService;
using ReminderApp.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ReminderApp.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        #region Constructor

        public SettingsPageViewModel(INavigationService navigationService = null,
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null)
            : base(navigationService, dialogService, sqliteService)
        {
            ClearDataCommand = new DelegateCommand(ClearDataExecute);
        }

        #endregion

        #region ClearDataCommand

        public ICommand ClearDataCommand { get; set; }

        async void ClearDataExecute()
        {
            var clearData = await DialogService.DisplayAlertAsync("Data Format", "Are you sure want to delete all the current data?", "Of course", "No");

            if (clearData)
            {
                //show loading screen
                IsBusy = true;

                SqLiteService.DeleteAll<Event>();
                await Task.Delay(400);

                //end loading
                IsBusy = false;

                //message to the subscribers to notice about the change
                MessagingCenter.Send(this, MessagingCenterKey.DataCleared.ToString());

                await App.Current.MainPage.DisplayAlert("Message", "Finish", "OK");
            }
        }
        #endregion
    }
}
