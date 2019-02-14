using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    /// <summary>
    ///     Implements the action to print the input files
    /// </summary>
    public class PrintingAction : IPostConversionAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IJobPrinter _jobPrinter;

        public PrintingAction(IJobPrinter jobPrinter)
        {
            _jobPrinter = jobPrinter;
        }

        /// <summary>
        ///     Prints the input files to the configured printer
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(Job job)
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

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.Printing.Enabled;
        }
    }
}
