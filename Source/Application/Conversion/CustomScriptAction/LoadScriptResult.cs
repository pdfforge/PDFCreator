using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.CustomScriptAction
{
    public class LoadScriptResult
    {
        public ActionResult Result { get; }
        public IPDFCreatorScript Script { get; }
        public string ExceptionMessage { get; }

        public LoadScriptResult(ActionResult actionResult, IPDFCreatorScript script, string exceptionMessage)
        {
            Result = actionResult;
            Script = script;
            ExceptionMessage = exceptionMessage;
        }
    }
}
