using System;
using System.Globalization;

namespace Verno
{
    public class SimpleValueConverter : IValueConverter
    {
        public T Convert<T>(object value)
        {
            return Convert<T>(value, CultureInfo.CurrentCulture);
        }

        public T Convert<T>(object value, CultureInfo culture)
        {
            return (T) Convert(value, typeof (T), culture);
        }

        public object Convert(object value, Type targetType, CultureInfo culture)
        {
            if (targetType == typeof(bool))
                return value.AsBool();
            else if (value is String && targetType.IsEnum)
                return Enum.Parse(targetType, (string)value, true);
            else if (value is String
                     && (targetType == typeof (decimal)
                         || targetType == typeof (double)
                         || targetType == typeof (Single)))
            {
                value = ConvertExtesions.ReplaceNumberFormat((string)value);
            }
            return System.Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
        }
    }
}