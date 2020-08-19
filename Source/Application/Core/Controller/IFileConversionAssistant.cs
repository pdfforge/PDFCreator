using pdfforge.PDFCreator.Core.DirectConversion;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Controller
{
    public interface IFileConversionAssistant
    {
        /// <summary>
        ///     Removes invalid files and launches print jobs for the files that needs to be printed.
        ///     If successful, the direct convertable files are added to the current JobInfoQueue.
        /// </summary>
        void HandleFileList(IEnumerable<(string path, AppStartParameters paramters)> droppedFiles);

        void HandleFileList(IEnumerable<string> droppedFiles);
    }
}
