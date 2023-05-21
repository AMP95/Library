using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Library.ViewModel.Converters
{
    internal class AlfasOnlyStringConverter : IValueConverter
    {
        //Конвертер для имени, не допускает ввода символов кроме букв/ю/-
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string y)
            {
                for (int i = 0; i < y.Length;)
                {
                    if (!char.IsLetter(y[i]) && y[i] != '.' && y[i] != '-' && y[i] != ' ')
                    {
                        y = y.Replace($"{y[i]}", "");
                    }
                    else
                    {
                        if (i == 0 && (y[i] == ' ' || y[i] == '.' || y[i] == '-'))
                        {
                            y = y.Replace($"{y[i]}", "");
                        }
                        else
                        {
                            if (y.Count(c => c == '.') > 2 && y[i] == '.')
                                y = y.Remove(y.Length - 1);
                            else
                            {
                                if (y.Count(c => c == '-') > 1 && y[i] == '-')
                                    y = y.Remove(y.Length - 1);
                                else
                                    i++;
                            }
                        }
                    }
                }
                return y;
            }
            return null;
        }
    }
}
