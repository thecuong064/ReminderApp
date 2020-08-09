using System;
using System.Globalization;
using Xamarin.Forms;

namespace ReminderApp.Converters
{
    public class ThumbnailDefaultConverter : IValueConverter
    {
        public string DefaultBackground { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var imageUrl = (string)value;

                if (!string.IsNullOrEmpty(imageUrl))
                    return imageUrl;
            }
            catch { }

            return DefaultBackground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DefaultBackground;
        }
    }
}
