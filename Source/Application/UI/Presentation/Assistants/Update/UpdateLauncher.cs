using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.Web;
using Prism.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public interface IUpdateLauncher
    {
        Task LaunchUpdateAsync(IApplicationVersion version);
    }

    public class DisabledUpdateLauncher : IUpdateLauncher
    {
        public Task LaunchUpdateAsync(IApplicationVersion version)
        {
            return Task.FromResult((object)null);
        }
    }

    public class SimpleUpdateLauncher : IUpdateLauncher
    {
        private readonly IWebLinkLauncher _webLinkLauncher;

        public SimpleUpdateLauncher(IWebLinkLauncher webLinkLauncher)
        {
            _webLinkLauncher = webLinkLauncher;
        }

        public Task LaunchUpdateAsync(IApplicationVersion version)
        {
            _webLinkLauncher.Launch(Urls.PDFCreatorDownloadUrl);
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
        private readonly IUpdateDownloader _updateDownloader;
        private readonly IEventAggregator _eventAggregator;
        private UpdateManagerTranslation _translation;

        public AutoUpdateLauncher(ITranslationFactory translationFactory, IInteractionInvoker interactionInvoker, IInteractionRequest interactionRequest, IHashUtil hashUtil, IThreadManager threadManager, ApplicationNameProvider applicationNameProvider, IUpdateDownloader updateDownloader, IEventAggregator EventAggregator)
        {
            UpdateTranslation(translationFactory);
            translationFactory.TranslationChanged += (sender, args) => UpdateTranslation(translationFactory);
            _interactionRequest = interactionRequest;
            _interactionInvoker = interactionInvoker;
            _hashUtil = hashUtil;
            _threadManager = threadManager;
            _applicationNameProvider = applicationNameProvider;
            _updateDownloader = updateDownloader;
            _eventAggregator = EventAggregator;
        }

        public async Task LaunchUpdateAsync(IApplicationVersion version)
        {
            var caption = _translation.GetFormattedTitle(_applicationNameProvider.ApplicationName);
            var downloadPath = _updateDownloader.GetDownloadPath(version.DownloadUrl);

            // retry until successful
            if (!_updateDownloader.IsDownloaded(downloadPath))
            {
                while (!TryLaunchUpdate(version, caption, downloadPath))
                { }
            }

            await AskUserForAppRestartAsync(downloadPath);
        }

        private bool TryLaunchUpdate(IApplicationVersion version, string caption, string filePath)
        {
            try
            {
                var interaction = new UpdateDownloadInteraction(async () => await _updateDownloader.StartDownloadAsync(version));
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
            catch (Exception)
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

        private async Task AskUserForAppRestartAsync(string downloadedFile)
        {
            await ShowAppRestartMessageAsync(downloadedFile);
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

        private async Task ShowAppRestartMessageAsync(string downloadedFile)
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
