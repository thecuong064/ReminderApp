using System;
using System.Globalization;
using Xamarin.Forms;

namespace ReminderApp.Converters
{
    public class ImageURLConverter : IValueConverter
    {
        public string DefaultThumbnail { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //try
            //{
            //    var imageUrl = ((FileContent)value).Thumbnail;

            //    if (!string.IsNullOrEmpty(imageUrl))
            //        return imageUrl;
            //}
            //catch {
            //}

            return DefaultThumbnail;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DefaultThumbnail;
        }
    }
}
