using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.UI.Presentation.Helper.TestPage;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class PrintTestPageAsyncCommand : AsyncCommandBase, IPrintTestPageAsyncCommand
    {
        private readonly ITestPageHelper _testPageHelper;
        private readonly ICurrentSettings<ApplicationSettings> _appSettings;
        private readonly ISelectedProfileProvider _selectedProfileProvider;

        public PrintTestPageAsyncCommand(ITestPageHelper testPageHelper, ICurrentSettings<ApplicationSettings> appSettings, ISelectedProfileProvider selectedProfileProvider)
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
            await Task.Run(() =>
            {
                LoggingHelper.ChangeLogLevel(_appSettings.Settings.LoggingLevel);

                var openDir = bool.Parse(parameter as string ?? string.Empty);
                _testPageHelper.CreateAndPrintTestPage(_selectedProfileProvider.SelectedProfile, openDir);
            });
        }
    }
}
