using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class BusinessHintStatusBarViewModel : TranslatableViewModelBase<BusinessFeatureTranslation>
    {
        public ICommand UrlOpenCommand { get; }

        public BusinessHintStatusBarViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator) : base(translationUpdater)
        {
            UrlOpenCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.BusinessHintLink);
        }
    }
}
