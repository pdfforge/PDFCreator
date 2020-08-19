using pdfforge.PDFCreator.UI.Presentation.Assistants;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateHelper : IUpdateHelper
    {
        public bool UpdatesEnabled => true;

        public void SkipVersion()
        {
        }

        public void SetNewUpdateTime()
        {
        }

        public Task<bool> IsUpdateAvailableAsync(bool checkNecessity)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsUpdateAvailableAsync()
        {
            return Task.FromResult(true);
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
            return true;
        }
    }
}
