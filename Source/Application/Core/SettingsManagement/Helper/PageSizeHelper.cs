using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public static class PageSizeHelper
    {
        public static PageSize ParsePageSize(string pageSize)
        {
            return (PageSize)Enum.Parse(typeof(PageSize), pageSize, true);
        }
    }
}
