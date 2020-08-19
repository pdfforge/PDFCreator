using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IUpdateHelper
    {
        bool UpdatesEnabled { get; }

        void SkipVersion();

        void SetNewUpdateTime();

        Task<bool> IsUpdateAvailableAsync(bool checkNecessity);

        Task UpdateCheckAsync(bool checkNecessity);

        bool UpdateShouldBeShown();

        void ShowLater();

        bool IsTimeForNextUpdate();
    }
}
