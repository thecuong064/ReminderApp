using ReminderApp.Models;
using ReminderApp.ViewModels;
using ReminderApp.Views.Base;
using Xamarin.Forms;

namespace ReminderApp.Views
{
    public partial class PastEventsPage : BasePage
    {
        #region Constructor

        public PastEventsPage()
        {
            InitializeComponent();
        }

        #endregion

        #region OnAppearing

        protected override void OnAppearing()
        {
            base.OnAppearing();

            clearButton.IsVisible = false;
            menuButton.Text = ":::";
        }

        #endregion

        private void RemoveMenuItem_Clicked(object sender, System.EventArgs e)
        {
            var item = ((MenuItem)sender).CommandParameter;
            PastEventsPageViewModel.Instance.DeleteEvent((Event)item);
        }

        private void menuButton_Clicked(object sender, System.EventArgs e)
        {
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
