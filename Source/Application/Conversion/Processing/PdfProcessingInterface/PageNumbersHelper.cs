using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public class PageNumbersHelper
    {
        public PageNumberPosition Alternate(PageNumberPosition pageAlignment)
        {
            switch (pageAlignment)
            {
                case PageNumberPosition.BottomRight:
                    return PageNumberPosition.BottomLeft;
                case PageNumberPosition.BottomLeft:
                    return PageNumberPosition.BottomRight;
                case PageNumberPosition.TopRight:
                    return PageNumberPosition.TopLeft;
                case PageNumberPosition.TopLeft:
                    return PageNumberPosition.TopRight;
                case PageNumberPosition.BottomCenter:
                case PageNumberPosition.TopCenter:
                    return pageAlignment;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pageAlignment), pageAlignment, null);
            }
        }

        public Tuple<float, float> CalculateUserOffset(float horizontalOffset, float verticalOffset, float height, float width, PageNumberPosition pageAlignment)
        {
            float calculatedY;
            switch (pageAlignment)
            {
                case PageNumberPosition.BottomRight:
                case PageNumberPosition.BottomLeft:
                case PageNumberPosition.BottomCenter:
                    calculatedY = 0 + verticalOffset;
                    break;
                case PageNumberPosition.TopRight:
                case PageNumberPosition.TopLeft:
                case PageNumberPosition.TopCenter:
                    calculatedY = height - verticalOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pageAlignment), pageAlignment, null);
            }


            float calculatedX;
            switch (pageAlignment)
            {
                case PageNumberPosition.TopLeft:
                case PageNumberPosition.BottomLeft:
                    calculatedX = 0 + horizontalOffset;
                    break;
                case PageNumberPosition.TopRight:
                case PageNumberPosition.BottomRight:
                    calculatedX = width - horizontalOffset;
                    break;
                case PageNumberPosition.TopCenter:
                case PageNumberPosition.BottomCenter:
                    calculatedX = horizontalOffset + (width / 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pageAlignment), pageAlignment, null);
            }

            return new Tuple<float, float>(calculatedX, calculatedY);
        }

        public string FormatPageNumber(string formatString, int page, int totalPageCount, bool useRoman)
        {
            var withPageNumber = formatString.Replace("<PageNumber>", useRoman ? ToRoman(page) : page.ToString());
            var withTotalPageCount = withPageNumber.Replace("<NumberOfPages>", useRoman ? ToRoman(totalPageCount) : totalPageCount.ToString());
            return withTotalPageCount;
        }

        public string ToRoman(int number)
        {

            var retVal = new StringBuilder(5);
            var valueMap = new SortedDictionary<int, string>
            {
                { 1, "I" },
                { 4, "IV" },
                { 5, "V" },
                { 9, "IX" },
                { 10, "X" },
                { 40, "XL" },
                { 50, "L" },
                { 90, "XC" },
                { 100, "C" },
                { 400, "CD" },
                { 500, "D" },
                { 900, "CM" },
                { 1000, "M" },
            };

            foreach (var kvp in valueMap.Reverse())
            {
                while (number >= kvp.Key)
                {
                    number -= kvp.Key;
                    retVal.Append(kvp.Value);
                }
            }

            return retVal.ToString();
        }
    }
}
