using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 脑卒中言语康复训练系统.Common.converters
{
    internal class AvatarPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fileName = value as string;
            if (fileName != null)
            {
                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\","/");
                var imagePath = Path.Combine("Image", "Avatar", fileName);
                var imagePathRelativeToBaseDirectory = Path.Combine(baseDirectory, imagePath);
                fileName = imagePathRelativeToBaseDirectory.ToString();
            }
            return fileName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
