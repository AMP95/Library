using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Library.Model;

namespace Library.ViewModel.Converters
{
    class EnumSolidBrushConverter : IValueConverter
    {
        //Обеспечивает разную окраску item на основе темы (Выбранная Nature)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BookTheme theme)
            {
                if (theme == BookTheme.Nature)
                {
                    return new SolidColorBrush(Color.FromRgb(198, 209, 187));
                }
            }
            return new SolidColorBrush(Color.FromRgb(167, 178, 185));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
