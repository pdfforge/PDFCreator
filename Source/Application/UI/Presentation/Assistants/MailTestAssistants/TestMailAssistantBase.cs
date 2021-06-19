using NLog;
using pdfforge.Obsidian.Trigger;
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
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public abstract class TestMailAssistantBase<TMailActionSettings> where TMailActionSettings : IMailActionSettings
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected MailTranslation Translation;
        private readonly ITestFileDummyHelper _testFileDummyHelper;
        private readonly IAction _mailAction;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        protected IInteractionRequest InteractionRequest { get; }
        private readonly ITokenHelper _tokenHelper;

        protected TestMailAssistantBase(ITranslationUpdater translationUpdater, ITokenHelper tokenHelper,
                                        ITestFileDummyHelper testFileDummyHelper, IAction mailAction,
                                        ErrorCodeInterpreter errorCodeInterpreter, IInteractionRequest interactionRequest)
        {
            translationUpdater.RegisterAndSetTranslation(tf => Translation = tf.UpdateOrCreateTranslation(Translation));
            _testFileDummyHelper = testFileDummyHelper;
            _mailAction = mailAction;
            _errorCodeInterpreter = errorCodeInterpreter;
            InteractionRequest = interactionRequest;
            _tokenHelper = tokenHelper;
        }

        protected abstract void SetMailActionSettings(ConversionProfile profile, TMailActionSettings mailActionSettings);

        protected abstract void ShowSuccess(Job job);

        protected abstract Task<bool> TrySetJobPasswords(Job job);

        protected async Task SendTestEmail(TMailActionSettings mailActionSettings, Accounts accounts)
        {
            try
            {
                var job = CreateTestMailJob(mailActionSettings, accounts);
                _mailAction.ApplyRestrictions(job);
                _mailAction.ApplyPreSpecifiedTokens(job);

                if (!await TrySetJobPasswords(job))
                    return;

                var currentCheckSettings = new CurrentCheckSettings(job.AvailableProfiles, job.PrinterMappings, job.Accounts);
                var result = _mailAction.Check(job.Profile, currentCheckSettings, CheckLevel.RunningJob);

                if (result)
                    result = await Task.Run(() => _mailAction.ProcessJob(job));

                if (result)
                    ShowSuccess(job);
                else
                    ShowErrorMessage(result);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Exception while sending test mail");
            }
            finally
            {
                _testFileDummyHelper.CleanUp();
            }
        }

        private Job CreateTestMailJob(TMailActionSettings mailActionSettings, Accounts accounts)
        {
            var mailSettingsCopy = (TMailActionSettings)mailActionSettings.Copy();

            mailSettingsCopy.Enabled = true;
            var numberOfFiles = mailSettingsCopy.AdditionalAttachments.Count;
            var additionalAttachmentDummies = _testFileDummyHelper.CreateFileList(Translation.AdditionalAttachmentFile, "pdf", numberOfFiles);
            mailSettingsCopy.AdditionalAttachments = additionalAttachmentDummies;

            var profile = new ConversionProfile();
            profile.AutoSave.Enabled = false;
            SetMailActionSettings(profile, mailSettingsCopy);

            var currentSettings = new CurrentJobSettings(new[] { profile }, new List<PrinterMapping>(), accounts);

            var job = new Job(new JobInfo(), profile, currentSettings);
            job.JobInfo.Metadata = new Metadata();
            job.JobInfo.SourceFiles.Add(new SourceFileInfo { Filename = "MailTest.ps" });
            job.TokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            var documentFileDummy = _testFileDummyHelper.CreateFile(Translation.DocumentFile, "pdf");
            job.OutputFiles.Add(documentFileDummy);

            return job;
        }

        private void ShowErrorMessage(ActionResult result)
        {
            var title = Translation.SendTestMail;
            var message = _errorCodeInterpreter.GetFirstErrorText(result, withNumber: false);
            var interaction = new MessageInteraction(message, title, MessageOptions.OK, MessageIcon.Error);

            InteractionRequest.Raise(interaction);
        }
    }
}
