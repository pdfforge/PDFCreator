using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Utilities.IO;
using System;

namespace pdfforge.PDFCreator.Core.Services
{
    public interface IPdfCreatorFolderCleanUp
    {
        void CleanTempFolder(TimeSpan timeSpan);

        void CleanSpoolFolder(TimeSpan timeSpan);
    }

    public class PdfCreatorFolderCleanUp : IPdfCreatorFolderCleanUp
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISpoolerProvider _spoolFolderProvider;
        private readonly IFolderCleaner _folderCleaner;
        private readonly ITempFolderProvider _tempFolderProvider;

        public PdfCreatorFolderCleanUp(ITempFolderProvider tempFolderProvider, ISpoolerProvider spoolFolderProvider, IFolderCleaner folderCleaner)
        {
            _tempFolderProvider = tempFolderProvider;
            _spoolFolderProvider = spoolFolderProvider;
            _folderCleaner = folderCleaner;
        }

        public void CleanTempFolder(TimeSpan timeSpan)
        {
            DoCleanFolder(_tempFolderProvider.TempFolder, timeSpan);
        }

        public void CleanSpoolFolder(TimeSpan timeSpan)
        {
            DoCleanFolder(_spoolFolderProvider.SpoolFolder, timeSpan);
        }

        private void DoCleanFolder(string folder, TimeSpan timeSpan)
        {
            var cleanupExceptions = _folderCleaner.Clean(folder, timeSpan);

            foreach (var pathExceptionPair in cleanupExceptions)
            {
                _logger.Debug($"Exception while cleaning up {pathExceptionPair.Key}: {pathExceptionPair.Value}");
            }
        }
    }
}
