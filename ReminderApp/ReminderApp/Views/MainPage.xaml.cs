using ReminderApp.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace ReminderApp.Views
{
    public partial class MainPage : CustomTabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitBottomTabbedPage();
            SelectedItem = Children[1];
            this.TabReselected += TabReselectedExecute;
        }

        private void InitBottomTabbedPage()
        {
            Children.Add(new NavigationPage(new PastEventsPage())
            {
                Title = "Past",
                IconImageSource = "ic_past_event_black_72dp.png",
            });

            Children.Add(new NavigationPage(new UpcomingEventsPage())
            {
                Title = "Upcoming",
                IconImageSource = "ic_upcoming_event_black_72dp.png",
            });

            Children.Add(new NavigationPage(new SettingsPage())
            {
                Title = "Settings",
                IconImageSource = "ic_settings_black_72dp.png",
            });
        }

        private async void TabReselectedExecute(object sender, EventArgs e)
        {
            var title = (string)sender;
            var tab = Children.FirstOrDefault(page => page.Title == title);

            if (tab != null)
            {
                if (title == "Upcoming")
                {
                    try
                    {
                        await tab.Navigation.PopToRootAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }
    }
}
