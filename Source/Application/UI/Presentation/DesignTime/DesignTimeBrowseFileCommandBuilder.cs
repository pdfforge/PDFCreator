using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeBrowseFileCommandBuilder : IBrowseFileCommandBuilder
    {
        public void Init(Func<Job> getJob, Action updateUi, Func<string> getLastConfirmedPath, Action<string> setLastConfirmedPath)
        {
        }

        public IMacroCommand BuildCommand(Predicate<object> canExecute = null)
        {
            return new MacroCommand(new List<ICommand>());
        }
    }
}
