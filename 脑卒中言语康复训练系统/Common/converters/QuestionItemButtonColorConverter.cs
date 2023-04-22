using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using 脑卒中言语康复训练系统.Models;

namespace 脑卒中言语康复训练系统.Common.converters
{
    public class QuestionItemButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null && value is OptionRaise) 
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffa000"));
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3f51b5"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
