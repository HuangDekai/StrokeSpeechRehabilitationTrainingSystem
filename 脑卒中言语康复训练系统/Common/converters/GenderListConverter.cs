using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 脑卒中言语康复训练系统.Common.converters
{
    internal class GenderListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (ObservableCollection<short>)value;
            var res = new ObservableCollection<string>();
            res.Add("");
            foreach (var item in list)
            {
                if (item == 0)
                {
                    res.Add("女");
                }
                else if (item == 1)
                {
                    res.Add("男");
                }
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
