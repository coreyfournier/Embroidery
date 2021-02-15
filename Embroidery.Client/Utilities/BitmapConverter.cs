using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;

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
            else if (value is Models.View.GroupedFile groupedFile && targetType == typeof(Avalonia.Media.IImage))
            {
                using (var db = new DataContext())
                {
                    var image = db.Files
                        .Where(x => x.Id == groupedFile.FirstFileId)
                        .Select(x=> x.ImageThumbnail)
                        .FirstOrDefault();

                    if (image == null)
                        return null;

                    return DbImageToBmpFile(groupedFile.FirstFileId, image);
                }
            }
            else if (value is Models.File file && targetType == typeof(Avalonia.Media.IImage))
            {
                if (file.ImageThumbnail == null)
                    return null;

                return DbImageToBmpFile(file.Id, file.ImageThumbnail);
            }

            throw new NotSupportedException();
        }

        private Bitmap DbImageToBmpFile(int fileId, byte[] jpgFile)
        {
            var bmpFileName = $"{Program.ImageCacheFolder}\\{fileId}.bmp";

            if (!System.IO.File.Exists(bmpFileName))
            {
                using (var jpgStream = new System.IO.MemoryStream(jpgFile))
                {
                    var image = System.Drawing.Image.FromStream(jpgStream);

                    var bitmap = new System.Drawing.Bitmap(image);
                    ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID
                    // for the Quality parameter category.
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object.
                    // An EncoderParameters object has an array of EncoderParameter
                    // objects. In this case, there is only one
                    // EncoderParameter object in the array.
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter;
                    myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    bitmap.Save(bmpFileName, jgpEncoder, myEncoderParameters);

                    //myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                    //myEncoderParameters.Param[0] = myEncoderParameter;
                    //bitmap.Save(bmpFileName, jgpEncoder, myEncoderParameters);

                    //// Save the bitmap as a JPG file with zero quality level compression.
                    //myEncoderParameter = new EncoderParameter(myEncoder, 0L);
                    //myEncoderParameters.Param[0] = myEncoderParameter;
                    //bitmap.Save(bmpFileName, jgpEncoder, myEncoderParameters);

                    //bitmap.Save(bmpFileName);
                    System.Diagnostics.Debug.WriteLine($"Converting FileId:{fileId} to bmp and caching it");
                }
            }

            return new Bitmap(bmpFileName);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
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
