using ReminderApp.Models;
using ReminderApp.ViewModels;
using ReminderApp.Views.Base;
using Xamarin.Forms;

namespace ReminderApp.Views
{
    public partial class EventsPage : BasePage
    {
        #region Constructor

        public EventsPage()
        {
            InitializeComponent();
        }

        #endregion

        private void MenuItem_Clicked(object sender, System.EventArgs e)
        {
            var item = ((MenuItem)sender).CommandParameter;
            EventsPageViewModel.Instance.DeleteEvent((Event)item);
        }
    }
}
