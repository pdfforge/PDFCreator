using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using SystemInterface.IO;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IBrowseFileCommandBuilder
    {
        void Init(Func<Job> getJob, Action updateUi, Func<string> getLastConfirmedPath, Action<string> setLastConfirmedPath);

        IMacroCommand BuildCommand(Predicate<object> canExecute = null);
    }

    public class BrowseFileCommandBuilder : IBrowseFileCommandBuilder
    {
        private BrowseFileCommandTranslation _translation;

        private readonly IDirectoryHelper _directoryHelper;
        private readonly IFileNameQuery _fileNameQuery;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ITranslationFactory _translationFactory;

        private Action _updateUi;
        private Func<Job> _getJob;
        private Func<string> _getLastConfirmedPath;
        private Action<string> _setLastConfirmedPath;

        public BrowseFileCommandBuilder(IDirectoryHelper directoryHelper, IFileNameQuery fileNameQuery, IInteractionRequest interactionRequest, ITranslationFactory translationFactory)
        {
            _directoryHelper = directoryHelper;
            _fileNameQuery = fileNameQuery;
            _interactionRequest = interactionRequest;
            _translationFactory = translationFactory;
        }

        public void Init(Func<Job> getJob, Action updateUi, Func<string> getLastConfirmedPath, Action<string> setLastConfirmedPath)
        {
            _updateUi = updateUi;
            _getJob = getJob;
            _setLastConfirmedPath = setLastConfirmedPath;
            _getLastConfirmedPath = getLastConfirmedPath;
        }

        public IMacroCommand BuildCommand(Predicate<object> canExecute = null)
        {
            if (_getJob == null || _updateUi == null || _setLastConfirmedPath == null || _getLastConfirmedPath == null)
                throw new InvalidOperationException($"Call {nameof(BrowseFileCommandBuilder)}.Init first to set communication functions.");

            var waitableAsyncCommand = new WaitableAsyncCommand(BrowseFileWithNotificationForTooLongInput, canExecute);

            return new MacroCommand(new List<ICommand> { waitableAsyncCommand });
        }

        private async Task<MacroCommandIsDoneEventArgs> BrowseFileWithNotificationForTooLongInput(object arg)
        {
            var job = _getJob();
            var inputFilePath = job.OutputFileTemplate;

            var inputDirectory = PathSafe.GetDirectoryName(inputFilePath);
            var inputFilename = PathSafe.GetFileName(inputFilePath);

            _directoryHelper.CreateDirectory(inputDirectory);

            var result = await GetFileOrRetry(inputDirectory, inputFilename, job.Profile.OutputFormat);

            if (result.Success)
            {
                job.OutputFileTemplate = result.Data.Filepath;
                job.Profile.OutputFormat = result.Data.OutputFormat;
                _updateUi();
                _setLastConfirmedPath(result.Data.Filepath);
                return new MacroCommandIsDoneEventArgs(ResponseStatus.Success);
            }

            _setLastConfirmedPath("");
            return new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel);
        }

        private async Task<QueryResult<OutputFilenameResult>> GetFileOrRetry(string dir, string file, OutputFormat format)
        {
            while (true)
            {
                try
                {
                    return _fileNameQuery.GetFileName(dir, file, format);
                }
                catch (PathTooLongException)
                {
                    _translation = _translationFactory.UpdateOrCreateTranslation(_translation);
                    var interaction = new MessageInteraction(_translation.PathTooLongText, _translation.PathTooLongTitle, MessageOptions.OK, MessageIcon.Exclamation);
                    await _interactionRequest.RaiseAsync(interaction);
                }
            }
        }
    }
}
