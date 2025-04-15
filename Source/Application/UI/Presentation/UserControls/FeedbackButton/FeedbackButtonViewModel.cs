using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement.GPO.Settings;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    internal class FeedbackButtonViewModel : TranslatableViewModelBase<FeedbackButtonTranslation>, IWhitelisted
    {
        public IGpoSettings GpoSettings { get; }

        public FeedbackButtonViewModel(ICommandLocator commandLocator, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            FeedbackCommand = commandLocator.GetCommand<FeedbackCommand>();
            PrioritySupportCommand = commandLocator.GetCommand<IPrioritySupportUrlOpenCommand>();
            GpoSettings = gpoSettings;
            HideFeedbackForm = gpoSettings.HideFeedbackForm;
        }

        public ICommand FeedbackCommand { get; }

        public ICommand PrioritySupportCommand { get; }

        public bool HideFeedbackForm { get; set; }
         
    }
}
