using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.UI.Presentation.Assistants;

namespace pdfforge.PDFCreator.UI.Presentation.Startup
{
    public class DownloadUpdateStartupAction : IDataStartupAction
    {
        private readonly IUpdateAssistant _updateAssistant;

        public DownloadUpdateStartupAction(IUpdateAssistant updateAssistant)
        {
            _updateAssistant = updateAssistant;
        }

        public void Execute()
        {
            _updateAssistant.DownloadUpdate();
        }
    }
}
