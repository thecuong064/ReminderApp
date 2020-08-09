using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReminderApp.Enums;
using ReminderApp.LocalNotification;
using ReminderApp.Models;
using ReminderApp.Services.SQLiteService;
using ReminderApp.ViewModels.Base;
using System;
using System.Windows.Input;

namespace ReminderApp.ViewModels
{
    public class EventDetailPageViewModel : BaseViewModel
    {
        #region Constructor

        public EventDetailPageViewModel(INavigationService navigationService = null,
            IPageDialogService dialogService = null,
            ISqLiteService sqliteService = null)
            : base(navigationService, dialogService, sqliteService)
        {
            SaveCommand = new DelegateCommand(SaveExecute);
        }

        #endregion

        #region Properties

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }


        private string _body;
        public string Body
        {
            get => _body;
            set => SetProperty(ref _body, value);
        }

        private DateTime _minimumDate;
        public DateTime MinimumDate
        {
            get => _minimumDate;
            set => SetProperty(ref _minimumDate, value);
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        private bool _isNotified;
        public bool IsNotified
        {
            get => _isNotified;
            set
            {
                SetProperty(ref _isNotified, value);
            }
        }

        private Event _currentEvent;
        public Event CurrentEvent
        {
            get => _currentEvent;
            set => SetProperty(ref _currentEvent, value);
        }

        #endregion

        #region OnNavigatedNewTo

        public override void OnNavigatedNewTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.ContainsKey(ParamKey.SelectedEventId.ToString()))
                {
                    var id = (int)parameters[ParamKey.SelectedEventId.ToString()];
                    CurrentEvent = SqLiteService.Get<Event>(s => s.Id == id);
                    if (CurrentEvent == null)
                    {
                        InitNewEvent();
                        CurrentEvent.Id = id;
                    }
                    else
                    {
                        Title = CurrentEvent.Title;
                        Body = CurrentEvent.Body;
                        Date = CurrentEvent.Date;
                        Time = CurrentEvent.Time;
                        IsNotified = CurrentEvent.IsNotified;
                    }
                }
                else
                {
                    InitNewEvent();
                }
            }
            else
            {
                InitNewEvent();
            }
        }

        #endregion

        #region InitFields

        private void InitNewEvent()
        {
            CurrentEvent = new Event();

            InitNewDateTime();
            IsNotified = true;
        }

        #endregion

        #region InitNewDateTime

        private void InitNewDateTime()
        {
            Date = DateTime.Now.Date;
            MinimumDate = DateTime.Now.Date;
            Time = TimeSpan.FromMinutes(DateTime.Now.Minute + DateTime.Now.Hour * 60 + 5);
        }

        #endregion

        #region SaveCommand

        public ICommand SaveCommand { get; set; }

        public async void SaveExecute()
        {
            await CheckNotBusy( async () =>
            {
                CurrentEvent.Title = Title.Trim();
                CurrentEvent.Body = Body.Trim();
                CurrentEvent.Date = Date;
                CurrentEvent.Time = Time;
                CurrentEvent.IsNotified = IsNotified;

                SqLiteService.Insert(CurrentEvent);
                
                if (IsNotified)
                {
                    var dateTime = Date.AddMinutes(Time.TotalMinutes);
                    CrossLocalNotifications.Current.Cancel(CurrentEvent.Id);
                    CrossLocalNotifications.Current.Show(Title, Body, CurrentEvent.Id, dateTime);
                }
                else
                {
                    CrossLocalNotifications.Current.Cancel(CurrentEvent.Id);
                }
            });

            NavigationParameters param = new NavigationParameters
            {
                {ParamKey.EventsListUpdated.ToString(), ParamKey.EventsListUpdated.ToString()},
            };

            await Navigation.GoBackAsync(parameters: param);
        }

        #endregion
    }
}
