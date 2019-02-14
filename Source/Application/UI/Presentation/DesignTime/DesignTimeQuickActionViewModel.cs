using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Windows;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeQuickActionViewModel : QuickActionViewModel
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(DesignTimeQuickActionViewModel), new PropertyMetadata(default(bool)));

        public DesignTimeQuickActionViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()), new DesignTimeCommandLocator(), null, null, null)
        {
        }
    }
}
