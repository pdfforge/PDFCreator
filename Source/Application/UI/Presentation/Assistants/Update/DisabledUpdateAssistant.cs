using System;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class DisabledUpdateAssistant : IUpdateAssistant
    {
        public IApplicationVersion OnlineVersion => new ApplicationVersion(new Version(0, 0, 0, 0), "", "", null);

        public event EventHandler TryShowUpdateInteraction;

        public bool UpdateProcedureIsRunning => false;
        public bool UpdatesEnabled => false;

        public void UpdateProcedure(bool checkNecessity)
        {
        }

        public bool ShowUpdate => false;
        public Release CurrentReleaseVersion { get; set; }

        public void SkipVersion()
        {
        }

        public void SetNewUpdateTime()
        {
        }

        public bool IsOnlineUpdateAvailable(bool checkNecessity)
        {
            return false;
        }

        public bool IsUpdateAvailable()
        {
            return false;
        }

        public void DownloadUpdate()
        {
        }
    }
}
