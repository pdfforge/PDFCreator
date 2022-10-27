using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.CustomScriptAction
{
    public class PostConversionScriptAction : ActionBase<CustomScript>, IPostConversionAction, IBusinessFeatureAction
    {
        private readonly ICustomScriptHandler _customScriptHandler;

        public PostConversionScriptAction(ICustomScriptHandler customScriptHandler)
            : base(p => p.CustomScript)
        {
            _customScriptHandler = customScriptHandler;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            return _customScriptHandler.ExecutePostConversion(job);
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            //Nothing to do here
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        {
            //Implement this in PreConversionScriptAction
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            // Check is done in PreConversionScriptAction
            return new ActionResult();
        }
    }
}
