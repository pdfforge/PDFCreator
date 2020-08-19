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

        private const int LargeListWarningLimit = 100;

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

        public void HandleFileList(IEnumerable<string> droppedFiles)
        {
            if (droppedFiles == null)
                return;

            var list = new List<(string path, AppStartParameters paramters)>();
            foreach (var droppedFile in droppedFiles)
            {
                list.Add((droppedFile, null));
            }
            HandleFileList(list);
        }

        /// <summary>
        ///     Removes invalid files and launches print jobs for the files that needs to be printed.
        ///     If successful, the direct convertable files are added to the current JobInfoQueue.
        /// </summary>
        public void HandleFileList(IEnumerable<(string path, AppStartParameters paramters)> droppedFiles)
        {
            if (droppedFiles == null)
                return;

            Logger.Debug("Launched Drag & Drop");
            var files = GetExistingFiles(droppedFiles);

            if (files.Count > LargeListWarningLimit)
            {
                var message = _translation.GetFormattedMoreThanXFilesQuestion(files.Count);
                var interaction = new MessageInteraction(message, "PDFCreator", MessageOptions.YesNo, MessageIcon.Question);
                _interactionInvoker.Invoke(interaction);

                if (interaction.Response != MessageResponse.Yes)
                    return;
            }
            HandleFiles(files);
        }

        private List<(string path, AppStartParameters paramters)> GetExistingFiles(IEnumerable<(string path, AppStartParameters paramters)> droppedFiles)
        {
            var validFiles = new List<(string path, AppStartParameters paramters)>();
            foreach (var pathTuple in droppedFiles)
            {
                if (_file.Exists(pathTuple.path))
                {
                    validFiles.Add(pathTuple);
                }
                else if (_directory.Exists(pathTuple.path))
                {
                    var directoryFiles = _directory.GetFiles(pathTuple.path);
                    foreach (var file in directoryFiles)
                    {
                        validFiles.Add((file, pathTuple.paramters));
                    }
                }
                else
                {
                    Logger.Warn("The file " + pathTuple.path + " does not exist.");
                }
            }

            return validFiles;
        }

        /// <summary>
        ///     Launches a print job for all dropped files that can be printed.
        ///     Return false if cancelled because of unprintable files
        /// </summary>
        private void PrintPrintableFiles(IList<string> printFiles)
        {
            if (!_printFileHelper.AddFiles(printFiles))
                return;
            _storedParametersManager.SaveParameterSettings("", "", printFiles.FirstOrDefault());
            _printFileHelper.PrintAll();
        }

        private void HandleFiles(IList<(string path, AppStartParameters paramters)> droppedFiles)
        {
            var directConversionFiles = new List<(string path, AppStartParameters paramters)>();
            var printFiles = new List<string>();
            foreach (var file in droppedFiles)
            {
                if (_directConversion.CanConvertDirectly(file.path))
                    directConversionFiles.Add(file);
                else
                    printFiles.Add(file.path);
            }

            foreach (var directConversionFile in directConversionFiles)
            {
                _directConversion.ConvertDirectly(directConversionFile.Item1, directConversionFile.Item2);
            }

            if (printFiles.Any())
                PrintPrintableFiles(printFiles);
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
