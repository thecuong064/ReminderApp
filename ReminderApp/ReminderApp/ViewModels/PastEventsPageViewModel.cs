using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReminderApp.Enums;
using ReminderApp.Models;
using ReminderApp.Services.SQLiteService;
using ReminderApp.Utilities;
using ReminderApp.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ReminderApp.ViewModels
{
    public class PastEventsPageViewModel : BaseViewModel
    {
        #region Constructor

        public PastEventsPageViewModel(INavigationService navigationService = null, 
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null)
            : base(navigationService, dialogService, sqliteService)
        {
            Instance = this;
            OnEventTappedCommand = new DelegateCommand(OnEventTappedExecute);
            RefreshListCommand = new DelegateCommand(RefreshListExecute);
        }

        #endregion

        #region Properties

        public static PastEventsPageViewModel Instance { get; private set; }

        public ObservableCollection<Event> AllEventsList;

        private ObservableCollection<Event> _showEventsList;
        public ObservableCollection<Event> ShowedEventsList
        {
            get => _showEventsList;
            set => SetProperty(ref _showEventsList, value);
        }

        private Event _selectedEvent;
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        #endregion

        #region OnFirstTimeAppear

        public async override void OnFirstTimeAppear()
        {
            base.OnFirstTimeAppear();

            await GetPastEventsList();
            //handle when clear data in settings page
            MessagingCenter.Subscribe<SettingsPageViewModel>(this, MessagingCenterKey.DataCleared.ToString(), async (sender) =>
            {
                await GetPastEventsList();
            });

        }

        #endregion

        #region OnNavigatedBackTo

        public async override void OnNavigatedBackTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.ContainsKey(ParamKey.EventsListUpdated.ToString()))
                {
                    await GetPastEventsList(true);
                }
            }
        }

        #endregion

        #region RefreshListCommand

        public ICommand RefreshListCommand { get; set; }

        public async void RefreshListExecute()
        {
            await GetPastEventsList(true);
        }

        private async Task GetPastEventsList(bool isLoadingDelay = false)
        {
            IsBusy = true;

            if (isLoadingDelay)
            {
                await Task.Delay(1000);
            }

            await CheckNotBusy(async () =>
            {
                AllEventsList = new ObservableCollection<Event>(SqLiteService.GetList<Event>());
                ShowedEventsList = new ObservableCollection<Event>();

                // past events
                foreach (var e in AllEventsList)
                {
                    if (DateTime.Compare(e.Date.AddMinutes(e.Time.TotalMinutes), DateTime.Now) <= 0)
                    {
                        ShowedEventsList.Add(e);
                    }
                }
            });

            IsBusy = false;
        }

        #endregion

        #region OnEventTappedCommand

        public ICommand OnEventTappedCommand { get; set; }

        async void OnEventTappedExecute()
        {
            await CheckNotBusy(async () =>
            {
                NavigationParameters param = new NavigationParameters
                {
                    {ParamKey.SelectedEvent.ToString(), SelectedEvent.Id},
                };

                await Navigation.NavigateAsync(PageManager.EventDetailPage, parameters: param);
            });
        }

        #endregion

        #region DeleteEvent

        public async void DeleteEvent(Event e)
        {
            var accept = await DialogService.DisplayAlertAsync("Confirmation", "You wanna delete this event, right?", "Of course", "No");
            if (accept)
            {
                SqLiteService.Delete<Event>(e);
                RefreshListExecute();
            }
        }

        #endregion
    }
}
