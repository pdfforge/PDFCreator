using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Printing.Printing
{
    public interface IPrintFileHelper
    {
        string PdfCreatorPrinter { get; set; }

        /// <summary>
        ///     Add a single file. The file is checked and dialogs are presented to the user, if there are problems.
        /// </summary>
        /// <param name="file">
        ///     A single file. If this is the path of a directory or an unprintable file, an error message will be
        ///     shown.
        /// </param>
        /// <returns>true, if all files are printable</returns>
        bool AddFile(string file, bool silent);

        /// <summary>
        ///     Add multiple files. The files are checked - if there are problems, abstract functions (that might present dialogs
        ///     to the user) get called.
        /// </summary>
        /// <param name="files">
        ///     A list of files. If this contains a directory or files are not printable, an error message will be
        ///     shown.
        /// </param>
        /// <returns>true, if all files are printable</returns>
        bool AddFiles(IEnumerable<string> files, bool silent);

        /// <summary>
        ///     Prints all files in the list.
        /// </summary>
        /// <returns>true, if all files could be printed</returns>
        bool PrintAll(bool silent);
    }
}
