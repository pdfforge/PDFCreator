using System;
using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum PageNumberPosition
    {
        [Translation("Bottom Right")]
        BottomRight,
        [Translation("Bottom Left")]
        BottomLeft,
        [Translation("Bottom Center")]
        BottomCenter,
        [Translation("Top Right")]
        TopRight,
        [Translation("Top Left")]
        TopLeft,
        [Translation("Top Center")]
        TopCenter,
    }

    public static class PageNumberExtension
    {
        public static bool IsCorner(this PageNumberPosition pos)
        {
            switch (pos)
            {
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.TopRight:
                case PageNumberPosition.TopLeft:
                    return true;
                case PageNumberPosition.BottomCenter:
                case PageNumberPosition.TopCenter:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
            }
        }
        public static bool IsLeft(this PageNumberPosition pos)
        {
            switch (pos)
            {
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.TopLeft:
                    return true;
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.TopRight:
                case PageNumberPosition.BottomCenter:
                case PageNumberPosition.TopCenter:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
            }
        }
        public static bool IsRight(this PageNumberPosition pos)
        {
            switch (pos)
            {
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.TopRight:
                    return true;
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.TopLeft:
                case PageNumberPosition.BottomCenter:
                case PageNumberPosition.TopCenter:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
            }
        }
        public static bool IsCenter(this PageNumberPosition pos)
        {
            switch (pos)
            {
                case PageNumberPosition.BottomCenter:
                case PageNumberPosition.TopCenter:
                    return true;
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.TopRight:
                case PageNumberPosition.TopLeft:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
            }
        }

        public static bool IsTop(this PageNumberPosition pos)
        {
            switch (pos)
            {
                case PageNumberPosition.TopRight:
                case PageNumberPosition.TopLeft:
                case PageNumberPosition.TopCenter:
                    return true;
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.BottomCenter:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
            }
        }

        public static bool IsBottom(this PageNumberPosition pos)
        {
            switch (pos)
            {
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.BottomCenter:
                    return true;
                case PageNumberPosition.TopRight:
                case PageNumberPosition.TopLeft:
                case PageNumberPosition.TopCenter:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
            }
        }
    }
}
