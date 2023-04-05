using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 脑卒中言语康复训练系统.Common.converters
{
    internal class GenderConverter : IValueConverter
    {
        /// <summary>
        /// 目标到源, 将 Gender 的数字转换为 男(1)/女(0)/空字符串(-1)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short gender = 0;
            if (value != null)
            {
                gender = (short)value;
            }
            if (gender == 1)
            {
                return "男";
            }
            return gender == 0 ? "女" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
