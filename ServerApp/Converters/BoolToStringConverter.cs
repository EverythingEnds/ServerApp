using System;
using System.Globalization;

namespace ClientApp.Converters
{
    public class BoolToStringConverter : Extensions.ConvertorBase<BoolToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value == false)
                return "Включить сервер";
            else
                return "Выключить сервер";
        }
    }
}