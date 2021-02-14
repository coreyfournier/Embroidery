using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;

namespace Embroidery.Client.Utilities
{
    /// <summary>
    /// Converts a jpg image to bitmap
    /// </summary>
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is byte[] imageBytes && targetType == typeof(Avalonia.Media.IImage))
            {
                return new Bitmap(new System.IO.MemoryStream(imageBytes));
                
            }
            else if (value is Models.File file && targetType == typeof(Avalonia.Media.IImage))
            {
                if (file.ImageThumbnail == null)
                    return null;

                var bmpFileName = $"{Program.ImageCacheFolder}\\{file.Id}.bmp";

                if (!System.IO.File.Exists(bmpFileName))
                {
                    using (var jpgStream = new System.IO.MemoryStream(file.ImageThumbnail))
                    {
                        var image = System.Drawing.Image.FromStream(jpgStream);
                        var bitmap = new System.Drawing.Bitmap(image);

                        bitmap.Save(bmpFileName);
                    }
                }

                return new Bitmap(bmpFileName);
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Bitmap ConvertToBitmap(string fileName)
        {
            System.Drawing.Bitmap bitmap;
            using (System.IO.Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(bmpStream);

                bitmap = new System.Drawing.Bitmap(image);

            }
            return bitmap;
        }
    }
}
