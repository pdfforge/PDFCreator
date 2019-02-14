using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.CustomScriptAction
{
    public class PostConversionScriptAction : IPostConversionAction
    // Do not implement ICheckable.
    // Check is done in PreConversionScriptAction
    {
        private readonly ICustomScriptHandler _customScriptHandler;

        public PostConversionScriptAction(ICustomScriptHandler customScriptHandler)
        {
            _customScriptHandler = customScriptHandler;
        }

        public ActionResult ProcessJob(Job job)
        {
            return _customScriptHandler.ExecutePostConversion(job);
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.CustomScript.Enabled;
        }
    }
}
