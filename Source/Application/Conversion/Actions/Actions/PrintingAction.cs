using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Helper;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    /// <summary>
    ///     Implements the action to print the input files
    /// </summary>
    public class PrintingAction : ActionBase<Printing>, IPostConversionAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IJobPrinter _jobPrinter;
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PrintingAction(IJobPrinter jobPrinter, IAppSettingsProvider appSettingsProvider)
            : base(p => p.Printing)
        {
            _jobPrinter = jobPrinter;
            _appSettingsProvider = appSettingsProvider;
        }

        /// <summary>
        ///     Prints the input files to the configured printer
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        protected override ActionResult DoProcessJob(Job job)
        {
            Logger.Debug("Launched Printing-Action");

            try
            {
                _jobPrinter.Print(job);
                return new ActionResult();
            }
            catch
            {
                Logger.Error("Error while printing");
                return new ActionResult(ErrorCode.Printing_GenericError);
            }
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            // Nothing to do here
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings currentCheckSettings, CheckLevel checkLevel)
        {
            //todo:
            //Profile Level: Maybe check if printer is installed

            //Job Level: Nothing to do

            var actionResult = new ActionResult();

            if (profile.Printing.SelectPrinter != SelectPrinter.SelectedPrinter)
                return actionResult;

            var (hasCyclicDependency, dependencyRoute) = CyclicDependenciesHelper.HasCyclicDependency(profile, _appSettingsProvider.Settings.PrinterMappings, currentCheckSettings.Profiles);

            if (hasCyclicDependency)
            {
                _logger.Error("The forward to further profile action forwards causes a circular dependency " +
                              "between profiles with the Guids:\r\n" +
                              string.Join(" -> ", dependencyRoute));
                actionResult.Add(ErrorCode.Printing_CyclicDependencyError);
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
