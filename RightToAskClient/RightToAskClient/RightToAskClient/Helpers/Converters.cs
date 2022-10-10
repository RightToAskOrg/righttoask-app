using System;
using System.Globalization;
using Xamarin.Forms;

namespace RightToAskClient.Helpers
{
    public class InvertConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Intended for use with strings. Return false if string is null or empty.
    public class NullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return string.IsNullOrEmpty(s);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // From https://alexdunn.org/2017/05/16/xamarin-tip-binding-a-picker-to-an-enum/
    public class IntEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                return (int)value;
            }
            return 0;
        }
 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int)
            {
                return Enum.ToObject(targetType, value);
            }
            return 0;
        }
    } 
}
