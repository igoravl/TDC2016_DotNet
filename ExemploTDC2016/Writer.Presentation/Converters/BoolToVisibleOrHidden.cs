using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Waf.Writer.Presentation.Converters
{
    internal class BoolToVisibleOrHidden : IValueConverter
    {
        #region Constructors

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = (bool) value;
            if (bValue)
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility) value;

            if (visibility == Visibility.Visible)
                return true;
            return false;
        }

        #endregion
    }
}