using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface IActionOrderHelper
    {
        void EnsureValidOrder(List<string> currentActionOrderList);

        void CleanUpAndEnsureValidOrder(IEnumerable<ConversionProfile> profiles);
    }
}
