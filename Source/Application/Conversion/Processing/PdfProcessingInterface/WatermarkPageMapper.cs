using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public class WatermarkPageMapper
    {
        private readonly int _numBackgroundPages;
        private readonly IWatermarkSettings _watermarkSettings;

        public WatermarkPageMapper(int numBackgroundPages, IWatermarkSettings watermarkSettings)
        {
            _numBackgroundPages = numBackgroundPages;
            _watermarkSettings = watermarkSettings;
        }

        ///<summary>
        ///     Determine the watermark page number for the current document page number
        /// </summary>
        public bool GetWatermarkPageNumber(int documentPageNumber, out int watermarkPageNumber)
        {
            watermarkPageNumber = -1;

            if (documentPageNumber <= 0)
                return false;

            if (documentPageNumber > _numBackgroundPages)
            {
                if (_watermarkSettings.Repetition == BackgroundRepetition.NoRepetition)
                    return false;

                if (_watermarkSettings.Repetition == BackgroundRepetition.RepeatLastPage)
                {
                    watermarkPageNumber = _numBackgroundPages;
                    return true;
                }
            }

            watermarkPageNumber = ((documentPageNumber - 1) % _numBackgroundPages) + 1;

            return true;
        }
    }
}
