﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chess.Converters
{
    public class BoolInvertedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool))
                return Binding.DoNothing;

            return (bool)value ? Visibility.Collapsed : Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
