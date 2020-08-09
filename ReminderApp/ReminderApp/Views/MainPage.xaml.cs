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
            SelectedItem = Children[0];
            this.TabReselected += TabReselectedExecute;
        }

        private void InitBottomTabbedPage()
        {
            Children.Add(new NavigationPage(new EventsPage())
            {
                Title = "Events",
                IconImageSource = "ic_events_black_72dp.png",
            });

            Children.Add(new NavigationPage(new CalendarPage())
            {
                Title = "Calendar",
                IconImageSource = "ic_calendar_black_72dp.png",
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
                if (title == "HOME")
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
