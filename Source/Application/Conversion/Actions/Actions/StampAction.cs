using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace pdfforge.PDFCreator.Conversion.Actions
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

        protected override ActionResult DoProcessJob(Job job)
        {
            throw new NotImplementedException();
        }

        public void ProcessJob(IPdfProcessor processor, Job job)
        {
            processor.AddStamp(job);
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

                if (checkLevel == CheckLevel.RunningJob)
                    actionResult.Add(_fontPathHelper.GetFontPath(profile));
            }
            return actionResult;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }
    }
}
