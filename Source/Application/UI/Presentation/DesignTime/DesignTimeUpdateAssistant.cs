using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateAssistant : IUpdateAssistant
    {
        public IApplicationVersion OnlineVersion => new ApplicationVersion(new Version(1, 2, 3, 4), "http://download.local", "12345", null);

        public event EventHandler TryShowUpdateInteraction;

        public bool UpdateProcedureIsRunning => false;
        public bool UpdatesEnabled => true;

        public void UpdateProcedure(bool checkNecessity)
        {
        }

        public bool ShowUpdate => true;
        public Release CurrentReleaseVersion { get; set; }

        public void SkipVersion()
        {
        }

        public void SetNewUpdateTime()
        {
        }

        public bool IsOnlineUpdateAvailable(bool checkNecessity)
        {
            return true;
        }

        public bool IsUpdateAvailable()
        {
            return true;
        }

        public void DownloadUpdate()
        {
        }
    }
}
