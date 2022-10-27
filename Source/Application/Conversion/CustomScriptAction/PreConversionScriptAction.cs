using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.CustomScriptAction
{
    public class PreConversionScriptAction : ActionBase<CustomScript>, IPreConversionAction, IBusinessFeatureAction
    {
        private readonly ICustomScriptHandler _customScriptHandler;
        private readonly ICustomScriptLoader _customScriptLoader;

        public PreConversionScriptAction(ICustomScriptHandler customScriptHandler, ICustomScriptLoader customScriptLoader)
            : base(p => p.CustomScript)
        {
            _customScriptHandler = customScriptHandler;
            _customScriptLoader = customScriptLoader;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            return _customScriptHandler.ExecutePreConversion(job);
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        { }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            if (!profile.CustomScript.Enabled)
                return new ActionResult();

            var loadResult = _customScriptLoader.LoadScriptWithValidation(profile.CustomScript.ScriptFilename);
            return loadResult.Result;
        }
    }
}
