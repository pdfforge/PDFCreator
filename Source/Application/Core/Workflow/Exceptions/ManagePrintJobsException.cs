namespace pdfforge.PDFCreator.Core.Workflow.Exceptions
{
    /// <summary>
    ///     The sole purpose of the exception is to signal that the user wants to manage the print jobs and that the processing
    ///     shall be suspended during that time
    /// </summary>
    public class ManagePrintJobsException : InterruptWorkflowException
    {
    }
}
