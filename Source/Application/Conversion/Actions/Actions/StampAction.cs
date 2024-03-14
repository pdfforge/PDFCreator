using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class StampAction : ActionBase<Stamping>, IConversionAction
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFontPathHelper _fontPathHelper;

        public StampAction(IFontPathHelper fontPathHelper)
            : base(p => p.Stamping)
        {
            _fontPathHelper = fontPathHelper;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor processor)
        {
            processor.AddStamp(job);
            return new ActionResult();
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.Stamping.StampText = job.TokenReplacer.ReplaceTokens(job.Profile.Stamping.StampText);
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            if (profile.Stamping.Enabled)
            {
                if (string.IsNullOrEmpty(profile.Stamping.StampText))
                {
                    _logger.Error("No stamp text is specified.");
                    actionResult.Add(ErrorCode.Stamp_NoText);
                }
                else if (checkLevel == CheckLevel.EditingProfile)
                {
                    if (!profile.UserTokens.Enabled && TokenIdentifier.ContainsUserToken(profile.Stamping.StampText))
                        actionResult.Add(ErrorCode.Stamp_RequiresUserTokens);
                }

                if (checkLevel == CheckLevel.RunningJob)
                {
                    if (!_fontPathHelper.TryGetFontPath(profile.Stamping.FontFile, out _))
                        actionResult.Add(ErrorCode.Stamp_FontNotFound);
                }
            }
            return actionResult;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }
    }
}
