using pdfforge.PDFCreator.Core.DirectConversion;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Controller
{
    public interface IFileConversionAssistant
    {
        /// <summary>
        ///     Removes invalid files and launches print jobs for the files that needs to be printed.
        ///     If successful, the direct convertible files are added to the current JobInfoQueue.
        /// </summary>
        void HandleFileList(IEnumerable<string> droppedFiles, AppStartParameters appStartParameters);

        void HandleFileListWithoutTooManyFilesWarning(IEnumerable<string> droppedFiles, AppStartParameters appStartParameters);
    }
}
