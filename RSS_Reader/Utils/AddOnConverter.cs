using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace RSS_Reader.Utils
{
    /// <summary>
    /// Утилита для XAML для того, чтобы можно было добавить/вычесть из какого-либо значения. Например, вычесть parameter из длины элемента управления родителя
    /// </summary>
    public class AddOnConverter : MarkupExtension, IValueConverter
    {
        private static AddOnConverter Instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return System.Convert.ToDouble(value) + System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance ?? (Instance = new AddOnConverter());
        }
    }
}
