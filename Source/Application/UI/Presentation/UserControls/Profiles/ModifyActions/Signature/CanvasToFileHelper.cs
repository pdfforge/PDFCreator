using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public interface ICanvasToFileHelper
    {
        Bitmap CanvasToBitmap(Canvas paintSurface);

        Bitmap CropSignature(Bitmap bitmap);

        bool SaveToFile(Bitmap croppedSignature, string path);
    }

    public class CanvasToFileHelper : ICanvasToFileHelper
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Bitmap CanvasToBitmap(Canvas paintSurface)
        {
            try
            {
                var renderTargetBitmap = new RenderTargetBitmap(
                    (int)paintSurface.RenderSize.Width,
                    (int)paintSurface.RenderSize.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32);
                renderTargetBitmap.Render(paintSurface);

                using var stream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                encoder.Save(stream);
                return new Bitmap(stream);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Could not convert the canvas to a bitmap format.");
                throw;
            }
        }

        public Bitmap CropSignature(Bitmap bitmap)
        {
            var bitmapWidth = bitmap.Width;
            var bitmapHeight = bitmap.Height;
            var leftMost = 0;
            var rightMost = bitmapWidth - 1;
            var topMost = 0;
            var bottomMost = bitmapHeight - 1;

            // Finding the leftmost non-transparent column
            for (var col = 0; col < bitmapWidth; col++)
            {
                if (IsColumnColored(col, bitmap))
                {
                    leftMost = col;
                    break;
                }
            }

            // Finding the rightmost non-transparent column
            for (var col = rightMost; col > 0; col--)
            {
                if (IsColumnColored(col, bitmap))
                {
                    rightMost = col;
                    break;
                }
            }

            // Finding the topmost non-transparent row
            for (var row = 0; row < bitmapHeight; row++)
            {
                if (IsRowColored(row, bitmap))
                {
                    topMost = row;
                    break;
                }
            }

            // Finding the bottom-most non-transparent row
            for (var row = bottomMost; row > 0; row--)
            {
                if (IsRowColored(row, bitmap))
                {
                    bottomMost = row;
                    break;
                }
            }

            // No cropping necessary
            if (leftMost == 0 && topMost == 0 && rightMost == bitmapWidth - 1 && bottomMost == bitmapHeight - 1)
                return bitmap;

            var croppedWidth = rightMost - leftMost + 1;
            var croppedHeight = bottomMost - topMost + 1;

            try
            {
                var croppedImage = new Bitmap(croppedWidth, croppedHeight);
                using var g = Graphics.FromImage(croppedImage);
                g.DrawImage(bitmap,
                    new RectangleF(0, 0, croppedWidth, croppedHeight),
                    new RectangleF(leftMost, topMost, croppedWidth, croppedHeight),
                    GraphicsUnit.Pixel);
                return croppedImage;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not crop image. Returning NON-CROPPED signature.");
                return bitmap;
            }
        }

        public bool SaveToFile(Bitmap croppedSignature, string path)
        {
            try
            {
                croppedSignature.Save(path, ImageFormat.Png);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed saving your file.");
                return false;
            }
        }

        private static bool IsRowColored(int row, Bitmap bitmap)
        {
            for (var i = 0; i < bitmap.Width; i++)
            {
                if (bitmap.GetPixel(i, row).A != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsColumnColored(int col, Bitmap bitmap)
        {
            for (var i = 0; i < bitmap.Height; i++)
            {
                if (bitmap.GetPixel(col, i).A != 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
