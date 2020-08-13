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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ReminderApp.ViewModels
{
    public class UpcomingEventsPageViewModel : BaseViewModel
    {
        #region Constructor

        public UpcomingEventsPageViewModel(INavigationService navigationService = null,
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null)
            : base(navigationService, dialogService, sqliteService)
        {
            Instance = this;
            AddEventCommand = new DelegateCommand(AddEventExecute);
            OnEventTappedCommand = new DelegateCommand(OnEventTappedExecute);
            RefreshListCommand = new DelegateCommand(RefreshListExecute);
        }

        #endregion

        #region Properties

        public static UpcomingEventsPageViewModel Instance { get; private set; }

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

        private ObservableCollection<GroupEvent> _groupEventList;
        public ObservableCollection<GroupEvent> GroupEventList
        {
            get => _groupEventList;
            set => SetProperty(ref _groupEventList, value);
        }

        private bool _isThereNoEvent;
        public bool IsThereNoEvent
        {
            get => _isThereNoEvent;
            set => SetProperty(ref _isThereNoEvent, value);
        }

        #endregion

        #region OnFirstTimeAppear

        public async override void OnFirstTimeAppear()
        {
            base.OnFirstTimeAppear();

            await GetUpcomingEventsList();
            //handle when clear data in settings page
            MessagingCenter.Subscribe<SettingsPageViewModel>(this, MessagingCenterKey.DataCleared.ToString(), async (sender) =>
            {
                await GetUpcomingEventsList();
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
                    await GetUpcomingEventsList(true);
                }
            }
        }

        #endregion

        #region RefreshListCommand

        public ICommand RefreshListCommand { get; set; }

        public async void RefreshListExecute()
        {
            await GetUpcomingEventsList(true);
        }

        private async Task GetUpcomingEventsList(bool isLoadingDelay = false)
        {
            IsBusy = true;

            IsThereNoEvent = false;

            if (isLoadingDelay)
            {
                await Task.Delay(1000);
            }

            await CheckNotBusy(async () =>
            {

                AllEventsList = new ObservableCollection<Event>(SqLiteService.GetList<Event>());
                ShowedEventsList = new ObservableCollection<Event>();

                GroupEventList = new ObservableCollection<GroupEvent>();
                var todayGroup = new GroupEvent 
                { 
                    Title = "Today", 
                    ShortName = "T" 
                };
                var elseGroup = new GroupEvent
                {
                    Title = "Following days",
                    ShortName = "F"
                };

                foreach (var e in AllEventsList)
                {
                    //if (DateTime.Compare(e.Date.AddMinutes(e.Time.TotalMinutes), DateTime.Now) > 0)
                    //{
                    //    ShowedEventsList.Add(e);
                    //if (DateTime.Compare(e.Date.AddMinutes(e.Time.TotalMinutes), DateTime.Now) > 0)
                    //}
                    if (DateTime.Compare(e.Date.AddMinutes(e.Time.TotalMinutes), DateTime.Now) > 0)
                    {

                        if (DateTime.Compare(e.Date, DateTime.Now.Date) == 0)
                        {
                            todayGroup.Add(e);
                        }
                        else
                        {
                            elseGroup.Add(e);
                        }
                    }
                }

                if (todayGroup.Count > 0)
                {
                    GroupEventList.Add(todayGroup);
                }
                if (elseGroup.Count > 0)
                {
                    GroupEventList.Add(elseGroup);
                }
                
                IsThereNoEvent = GroupEventList.Count <= 0;

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

                var newEvent = new Event
                {
                    Id = newId,
                    IsNotified = true,
                };

                NavigationParameters param = new NavigationParameters
                {
                    {ParamKey.NewEvent.ToString(), newEvent }
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
                    {ParamKey.SelectedEvent.ToString(), SelectedEvent},
                };

                await Navigation.NavigateAsync(PageManager.EventDetailPage, parameters : param);
            });

            SelectedEvent = null;
        }

        #endregion

        #region DeleteEvent

        public async void DeleteEvent(Event selectedEvent)
        {
            var accept = await DialogService.DisplayAlertAsync("Confirmation", "You wanna delete this event, right?", "Of course", "No");
            if (accept)
            {
                SqLiteService.Delete<Event>(selectedEvent);
                RefreshListExecute();
            }
        }

        #endregion

        public async void DuplicateEvent(Event selectedEvent)
        {
            await CheckNotBusy(async () =>
            {
                var newId = AllEventsList.Count;

                if (AllEventsList.Count > 0)
                {
                    newId = AllEventsList[AllEventsList.Count - 1].Id + 1;
                }

                var newEvent = selectedEvent;
                newEvent.Id = newId;

                NavigationParameters param = new NavigationParameters
                {
                    {ParamKey.DuplicatedEvent .ToString(), newEvent},
                };

                await Navigation.NavigateAsync(PageManager.EventDetailPage, parameters: param);
            });
        }
    }
}
