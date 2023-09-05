using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.Annotations;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using System;
using SystemInterface.Diagnostics;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.TestPage
{
    public interface ITestPageHelper
    {
        void CreateAndPrintTestPage(ConversionProfile profile = null, bool openDirectory = false);
    }

    public abstract class TestPageHelperBase : ITestPageHelper
    {
        private readonly ITestPageCreator _testPageCreator;
        private readonly IDirectory _directory;
        private readonly IProcess _process;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        protected TestPageHelperBase(ITestPageCreator testPageCreator, IDirectory directory, IProcess process)
        {
            _testPageCreator = testPageCreator;
            _directory = directory;
            _process = process;
        }

        public void CreateAndPrintTestPage(ConversionProfile profile = null, bool openDirectory = false)
        {
            var tempFolderPath = CreateTempDirectory();
            var infFilePath = _testPageCreator.CreateTestPage(tempFolderPath);

            PrintTestPage(infFilePath, profile);

            if (openDirectory)
                TryOpenFolder(profile?.TargetDirectory);

            CleanUp(tempFolderPath);
        }

        private string CreateTempDirectory()
        {
            var spoolFolder = GetSpoolFolder();
            var tempDirectory = PathSafe.Combine(spoolFolder, Guid.NewGuid().ToString());
            _directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        protected abstract string GetSpoolFolder();

        protected abstract void PrintTestPage(string infFilePath, ConversionProfile queue = null);

        protected abstract void CleanUp(string tempFolderPath);

        private void TryOpenFolder([CanBeNull] string targetDirectory)
        {
            try
            {
                if (_directory.Exists(targetDirectory))
                {
                    _process.Start("explorer.exe", targetDirectory);
                }
                else
                {
                    _logger.Error($"The directory '{targetDirectory}' does not exist!");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}
