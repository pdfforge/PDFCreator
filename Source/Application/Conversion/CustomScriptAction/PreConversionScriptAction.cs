using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.CustomScriptAction
{
    public class PreConversionScriptAction : IPreConversionAction, ICheckable
    {
        private readonly ICustomScriptHandler _customScriptHandler;
        private readonly ICustomScriptLoader _customScriptLoader;

        public PreConversionScriptAction(ICustomScriptHandler customScriptHandler, ICustomScriptLoader customScriptLoader)
        {
            _customScriptHandler = customScriptHandler;
            _customScriptLoader = customScriptLoader;
        }

        public ActionResult ProcessJob(Job job)
        {
            return _customScriptHandler.ExecutePreConversion(job);
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.CustomScript.Enabled;
        }

        public void ApplyPreSpecifiedTokens(Job job)
        { }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            if (!profile.CustomScript.Enabled)
                return new ActionResult();

            var loadResult = _customScriptLoader.LoadScriptWithValidation(profile.CustomScript.ScriptFilename);
            return loadResult.Result;
        }
    }
}
