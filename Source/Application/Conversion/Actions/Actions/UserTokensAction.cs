using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class UserTokensAction : ActionBase<UserTokens>, IPreConversionAction, IBusinessFeatureAction
    {
        public UserTokensAction() : base(p => p.UserTokens)
        { }

        protected override ActionResult DoProcessJob(Job job)
        {
            return new ActionResult();
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            //Nothing to do here
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            //Nothing to do here
            return new ActionResult();
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }
    }
}
