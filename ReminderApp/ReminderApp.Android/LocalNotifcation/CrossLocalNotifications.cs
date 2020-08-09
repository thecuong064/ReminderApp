using ReminderApp.Droid.LocalNotification;
using ReminderApp.Interfaces;
using System;
using System.Threading;
using Xamarin.Forms;

namespace ReminderApp.Droid.LocalNotification
{
    /// <summary>
    /// Access Cross Local Notifictions
    /// </summary>
    public static class CrossLocalNotifications
    {
        private static Lazy<ILocalNotification> _impl = new Lazy<ILocalNotification>(CreateLocalNotificationsImplementation, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the current platform specific ILocalNotifications implementation.
        /// </summary>
        public static ILocalNotification Current
        {
            get
            {
                var val = _impl.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static ILocalNotification CreateLocalNotificationsImplementation()
        {
            return new LocalNotificationImplementation();
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException();
        }
    }
}
