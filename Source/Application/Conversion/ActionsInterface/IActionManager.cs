using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface IActionManager
    {
        IEnumerable<IPreConversionAction> GetApplicablePreConversionActions(Job job);

        IEnumerable<IPostConversionAction> GetApplicablePostConversionActions(Job job);
    }
}
