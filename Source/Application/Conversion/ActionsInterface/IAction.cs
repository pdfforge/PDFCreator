using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    /// <summary>
    /// Marker for PreConversion Action
    /// </summary>
    public interface IPreConversionAction : IAction
    { }

    /// <summary>
    /// Marker for PostConversion Action
    /// </summary>
    public interface IPostConversionAction : IAction
    { }

    /// <summary>
    ///     The interface Action defines actions that can process a set of files (i.e. encrypt, send as mail)
    ///     and return a set of files after processing them
    /// </summary>
    public interface IAction
    {
        /// <summary>
        ///     Process all output files
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        ActionResult ProcessJob(Job job);

        bool IsEnabled(ConversionProfile profile);
    }
}
