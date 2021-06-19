using System;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public static class UpdateIntervalHelper
    {
        public static UpdateInterval ParseUpdateInterval(string updateInterval)
        {
            return (UpdateInterval)Enum.Parse(typeof(UpdateInterval), updateInterval, true);
        }
    }
}
