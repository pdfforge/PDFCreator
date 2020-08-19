using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Threading;
using Prism.Events;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeMainShellViewModel : MainShellViewModel
    {
        public DesignTimeMainShellViewModel() : base(new DragAndDropEventHandler(null), new TranslationUpdater(new TranslationFactory(),
            new ThreadManager()), new DesignTimeApplicationNameProvider(), new InteractionRequest(),
            new EventAggregator(), new DesignTimeCommandLocator(), null, null, null, null, null, null,
            new DesignTimeCurrentSettings<Conversion.Settings.UsageStatistics>(),
            new DesignTimeVersionHelper(), null, null)
        {
        }
    }
}
