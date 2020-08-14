using ReminderApp.Models;
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

        #region OnAppearing

        protected override void OnAppearing()
        {
            base.OnAppearing();
            addButton.IsVisible = false;
            clearButton.IsVisible = false;
            menuButton.Text = ":::";
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

        private void menuButton_Clicked(object sender, System.EventArgs e)
        {
            addButton.IsVisible = !addButton.IsVisible;
            clearButton.IsVisible = !clearButton.IsVisible;
            if (menuButton.Text.Equals("x"))
            {
                menuButton.Text = ":::";
            }
            else
            {
                menuButton.Text = "x";
            }
        }
    }
}
