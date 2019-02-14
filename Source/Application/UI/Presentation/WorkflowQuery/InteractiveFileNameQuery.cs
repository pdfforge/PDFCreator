using NLog;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.WorkflowQuery
{
    public class InteractiveFileNameQuery : IRetypeFileNameQuery, IFileNameQuery
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private InteractiveWorkflowTranslation _translation;
        private readonly IDirectoryHelper _directoryHelper;

        public InteractiveFileNameQuery(ITranslationUpdater translationUpdater, IInteractionInvoker interactionInvoker, IDirectoryHelper directoryHelper)
        {
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
            _interactionInvoker = interactionInvoker;
            _directoryHelper = directoryHelper;
        }

        public QueryResult<OutputFilenameResult> GetFileName(string directory, string filename, OutputFormat outputFormat)
        {
            var interaction = CreateFileNameInteraction(filename, directory, outputFormat);
            var result = InvokeInteraction(interaction, outputFormat, true);

            return result;
        }

        public QueryResult<string> RetypeFileNameQuery(string filename, OutputFormat outputFormat, RetypeReason retypeReason)
        {
            NotifyUserAboutFailure(filename, retypeReason);

            var interaction = CreateRetypeInteraction(filename, outputFormat);

            var result = InvokeInteraction(interaction, outputFormat, false);

            var newFilename = result.Success ? result.Data.Filepath : "";

            return new QueryResult<string>(result.Success, newFilename);
        }

        private SaveFileInteraction CreateFileNameInteraction(string fileName, string directory, OutputFormat outputFormat)
        {
            if (!string.IsNullOrWhiteSpace(directory))
            {
                var success = CreateTargetDirectory(directory);

                if (!success)
                {
                    _logger.Warn("Could not create directory for save file dialog. It will be opened with default save location.");
                    directory = "";
                }
            }

            var interaction = new SaveFileInteraction();

            interaction.Title = _translation.SelectDestination;
            interaction.Filter = GetAllFilters();
            interaction.FilterIndex = (int)outputFormat + 1;
            interaction.OverwritePrompt = true;
            interaction.ForceTopMost = true;
            interaction.FileName = fileName;
            interaction.InitialDirectory = directory;

            return interaction;
        }

        private bool CreateTargetDirectory(string saveDirectory)
        {
            return _directoryHelper.CreateDirectory(saveDirectory);
        }

        private QueryResult<OutputFilenameResult> InvokeInteraction(SaveFileInteraction interaction, OutputFormat outputFormat, bool canChangeFormat)
        {
            var result = new QueryResult<OutputFilenameResult> { Success = false };

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return result;

            if (canChangeFormat)
            {
                outputFormat = (OutputFormat)interaction.FilterIndex - 1;
            }

            var outputFile = interaction.FileName;
            var outputFormatHelper = new OutputFormatHelper();
            if (!outputFormatHelper.HasValidExtension(outputFile, outputFormat))
                outputFile = outputFormatHelper.EnsureValidExtension(outputFile, outputFormat);

            result.Success = true;
            result.Data = new OutputFilenameResult(outputFile, outputFormat);
            return result;
        }

        private SaveFileInteraction CreateRetypeInteraction(string filename, OutputFormat outputFormat)
        {
            var interaction = new SaveFileInteraction();

            interaction.Title = _translation.SelectDestination;

            interaction.Filter = GetFilterForOutputFormat(outputFormat);
            interaction.FilterIndex = 1;

            interaction.OverwritePrompt = true;
            var currentFileName = filename;
            interaction.FileName = Path.GetFileName(currentFileName);
            interaction.InitialDirectory = Path.GetDirectoryName(currentFileName);
            interaction.ForceTopMost = true;

            return interaction;
        }

        private string GetAllFilters()
        {
            var formatStrings = Enum.GetValues(typeof(OutputFormat))
                .Cast<OutputFormat>()
                .Select(GetFilterForOutputFormat);

            return string.Join("|", formatStrings);
        }

        private string GetFilterForOutputFormat(OutputFormat outputFormat)
        {
            switch (outputFormat)
            {
                case OutputFormat.Pdf:
                    return _translation.PdfFile + @" (*.pdf)|*.pdf";

                case OutputFormat.PdfA1B:
                    return _translation.PdfA1bFile + @" (*.pdf)|*.pdf";

                case OutputFormat.PdfA2B:
                    return _translation.PdfA2bFile + @" (*.pdf)|*.pdf";

                case OutputFormat.PdfA3B:
                    return _translation.PdfA3bFile + @" (*.pdf)|*.pdf";

                case OutputFormat.PdfX:
                    return _translation.PdfXFile + @" (*.pdf)|*.pdf";

                case OutputFormat.Jpeg:
                    return _translation.JpegFile + @" (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                case OutputFormat.Png:
                    return _translation.PngFile + @" (*.png)|*.png";

                case OutputFormat.Tif:
                    return _translation.TiffFile + @" (*.tif;*.tiff)|*.tif;*.tiff";
                case OutputFormat.Txt:
                    return _translation.TextFile + @" (*.txt)|*.txt";

                default:
                    throw new ArgumentOutOfRangeException(nameof(outputFormat), outputFormat, null);
            }
        }

        private void NotifyUserAboutFailure(string currentFileName, RetypeReason retypeReason)
        {
            var message = "";
            switch (retypeReason)
            {
                case RetypeReason.InvalidRootedPath:
                    message = _translation.FormatInvalidRootedPathMessage(currentFileName);
                    break;

                case RetypeReason.CopyError:
                    message = _translation.FormatCopyErrorMessage(currentFileName);
                    break;
            }

            const string title = "PDFCreator";

            ShowMessage(message, title, MessageOptions.OK, MessageIcon.Warning);
        }

        private void ShowMessage(string text, string title, MessageOptions buttons, MessageIcon icon)
        {
            var messageInteraction = new MessageInteraction(text, title, buttons, icon);
            _interactionInvoker.Invoke(messageInteraction);
        }
    }
}
