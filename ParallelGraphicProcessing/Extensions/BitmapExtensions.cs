using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelGraphicProcessing.Extensions
{
    static class BitmapExtensions
    {
        public static ImageSource ToImageSource(this SKBitmap bitmap)
        {
            var imageStream = new MemoryStream();
            bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(imageStream);
            imageStream.Seek(0, SeekOrigin.Begin);
            return ImageSource.FromStream(() => new MemoryStream(imageStream.ToArray()));
        }


    }
}
