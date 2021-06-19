using System;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public class AggregateProcessingException : Exception
    {
        public AggregateProcessingException(string message, ActionResult result)
            : base(message)
        {
            Result = result;
        }

        public ActionResult Result { get; private set; }
    }
}
