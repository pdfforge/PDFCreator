using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Update;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using Prism.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public interface IUpdateLauncher
    {
        Task LaunchUpdate(IApplicationVersion version);
    }

    public class DisabledUpdateLauncher : IUpdateLauncher
    {
        public Task LaunchUpdate(IApplicationVersion version)
        {
            return Task.FromResult((object)null);
        }
    }

    public class SimpleUpdateLauncher : IUpdateLauncher
    {
        public Task LaunchUpdate(IApplicationVersion version)
        {
            Process.Start(Urls.PDFCreatorDownloadUrl);
            return Task.FromResult((object)null);
        }
    }

    public class AutoUpdateLauncher : IUpdateLauncher
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IHashUtil _hashUtil;
        private readonly IThreadManager _threadManager;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IUpdateHelper _updateHelper;
        private readonly IEventAggregator _eventAggregator;
        private UpdateManagerTranslation _translation;

        public AutoUpdateLauncher(ITranslationFactory translationFactory, IInteractionInvoker interactionInvoker, IInteractionRequest interactionRequest, IHashUtil hashUtil, IThreadManager threadManager, ApplicationNameProvider applicationNameProvider, IUpdateHelper updateHelper, IEventAggregator EventAggregator)
        {
            UpdateTranslation(translationFactory);
            translationFactory.TranslationChanged += (sender, args) => UpdateTranslation(translationFactory);
            _interactionRequest = interactionRequest;
            _interactionInvoker = interactionInvoker;
            _hashUtil = hashUtil;
            _threadManager = threadManager;
            _applicationNameProvider = applicationNameProvider;
            _updateHelper = updateHelper;
            _eventAggregator = EventAggregator;
        }

        public async Task LaunchUpdate(IApplicationVersion version)
        {
            var caption = _translation.GetFormattedTitle(_applicationNameProvider.ApplicationName);
            var downloadPath = _updateHelper.GetDownloadPath(version.DownloadUrl);

            // retry until successful
            if (!_updateHelper.IsDownloaded(downloadPath))
            {
                while (!TryLaunchUpdate(version, caption, downloadPath))
                { }
            }

            await AskUserForAppRestart(downloadPath);
        }

        private bool TryLaunchUpdate(IApplicationVersion version, string caption, string filePath)
        {
            try
            {
                var interaction = new UpdateDownloadInteraction(() => _updateHelper.StartDownload(version));
                _interactionInvoker.Invoke(interaction);

                if (interaction.Success)
                {
                    // Hash test
                    if (!_hashUtil.VerifyFileMd5(filePath, version.FileHash))
                    {
                        //Hash does not fit should we retry download
                        var message = _translation.DownloadHashErrorMessage;
                        var res = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Warning);

                        if (res == MessageResponse.Yes) // Yes please retry download: Download was not successful
                            return false;
                    }
                }
            }
            catch (Exception e)
            {
                var message = _translation.DownloadErrorMessage;
                var res = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.PDFCreator);

                if (res == MessageResponse.Yes)
                {
                    Process.Start(version.DownloadUrl);
                }
            }

            return true; // Download was successful
        }

        private async Task AskUserForAppRestart(string downloadedFile)
        {
            await ShowAppRestartMessage(downloadedFile);
        }

        private void LaunchDownloadedFile(string filename)
        {
            try
            {
                Process.Start(filename);
            }
            catch
            {
                // Do nothing when this fails
            }
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }

        private async Task ShowAppRestartMessage(string downloadedFile)
        {
            var interaction = new RestartApplicationInteraction();
            await _interactionRequest.RaiseAsync(interaction);
            EvaluateUserInputAppRestartMessage(interaction, downloadedFile);
        }

        private void EvaluateUserInputAppRestartMessage(RestartApplicationInteraction interaction, string downloadedFile)
        {
            switch (interaction.InteractionResult)
            {
                case RestartApplicationInteractionResult.Now:
                    _threadManager.UpdateAfterShutdownAction = () => LaunchDownloadedFile(downloadedFile);
                    _eventAggregator.GetEvent<TryCloseApplicationEvent>().Publish();
                    break;

                case RestartApplicationInteractionResult.Later:
                    _threadManager.UpdateAfterShutdownAction = () => LaunchDownloadedFile(downloadedFile);
                    break;

                case RestartApplicationInteractionResult.Cancel:
                    break;
            }
        }

        private void UpdateTranslation(ITranslationFactory translationFactory)
        {
            _translation = translationFactory.CreateTranslation<UpdateManagerTranslation>();
        }
    }
}
