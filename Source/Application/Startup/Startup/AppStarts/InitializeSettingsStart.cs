using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class InitializeSettingsStart : AppStartBase
    {
        private readonly ISettingsManager _settingsManager;

        public InitializeSettingsStart(ICheckAllStartupConditions checkAllStartupConditions, ISettingsManager settingsManager)
            : base(checkAllStartupConditions)
        {
            _settingsManager = settingsManager;
        }

        public override Task<ExitCode> Run()
        {
            try
            {
                _settingsManager.LoadAllSettings();
                _settingsManager.SaveCurrentSettings();
            }
            catch (Exception)
            {
                return Task.FromResult(ExitCode.Unknown);
            }

            return Task.FromResult(ExitCode.Ok);
        }
    }
}
