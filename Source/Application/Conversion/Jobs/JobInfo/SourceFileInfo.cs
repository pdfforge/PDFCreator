using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    /// <summary>
    ///     SourceFileInfo holds data stored about a single source file, like name of the input file, printer name etc.
    /// </summary>
    public class SourceFileInfo
    {
        /// <summary>
        ///     The full path of the source file
        /// </summary>
        public string Filename { get; set; } = "";

        /// <summary>
        ///     The Windows Session Id
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        ///     The window station the job was created on (i.e. Console)
        /// </summary>
        public string WinStation { get; set; } = "";

        /// <summary>
        ///     The Author of the document
        /// </summary>
        public string Author { get; set; } = "";

        /// <summary>
        ///     Name of the computer on which the job was created
        /// </summary>
        public string ClientComputer { get; set; } = "";

        /// <summary>
        ///     Name of the printer
        /// </summary>
        public string PrinterName { get; set; } = "";

        /// <summary>
        ///     pdfcmon job counter
        /// </summary>
        public int JobCounter { get; set; }

        /// <summary>
        ///     ID of the Job as given from Windows printer
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        ///     The Title of the document
        /// </summary>
        public string DocumentTitle { get; set; } = "";

        /// <summary>
        ///     The path to the original file
        /// </summary>
        public string OriginalFilePath { get; set; } = "";

        public JobType Type { get; set; }

        /// <summary>
        ///     Number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        ///     Number of copies
        /// </summary>
        public int Copies { get; set; }

        /// <summary>
        ///     Flag if PS file was parsed for user tokens
        /// </summary>
        public bool UserTokenEvaluated { get; set; }

        /// <summary>
        ///     User tokens extracted from ps file if activated
        /// </summary>
        public UserToken UserToken { get; set; }

        /// <summary>
        /// Printer set via commandline for direct conversion
        /// </summary>
        public string PrinterParameter { get; set; }

        /// <summary>
        /// Profile set via commandline for direct conversion
        /// </summary>
        public string ProfileParameter { get; set; }

        /// <summary>
        /// Outputfile set via commandline for direct conversion
        /// </summary>
        public string OutputFileParameter { get; set; }
    }
}
