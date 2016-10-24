using System;
using System.Globalization;

namespace Verno
{
    public interface IValueConverter
    {
        object Convert(object value, Type targetType, CultureInfo culture);
    }
}