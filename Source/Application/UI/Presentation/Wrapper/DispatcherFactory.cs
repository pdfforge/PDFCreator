using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.UI.Presentation.Wrapper
{
    public class DispatcherFactory : IDispatcherFactory
    {
        public IDispatcher CreateDispatcher()
        {
            return new DispatcherWrapper();
        }
    }
}