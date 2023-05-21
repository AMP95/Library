using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Library.ViewModel.Converters
{
    //Для Годаб не допускает символов кроме цыфр
    class StringIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int y)
            {
                return y.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string y)
            {
                for (int i = 0; i < y.Length;)
                {
                    if (!char.IsDigit(y[i]))
                    {
                        y = y.Replace($"{y[i]}", "");
                    }
                    else
                    {
                        if (i == 0 && y[i] == '0')
                        {
                            y = y.Replace($"{y[i]}", "");
                        }
                        else
                            i++;
                    }
                }
                if (y.Length != 0)
                {
                    int number = System.Convert.ToInt32(y);
                    if (number > DateTime.Now.Year)
                        number = DateTime.Now.Year;
                    return number;
                }
            }
            return null;
        }
    }
}
