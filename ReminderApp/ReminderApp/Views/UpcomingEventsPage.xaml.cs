﻿using ReminderApp.Models;
using ReminderApp.ViewModels;
using ReminderApp.Views.Base;
using Xamarin.Forms;

namespace ReminderApp.Views
{
    public partial class UpcomingEventsPage : BasePage
    {
        #region Constructor

        public UpcomingEventsPage()
        {
            InitializeComponent();
        }

        #endregion

        private void RemoveMenuItem_Clicked(object sender, System.EventArgs e)
        {
            var item = ((MenuItem)sender).CommandParameter;
            UpcomingEventsPageViewModel.Instance.DeleteEvent((Event)item);
        }

        private void DuplicateMenuItem_Clicked(object sender, System.EventArgs e)
        {
            var item = ((MenuItem)sender).CommandParameter;
            UpcomingEventsPageViewModel.Instance.DuplicateEvent((Event)item);
        }
    }
}