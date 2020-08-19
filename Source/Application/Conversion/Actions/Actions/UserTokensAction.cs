using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class UserTokensAction : IPreConversionAction, IBusinessFeatureAction
    {
        public ActionResult ProcessJob(Job job)
        {
            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.UserTokens.Enabled;
        }
    }
}
