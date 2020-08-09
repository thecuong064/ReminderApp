using System;
namespace ReminderApp.Interfaces
{
    public interface INotification
    {
        void CreateNotification(String title, String message);
    }
}
