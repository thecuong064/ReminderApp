using SQLite;
using System;
using System.Collections.ObjectModel;

namespace ReminderApp.Models
{
    public class Event
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsNotified { get; set; }
    }

    public class GroupEvent : ObservableCollection<Event>
    {
        public string Title { get; set; }
        public string ShortName { get; set; } 
    }
}
