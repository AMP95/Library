using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Library.Model;

namespace Library.ViewModel.Converters
{
    //Перевод значения Enum в строку
    class EnumStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is BookTheme bt)
            {
                return Enum.GetName(bt);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is string bt)
            {
                return Enum.Parse<BookTheme>(bt);
            }
            return null;
        }
    }
}
