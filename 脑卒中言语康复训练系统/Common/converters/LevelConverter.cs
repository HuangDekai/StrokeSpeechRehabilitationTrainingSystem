using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 脑卒中言语康复训练系统.Common.converters
{
    class LevelConverter : IValueConverter
    {
        private string[] levels = new string[]
        {
            "零","一","二","三","四","五","六","七","八","九","十"
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int data = 1;
            if (value != null)
            {
                data = ((int) value) > 10 ? 10 : (int)value;
            }
            return levels[data] + "级";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
