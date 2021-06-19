using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class DragAndDropEventHandler
    {
        private readonly IFileConversionAssistant _fileConversionAssistant;

        public DragAndDropEventHandler(IFileConversionAssistant fileConversionAssistant)
        {
            _fileConversionAssistant = fileConversionAssistant;
        }

        /// <summary>
        ///     Sets the DragDropEffect to Copy for a FileDrop
        /// </summary>
        public void HandleDragEnter(DragEventArgs e)
        {
            var strings = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (strings != null && strings.Length == 0)
                e.Effects = DragDropEffects.None;
            else
                e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        ///     Removes invalid files, adds the files that do not need to be printed to the current JobInfoQueue
        ///     and launches print jobs for the remaining files.
        /// </summary>
        public void HandleDropEvent(DragEventArgs e)
        {
            var droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            _fileConversionAssistant.HandleFileListWithoutTooManyFilesWarning(droppedFiles, new AppStartParameters());
        }
    }
}
