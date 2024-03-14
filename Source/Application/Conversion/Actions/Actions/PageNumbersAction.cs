using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class PageNumbersAction : ActionBase<PageNumbers>, IConversionAction
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFontPathHelper _fontPathHelper;

        public PageNumbersAction(IFontPathHelper fontPathHelper)
            : base(p => p.PageNumbers)
        {
            _fontPathHelper = fontPathHelper;
        }

        protected override ActionResult DoProcessJob(Job job, IPdfProcessor pdfProcessor)
        {
            pdfProcessor.AddPageNumbers(job);
            return new ActionResult();
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            var numberOfPagesToken = job.TokenReplacer.GetToken("NumberOfPages");
            job.TokenReplacer.AddStringToken("NumberOfPages", "<NumberOfPages>"); //Preserve the token to be replaced in the tool.
            job.Profile.PageNumbers.Format = job.TokenReplacer.ReplaceTokens(job.Profile.PageNumbers.Format);
            if (numberOfPagesToken != null) job.TokenReplacer.AddToken(numberOfPagesToken);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();

            if (profile.PageNumbers.Enabled)
            {
                if (string.IsNullOrEmpty(profile.PageNumbers.Format))
                {
                    _logger.Error("No page number format is specified.");
                    actionResult.Add(ErrorCode.PageNumbers_NoFormat);
                }
                else if (!profile.PageNumbers.Format.Contains("<PageNumber>"))
                {
                    _logger.Error("The page number format does not contain the <PageNumber> format.");
                    actionResult.Add(ErrorCode.PageNumbers_NoPageNumberInFormat);
                }

                if (checkLevel == CheckLevel.EditingProfile) 
                    if(!profile.UserTokens.Enabled && TokenIdentifier.ContainsUserToken(profile.PageNumbers.Format))
                        actionResult.Add(ErrorCode.PageNumbers_RequiresUserTokens);

                if (checkLevel == CheckLevel.RunningJob)
                {
                    if (!_fontPathHelper.TryGetFontPath(profile.PageNumbers.FontFile, out _))
                        actionResult.Add(ErrorCode.PageNumbers_FontNotFound);
                }
            }
            return actionResult;
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }
    }
}
