using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    /// <summary>
    ///     The JobInfo class holds all information about the job, like the source files, metadata and such.
    /// </summary>
    public class JobInfo
    {
        public JobInfo()
        {
            Metadata = new Metadata();
        }

        public string InfFile { get; set; }

        public bool ShowMergedFiles => SourceFiles.Count > 1;

        public ObservableCollection<SourceFileInfo> SourceFiles { get; set; } = new ObservableCollection<SourceFileInfo>();

        /// <summary>
        ///     Sum of TotalPages of all SourceFiles
        /// </summary>
        public int TotalPages => CalculateTotalPages();

        public Metadata Metadata { get; set; }
        public JobType JobType { get; set; }
        public DateTime PrintDateTime { get; set; }
        public string PrinterName { get; set; }
        public string PrinterParameter { get; set; }
        public string ProfileParameter { get; set; }
        public string OutputFileParameter { get; set; }
        public string OriginalFilePath { get; set; }
        public string SplitDocument { get; set; }

        private int CalculateTotalPages()
        {
            var pages = 0;
            if (SourceFiles == null)
                return pages;

            foreach (var sfi in SourceFiles)
                pages += sfi.TotalPages;

            return pages;
        }

        public override string ToString()
        {
            if (SourceFiles.Count == 0)
                return base.ToString();

            var sourceFile = SourceFiles[0];
            return $"{sourceFile.DocumentTitle} - {sourceFile.Filename}";
        }
    }
}
