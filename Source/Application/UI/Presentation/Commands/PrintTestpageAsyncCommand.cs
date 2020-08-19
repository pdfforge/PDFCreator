using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Logging;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class PrintTestpageAsyncCommand : AsyncCommandBase, IPrintTestPageAsyncCommand
    {
        private readonly ITestPageHelper _testPageHelper;
        private readonly ICurrentSettings<ApplicationSettings> _appSettings;
        private readonly ISelectedProfileProvider _selectedProfileProvider;

        public PrintTestpageAsyncCommand(ITestPageHelper testPageHelper, ICurrentSettings<ApplicationSettings> appSettings, ISelectedProfileProvider selectedProfileProvider)
        {
            _testPageHelper = testPageHelper;
            _appSettings = appSettings;
            _selectedProfileProvider = selectedProfileProvider;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            await Task.Run(() => {
                LoggingHelper.ChangeLogLevel(_appSettings.Settings.LoggingLevel);
                _testPageHelper.CreateTestPage(_selectedProfileProvider.SelectedProfile.Name);
            });
        }
    }
}
