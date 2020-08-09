using SQLite;
using System;

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
}
