using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface IActionManager
    {
        IEnumerable<T> GetActions<T>(ConversionProfile profile) where T : IAction;

        IEnumerable<T> GetActions<T>(Job job) where T : IAction;
    }
}
