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
    public class EventsPageViewModel : BaseViewModel
    {
        #region Constructor

        public EventsPageViewModel(INavigationService navigationService = null,
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null)
            : base(navigationService, dialogService, sqliteService)
        {
            Instance = this;
            AddEventCommand = new DelegateCommand(AddEventExecute);
            OnEventTappedCommand = new DelegateCommand(OnEventTappedExecute);
            RefreshListCommand = new DelegateCommand(RefreshListExecute);
            Init();
        }

        #endregion

        #region Properties

        public static EventsPageViewModel Instance { get; private set; }

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

        private ObservableCollection<string> _eventTypesList;
        public ObservableCollection<string> EventTypesList
        {
            get => _eventTypesList;
            set => SetProperty(ref _eventTypesList, value);
        }

        private int _selectedEventTypeIndex;
        public int SelectedEventTypeIndex
        {
            get => _selectedEventTypeIndex;
            set
            {
                if (value >= 0)
                {
                    SetProperty(ref _selectedEventTypeIndex, value);
                    GetTheList();
                }
            }
        }

        #endregion

        #region OnFirstTimeAppear

        public override void OnFirstTimeAppear()
        {
            base.OnFirstTimeAppear();

            //handle when clear data in settings page
            MessagingCenter.Subscribe<SettingsPageViewModel>(this, MessagingCenterKey.DataCleared.ToString(), async (sender) =>
            {
                await GetTheList();
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
                    await GetTheList(true);
                }
            }
        }

        #endregion

        #region Init

        private void Init()
        {
            //set event types list
            EventTypesList = new ObservableCollection<string>()
            {
                "All",
                "Upcoming",
            };

            //set SortType default to get new ShowList (not have to set ShowList again)
            SelectedEventTypeIndex = 0;
        }

        #endregion

        #region RefreshListCommand

        public ICommand RefreshListCommand { get; set; }

        public async void RefreshListExecute()
        {
            await GetTheList(true);
        }

        private async Task GetTheList(bool isLoadingDelay = false)
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
                if (SelectedEventTypeIndex == 1)
                {
                    // upcoming events
                    foreach (var e in AllEventsList)
                    {
                        if (DateTime.Compare(e.Date.AddMinutes(e.Time.TotalMinutes), DateTime.Now) > 0)
                        {
                            ShowedEventsList.Add(e);
                        }
                    }
                }
                else
                {
                    // all events
                    ShowedEventsList = new ObservableCollection<Event>(AllEventsList);
                }
            });

            IsBusy = false;
        }

        #endregion

        #region AddEventCommand

        public ICommand AddEventCommand { get; set; }

        async void AddEventExecute()
        {
            await CheckNotBusy(async () =>
            {
                var newId = AllEventsList.Count;

                if (AllEventsList.Count > 0)
                {
                    newId = AllEventsList[AllEventsList.Count - 1].Id + 1;
                }

                NavigationParameters param = new NavigationParameters
                {
                    {ParamKey.SelectedEventId.ToString(), newId }
                };

                await Navigation.NavigateAsync(PageManager.EventDetailPage, parameters : param, animated: false);
            });
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
                    {ParamKey.SelectedEventId.ToString(), SelectedEvent.Id},
                };

                await Navigation.NavigateAsync(PageManager.EventDetailPage, parameters : param);
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
