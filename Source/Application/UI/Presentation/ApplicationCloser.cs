using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class ApplicationCloser : IApplicationCloser
    {
        private readonly IEventAggregator _eventAggregator;

        public ApplicationCloser(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void CloseApplication()
        {
            _eventAggregator.GetEvent<TryCloseApplicationEvent>().Publish();
        }
    }
}
