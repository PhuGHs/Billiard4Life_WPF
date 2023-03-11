using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Billiard4Life
{
    public class Converter
    {
        private static Converter _imageConverter;
        public static Converter ImageConverter
        {
            get
            {
                if (_imageConverter == null)
                {
                    _imageConverter = new Converter();
                }
                return Converter._imageConverter;
            }
            set
            {
                _imageConverter = value;
            }
        }
        public BitmapImage ConvertByteToBitmapImage(Byte[] image)
        {
            BitmapImage bitImage = new BitmapImage();
            MemoryStream mem = new MemoryStream();
            if (image == null)
            {
                return null;
            }
            mem.Write(image, 0, image.Length);
            mem.Position = 0;
            Image img = Image.FromStream(mem);
            bitImage.BeginInit();
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bitImage.StreamSource = ms;
            bitImage.EndInit();
            return bitImage;
        }
        public Byte[] ConvertImageToBytes(BitmapImage bitmapImage)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
    }
}
