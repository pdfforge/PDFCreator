using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface ISmtpTest
    {
        Task SendTestMail(ConversionProfile profile, Accounts accounts);
    }

    public class SmtpTestEmailAssistant : ISmtpTest
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IFile _file;
        private readonly IPath _path;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ITokenHelper _tokenHelper;
        private readonly ISmtpMailAction _smtpMailAction;
        private SmtpTranslation _translation;

        public SmtpTestEmailAssistant(ITranslationUpdater translationUpdater, IInteractionRequest interactionRequest, IFile file,
            ISmtpMailAction smtpMailAction, IPath path, ErrorCodeInterpreter errorCodeInterpreter,
            IInteractionInvoker interactionInvoker, ITokenHelper tokenHelper)
        {
            _interactionRequest = interactionRequest;
            _file = file;
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
            _smtpMailAction = smtpMailAction;
            _path = path;
            _errorCodeInterpreter = errorCodeInterpreter;
            _interactionInvoker = interactionInvoker;
            _tokenHelper = tokenHelper;
        }

        private string GetRecipientsString(EmailSmtpSettings smtpSettings, TokenReplacer tokenReplacer)
        {
            var recipientsTo = tokenReplacer.ReplaceTokens(smtpSettings.Recipients);
            var recipientsCc = tokenReplacer.ReplaceTokens(smtpSettings.RecipientsCc);
            var recipientsBcc = tokenReplacer.ReplaceTokens(smtpSettings.RecipientsBcc);

            var sb = new StringBuilder();
            sb.AppendLine($"{_translation.RecipientsToText} {recipientsTo}");

            if (!string.IsNullOrWhiteSpace(recipientsCc))
                sb.AppendLine($"{_translation.RecipientsCcText} {recipientsCc}");

            if (!string.IsNullOrWhiteSpace(recipientsBcc))
                sb.AppendLine($"{_translation.RecipientsBccText} {recipientsBcc}");

            return sb.ToString();
        }

        public async Task SendTestMail(ConversionProfile profile, Accounts accounts)
        {
            var currentProfile = profile.Copy();

            currentProfile.AutoSave.Enabled = false;

            var job = CreateJob(currentProfile, accounts);

            _smtpMailAction.ApplyPreSpecifiedTokens(job);
            var result = _smtpMailAction.Check(job.Profile, job.Accounts, CheckLevel.Job);
            if (!result)
            {
                DisplayResult(result, job);
                return;
            }

            if (!TrySetJobPasswords(job, profile))
                return;

            var testFile = _path.Combine(_path.GetTempPath(), _translation.AttachmentFile + ".pdf");
            _file.WriteAllText(testFile, @"PDFCreator", Encoding.GetEncoding("Unicode"));
            job.OutputFiles.Add(testFile);

            result = await RunSmtpAction(job);
            DisplayResult(result, job);

            _file.Delete(testFile);
        }

        private Task<ActionResult> RunSmtpAction(Job job)
        {
            return Task.Run(() => _smtpMailAction.ProcessJob(job));
        }

        private bool TrySetJobPasswords(Job job, ConversionProfile profile)
        {
            var smtpAccount = job.Accounts.GetSmtpAccount(profile);
            if (smtpAccount == null)
            {
                var message = new MessageInteraction(_translation.NoAccount, "PDFCreator", MessageOptions.OK, MessageIcon.Error);
                _interactionRequest.Raise(message);
                return false;
            }

            job.Passwords.SmtpPassword = smtpAccount.Password;

            if (!string.IsNullOrWhiteSpace(job.Passwords.SmtpPassword))
                return true;

            var recipientsString = GetRecipientsString(job.Profile.EmailSmtpSettings, job.TokenReplacer);

            var title = _translation.SetSmtpServerPassword;
            var description = _translation.SmtpServerPasswordLabel;

            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.None, title, description, false);
            interaction.IntroText = recipientsString;

            // TODO:Use IInteractionRequest
            _interactionInvoker.Invoke(interaction);

            if (interaction.Result != PasswordResult.StorePassword)
                return false;

            job.Passwords.SmtpPassword = interaction.Password;
            return true;
        }

        private Job CreateJob(ConversionProfile currentProfile, Accounts accounts)
        {
            var jobInfo = new JobInfo();
            var job = new Job(jobInfo, currentProfile, accounts);
            job.JobInfo.Metadata = new Metadata();
            job.JobInfo.SourceFiles.Add(new SourceFileInfo { Filename = "test.ps" });
            job.TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            return job;
        }

        private void DisplayResult(ActionResult actionResult, Job job)
        {
            if (actionResult.IsSuccess)
            {
                var title = _translation.SendTestMail;
                var message = _translation.TestMailSent + "\n" + GetRecipientsString(job.Profile.EmailSmtpSettings, job.TokenReplacer);
                DisplayMessage(message, title, MessageIcon.Info);
            }
            else
            {
                DisplayErrorMessage(actionResult);
            }
        }

        private void DisplayErrorMessage(ActionResult actionResult)
        {
            var title = _translation.SendTestMail;
            var message = GetErrorMessage(actionResult.First());
            DisplayMessage(message, title, MessageIcon.Error);
        }

        private string GetErrorMessage(ErrorCode errorCode)
        {
            return _errorCodeInterpreter.GetFirstErrorText(new ActionResult(errorCode), withNumber: false);
        }

        private void DisplayMessage(string message, string title, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, MessageOptions.OK, icon);
            _interactionRequest.Raise(interaction);
        }
    }
}
