using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface IFileNameQuery
    {
        QueryResult<OutputFilenameResult> GetFileName(string directory, string filename, OutputFormat outputFormat, bool requiresOverwriteCheck = true);
    }

    public class OutputFilenameResult
    {
        public OutputFilenameResult(string filepath, OutputFormat outputFormat)
        {
            Filepath = filepath;
            OutputFormat = outputFormat;
        }

        public string Filepath { get; set; } = "";
        public OutputFormat OutputFormat { get; set; } = OutputFormat.Pdf;
    }
}
