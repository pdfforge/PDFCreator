using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public class StoreLicenseForAllUsersWindowViewModel : InteractionAwareViewModelBase<StoreLicenseForAllUsersInteraction>
    {
        private readonly IOsHelper _osHelper;
        private readonly IUacAssistant _uacAssistant;
        private readonly IInteractionRequest _interactionRequest;
        public StoreLicenseForAllUsersWindowTranslation Translation { get; set; }
        public ICommand StoreLicenseInLmCommand { get; }
        public ICommand CloseCommand { get; }

        public string ProductName { get; }

        public Visibility RequiresUacVisibility
        {
            get { return _osHelper.UserIsAdministrator() ? Visibility.Collapsed : Visibility.Visible; }
        }

        public StoreLicenseForAllUsersWindowViewModel(ApplicationNameProvider applicationNameProvider, IOsHelper osHelper, IUacAssistant uacAssistant, IInteractionRequest interactionRequest, ITranslationUpdater translationUpdater)
        {
            _osHelper = osHelper;
            _uacAssistant = uacAssistant;
            _interactionRequest = interactionRequest;
            ProductName = applicationNameProvider.ApplicationNameWithEdition;

            StoreLicenseInLmCommand = new DelegateCommand(StoreLicenseInLmCommandExecute);
            CloseCommand = new DelegateCommand(o => FinishInteraction());

            translationUpdater.RegisterAndSetTranslation(tf => Translation = tf.UpdateOrCreateTranslation(Translation));
        }

        private void StoreLicenseInLmCommandExecute(object obj)
        {
            var success = _uacAssistant.StoreLicenseForAllUsers(Interaction.LicenseServerCode, Interaction.LicenseKey);
            if (success)
            {
                var text = Translation.StoreForAllUsersSuccessful;
                var interaction = new MessageInteraction(text, Title, MessageOptions.Ok, MessageIcon.PDFCreator);
                _interactionRequest.Raise(interaction);
            }
            else
            {
                var text = Translation.StoreForAllUsersFailed;
                var interaction = new MessageInteraction(text, Title, MessageOptions.Ok, MessageIcon.Error);
                _interactionRequest.Raise(interaction);
            }
            FinishInteraction();
        }

        public override string Title => ProductName.ToUpper();
    }
}
