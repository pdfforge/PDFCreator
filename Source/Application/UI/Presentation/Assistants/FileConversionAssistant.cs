using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class FileConversionAssistant : IFileConversionAssistant
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDirectConversion _directConversion;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IStoredParametersManager _storedParametersManager;
        private readonly IPrintFileHelper _printFileHelper;
        private FileConversionAssistantTranslation _translation = new FileConversionAssistantTranslation();

        private const int LargeListWarningLimit = 40;

        public FileConversionAssistant(IDirectConversion directConversion,
            IPrintFileHelper printFileHelper,
            IFile file,
            IDirectory directory,
            IInteractionInvoker interactionInvoker,
            ITranslationUpdater translationUpdater,
            IStoredParametersManager storedParametersManager)
        {
            _directConversion = directConversion;
            _printFileHelper = printFileHelper;
            _file = file;
            _directory = directory;
            _interactionInvoker = interactionInvoker;
            _storedParametersManager = storedParametersManager;

            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
        }

        public void HandleFileListWithoutTooManyFilesWarning(IEnumerable<string> droppedFiles, AppStartParameters appStartParameters)
        {
            appStartParameters.Silent = true;
            HandleFileList(droppedFiles, appStartParameters);
        }

        /// <summary>
        ///     Removes invalid files and launches print jobs for the files that needs to be printed.
        ///     If successful, the direct convertable files are added to the current JobInfoQueue.
        /// </summary>
        public void HandleFileList(IEnumerable<string> droppedFiles, AppStartParameters appStartParameters)
        {
            if (droppedFiles == null)
                return;

            Logger.Debug("Launched Drag & Drop");
            var existingFiles = GetExistingFiles(droppedFiles);

            if (!appStartParameters.Silent && existingFiles.Count > LargeListWarningLimit)
            {
                var message = _translation.GetFormattedMoreThanXFilesQuestion(existingFiles.Count);
                var interaction = new MessageInteraction(message, "PDFCreator", MessageOptions.YesNo, MessageIcon.Question);
                _interactionInvoker.Invoke(interaction);

                if (interaction.Response != MessageResponse.Yes)
                    return;
            }

            HandleFiles(existingFiles, appStartParameters);
        }

        private List<string> GetExistingFiles(IEnumerable<string> droppedFiles)
        {
            var existingFiles = new List<string>();
            foreach (var droppedFile in droppedFiles)
            {
                if (_file.Exists(droppedFile))
                {
                    existingFiles.Add(droppedFile);
                }
                else if (_directory.Exists(droppedFile))
                {
                    var directoryFiles = _directory.GetFiles(droppedFile);
                    foreach (var file in directoryFiles)
                    {
                        existingFiles.Add(file);
                    }
                }
                else
                {
                    Logger.Warn("The file or directory " + droppedFile + " does not exist.");
                }
            }

            return existingFiles;
        }

        /// <summary>
        ///     Launches a print job for all dropped files that can be printed.
        ///     Return false if cancelled because of unprintable files
        /// </summary>
        private void PrintPrintableFiles(IList<string> printFiles, AppStartParameters appStartParameters)
        {
            if (!_printFileHelper.AddFiles(printFiles, appStartParameters.Silent))
                return;
            _storedParametersManager.SaveParameterSettings(appStartParameters.OutputFile, appStartParameters.Profile, printFiles.FirstOrDefault());
            _printFileHelper.PrintAll(appStartParameters.Silent);
        }

        private void HandleFiles(IEnumerable<string> droppedFiles, AppStartParameters appStartParameters)
        {
            var directConversionFiles = new List<string>();
            var printFiles = new List<string>();
            foreach (var file in droppedFiles)
            {
                if (_directConversion.CanConvertDirectly(file))
                    directConversionFiles.Add(file);
                else
                    printFiles.Add(file);
            }

            var directConversionFilesList = new List<string>();
            foreach (var directConversionFile in directConversionFiles)
            {
                if (appStartParameters != null && appStartParameters.Merge)
                    directConversionFilesList.Add(directConversionFile);
                else
                    _directConversion.ConvertDirectly(new List<string>() { directConversionFile }, appStartParameters);
            }

            if (directConversionFilesList.Count > 0)
                _directConversion.ConvertDirectly(directConversionFilesList, appStartParameters);

            if (printFiles.Any())
                PrintPrintableFiles(printFiles, appStartParameters);
        }
    }
}

public class FileConversionAssistantTranslation : ITranslatable
{
    private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
    private string[] MoreThanXFilesQuestion { get; set; } = { "Do you want to convert the {0} selected file?", "Do you want to convert the {0} selected files?" };

    public string GetFormattedMoreThanXFilesQuestion(int numberOfFiles)
    {
        return PluralBuilder.GetFormattedPlural(numberOfFiles, MoreThanXFilesQuestion);
    }
}
