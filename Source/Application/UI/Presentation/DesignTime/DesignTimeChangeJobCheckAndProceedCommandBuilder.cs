using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeChangeJobCheckAndProceedCommandBuilder : IChangeJobCheckAndProceedCommandBuilder
    {
        public void Init(Func<Job> getJob, Action callFinishInteraction, Func<string> getLatestConfirmedPath, Action<string> setLatestConfirmedPath)
        {
        }

        public IAsyncCommand BuildCommand(Action<Job> changeJobAction, IMacroCommand preSaveCommand = null)
        {
            return new AsyncCommand(o => Task.CompletedTask, (o) => true);
        }
    }
}
