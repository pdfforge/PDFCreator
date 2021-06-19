using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public partial class ConversionProfile
    {
        public bool IsDefault => Guid == ProfileGuids.DEFAULT_PROFILE_GUID;

        public bool IsFirstActionBeforeSecond(string nameOfFirstActionSetting, string nameOfSecondActionSettings)
        {
            var firstIndex = ActionOrder.IndexOf(nameOfFirstActionSetting);
            if (firstIndex < 0)
                return false;
            var secondIndex = ActionOrder.IndexOf(nameOfSecondActionSettings);
            return firstIndex < secondIndex;
        }

        public bool SupportsMetadata => OutputFormat.IsPdf();
    }
}
