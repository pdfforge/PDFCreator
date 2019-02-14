using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.UI.COM
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("E0F6B8E7-8E89-400C-B623-67EFB0A7A9A0")]
    public interface IPrintJobInfo
    {
        string PrintJobName { get; set; }
        string PrintJobAuthor { get; set; }
        string Subject { get; set; }
        string Keywords { get; set; }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class PrintJobInfo : IPrintJobInfo
    {
        private readonly Metadata _metadata;

        internal PrintJobInfo(Metadata metadata)
        {
            _metadata = metadata;
        }

        /// <summary>
        ///     Title from PrintJob
        /// </summary>
        public string PrintJobName
        {
            get { return _metadata.PrintJobName; }
            set { _metadata.PrintJobName = value; }
        }

        /// <summary>
        ///     Author from PrintJob
        /// </summary>
        public string PrintJobAuthor
        {
            get { return _metadata.PrintJobAuthor; }
            set { _metadata.PrintJobAuthor = value; }
        }

        /// <summary>
        ///     Subject of the document
        /// </summary>
        public string Subject
        {
            get { return _metadata.Subject; }
            set { _metadata.Subject = value; }
        }

        /// <summary>
        ///     Keywords that describe the document
        /// </summary>
        public string Keywords
        {
            get { return _metadata.Keywords; }
            set { _metadata.Keywords = value; }
        }
    }
}
