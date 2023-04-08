using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace 脑卒中言语康复训练系统.Common.converters
{
    /// <summary>
    /// 用于给控件的展示与否进行转换
    /// </summary>
    public class IsShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = Visibility.Visible;
            if (value == null)
            {
                return data;
            }
            int val = (int)value;
            if (val == 0)
            {
                data = Visibility.Visible;
            }
            else if (val == 1)
            {
                data = Visibility.Hidden;
            }
            else
            {
                data |= Visibility.Collapsed;
            }
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
