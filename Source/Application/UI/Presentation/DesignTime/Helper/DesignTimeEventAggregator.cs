using Prism.Events;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeEventAggregator : IEventAggregator
    {
        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            return Activator.CreateInstance<TEventType>();
        }
    }
}
