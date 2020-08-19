using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class DisabledUpdateHelper : IUpdateHelper
    {
        public bool UpdatesEnabled => false;

        public void SkipVersion()
        {
        }

        public void SetNewUpdateTime()
        {
        }

        public Task<bool> IsUpdateAvailableAsync(bool checkNecessity)
        {
            return Task.FromResult(false);
        }

        public Task UpdateCheckAsync(bool checkNecessity)
        {
            return Task.FromResult(false);
        }

        public bool UpdateShouldBeShown()
        {
            return false;
        }

        public void ShowLater()
        {
        }

        public bool IsTimeForNextUpdate()
        {
            return false;
        }
    }
}
