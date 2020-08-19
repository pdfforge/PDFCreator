using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class StampAction : IConversionAction, ICheckable
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFontPathHelper _fontPathHelper;

        public StampAction(IFontPathHelper fontPathHelper)
        {
            _fontPathHelper = fontPathHelper;
        }

        public ActionResult ProcessJob(Job job)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.Stamping.Enabled;
        }

        public void ProcessJob(IPdfProcessor processor, Job job)
        {
            processor.AddStamp(job);
        }

        public void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.Stamping.StampText = job.TokenReplacer.ReplaceTokens(job.Profile.Stamping.StampText);
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            if (profile.Stamping.Enabled)
            {
                if (string.IsNullOrEmpty(profile.Stamping.StampText))
                {
                    _logger.Error("No stamp text is specified.");
                    actionResult.Add(ErrorCode.Stamp_NoText);
                }

                if (checkLevel == CheckLevel.Job)
                    actionResult.Add(_fontPathHelper.GetFontPath(profile));
            }
            return actionResult;
        }
    }
}
