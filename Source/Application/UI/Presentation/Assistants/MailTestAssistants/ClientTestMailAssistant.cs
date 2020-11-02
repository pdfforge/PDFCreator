using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IClientTestMailAssistant
    {
        Task SendTestEmail(EmailClientSettings emailClientSettings);
    }

    public class ClientTestMailAssistant : TestMailAssistantBase<EmailClientSettings>, IClientTestMailAssistant
    {
        public ClientTestMailAssistant(ITranslationUpdater translationUpdater, ITokenHelper tokenHelper,
            ITestFileDummyHelper testFileDummyHelper, IEMailClientAction mailAction,
            ErrorCodeInterpreter errorCodeInterpreter, IInteractionRequest interactionRequest)
            : base(translationUpdater, tokenHelper, testFileDummyHelper, mailAction, errorCodeInterpreter, interactionRequest)
        { }

        protected override void SetMailActionSettings(ConversionProfile profile, EmailClientSettings emailClientSettings)
        {
            profile.EmailClientSettings = emailClientSettings;
        }

        protected override void ShowSuccess(Job job)
        {
            //Nothing to to here. The opened mail client ist the success response.
        }

        protected override Task<bool> TrySetJobPasswords(Job job)
        {
            return Task.FromResult(true);
        }

        public async Task SendTestEmail(EmailClientSettings emailClientSettings)
        {
            await SendTestEmail(emailClientSettings, new Accounts());
        }
    }
}
