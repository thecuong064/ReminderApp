using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace ReminderApp.Converters
{
    public class BoolImageURLConverter : IValueConverter
    {
        public string TrueButtonImageURL { get; set; }
        public string FalseButtonImageURL { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueButtonImageURL : FalseButtonImageURL;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TrueButtonImageURL;
        }
    }
}
