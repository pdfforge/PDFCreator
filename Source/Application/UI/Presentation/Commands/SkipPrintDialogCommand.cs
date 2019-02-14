using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using System;
using System.Windows.Input;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class SkipPrintDialogCommand : ICommand
    {
        private readonly IFileNameQuery _saveFileQuery;

        public SkipPrintDialogCommand(IFileNameQuery saveFileQuery)
        {
            _saveFileQuery = saveFileQuery;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var job = parameter as Job;

            var folder = PathSafe.GetDirectoryName(job.OutputFileTemplate) ?? "";

            var filename = PathSafe.GetFileName(job.OutputFileTemplate) ?? "";

            var result = _saveFileQuery.GetFileName(folder, filename, job.Profile.OutputFormat);

            if (!result.Success)
            {
                throw new AbortWorkflowException("User cancelled in SaveFileDialog");
            }

            job.OutputFileTemplate = result.Data.Filepath;
            job.Profile.OutputFormat = result.Data.OutputFormat;
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
