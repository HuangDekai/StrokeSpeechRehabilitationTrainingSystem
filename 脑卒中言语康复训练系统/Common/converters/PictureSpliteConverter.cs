using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using 脑卒中言语康复训练系统.Models;

namespace 脑卒中言语康复训练系统.Common.converters
{
    class PictureSpliteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AnswerRaise answer = value as AnswerRaise;
            string fileName = "";
            TransformedBitmap res = null;
            if (answer != null)
            {
                fileName = answer.Picture;
                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
                var imagePath = Path.Combine("Image", fileName);
                var imagePathRelativeToBaseDirectory = Path.Combine(baseDirectory, imagePath);
                fileName = imagePathRelativeToBaseDirectory.ToString();
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(fileName);
                image.EndInit();
                if (answer.IsCorrect)
                {
                    // 定义矩形区域
                    Int32Rect rect2 = new Int32Rect(image.PixelWidth / 2, 0, image.PixelWidth / 2, image.PixelHeight);

                    // 创建CroppedBitmap
                    CroppedBitmap cb = new CroppedBitmap(image, rect2);

                    // 创建TransformedBitmap
                    res = new TransformedBitmap(cb, new ScaleTransform(-1, 1));
                } else
                {
                    // 定义矩形区域
                    Int32Rect rect = new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight);

                    // 创建CroppedBitmap
                    CroppedBitmap cb = new CroppedBitmap(image, rect);

                    res = new TransformedBitmap(cb, new ScaleTransform(-1, 1));
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
