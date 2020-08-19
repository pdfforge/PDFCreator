using NLog;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.Utilities;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Startup
{
    public class DownloadUpdateStartupAction : IDataStartupAction
    {
        private readonly IVersionHelper _versionHelper;
        private readonly IOnlineVersionHelper _onlineVersionHelper;
        private readonly IUpdateHelper _updateHelper;
        private readonly IUpdateDownloader _updateDownloader;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DownloadUpdateStartupAction(IVersionHelper versionHelper, IOnlineVersionHelper onlineVersionHelper, IUpdateHelper updateHelper, IUpdateDownloader updateDownloader)
        {
            _versionHelper = versionHelper;
            _onlineVersionHelper = onlineVersionHelper;
            _updateHelper = updateHelper;
            _updateDownloader = updateDownloader;
        }

        public async Task Execute()
        {
            if (!_updateHelper.IsTimeForNextUpdate())
                return;

            var onlineVersion = await _onlineVersionHelper.LoadOnlineVersionAsync();

            if (!_updateHelper.UpdateShouldBeShown())
                return;

            if (onlineVersion == null)
            {
                _logger.Error("OnlineVersion not available");
                return;
            }

            var thisVersion = _versionHelper.ApplicationVersion;
            if (thisVersion.CompareTo(onlineVersion.Version) < 0)
            {
                await _updateDownloader.StartDownloadAsync(onlineVersion);
            }
        }
    }
}
