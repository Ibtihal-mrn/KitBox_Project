using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace KitBox_Project.Converters
{
    public class PaidButtonContentConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
                return isPaid ? "Paid" : "Pay order";
            return "Pay order";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();

    }
}
