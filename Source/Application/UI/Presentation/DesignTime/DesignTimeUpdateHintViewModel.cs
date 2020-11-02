using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System.Reflection;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateHintViewModel : UpdateHintViewModel
    {
        public DesignTimeUpdateHintViewModel()
            : base(new DesignTimeUpdateHelper(),
                new DesignTimeTranslationUpdater(),
                new EventAggregator(),
                new DesignTimeVersionHelper(),
                new DesignTimeUpdateLauncher(),
                null,
                new DisabledOnlineVersionHelper(new DesignTimeVersionHelper()),
                new AssemblyHelper(Assembly.GetExecutingAssembly()))
        {
        }
    }
}
