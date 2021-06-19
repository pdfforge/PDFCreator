using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System.Collections.Generic;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeActionOrderHelper : IActionOrderHelper
    {
        private readonly bool _isActionInDefaultOrder;
        private readonly bool _hasCorruptOrder;

        public DesignTimeActionOrderHelper(bool isActionInDefaultOrder, bool hasCorruptOrder)
        {
            _isActionInDefaultOrder = isActionInDefaultOrder;
            _hasCorruptOrder = hasCorruptOrder;
        }

        public void ForceDefaultOrder(ConversionProfile conversionWorkflow)
        {
        }

        public void ForceDefaultOrder(List<string> actionOrderList)
        {
        }

        public void EnsureEncryptionAndSignatureOrder(ConversionProfile conversionProfile)
        {
        }

        public bool IsActionInDefaultOrder(string action)
        {
            return _isActionInDefaultOrder;
        }

        public bool HasCorruptOrder(ConversionProfile conversionWorkflow)
        {
            return _hasCorruptOrder;
        }
    }
}
