using System;
using System.Globalization;
using Xamarin.Forms;
using static RightToAskClient.Helpers.BoolEnumConverter;

namespace RightToAskClient.Helpers
{
    public class InvertConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Intended for use with strings. Return false if string is null or empty.
    public class NullorEmptyConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return String.IsNullOrEmpty(value as string);
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
    
    // For use in radio buttons. Turns a (boolean) selection into the correct enum; turns an enum into the correct 
    // boolean selection (i.e. true for the matching one and false otherwise).
    public class BoolEnumConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return false;

            if (parameter is null)
                return false;

            // Return false if the converter doesn't make sense.
            if (parameter.GetType() != value.GetType())
                return false;

            // return value.Equals(Enum.Parse(value.GetType(), (string)parameter, true));
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    } 
    
    // The boolean opposite of BoolEnumConverter
    public class BoolEnumInvertConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
                return false;

            if (parameter is null)
                return false;

            // Return false if the converter doesn't make sense.
            if (parameter.GetType() != value.GetType())
                return false;

            // return value.Equals(Enum.Parse(value.GetType(), (string)parameter, true));
            return !value.Equals(parameter);
        }

        
        // Not clear that this is ever useful. The logical negation of exact matching doesn't make a lot of sense.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
