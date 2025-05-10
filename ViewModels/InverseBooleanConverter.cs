using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KitBox_Project.ViewModels
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue;
        }
    }
}