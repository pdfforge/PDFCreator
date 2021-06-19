using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeProfileChecker : IProfileChecker
    {
        public ActionResultDict CheckProfileList(CurrentCheckSettings settings)
        {
            return new ActionResultDict();
        }

        public ActionResult CheckFileNameAndTargetDirectory(ConversionProfile profile)
        {
            return new ActionResult();
        }

        public ActionResult CheckProfile(ConversionProfile profile, CurrentCheckSettings settings)
        {
            return new ActionResult();
        }

        public ActionResult CheckJob(Job job)
        {
            return new ActionResult();
        }
    }
}
