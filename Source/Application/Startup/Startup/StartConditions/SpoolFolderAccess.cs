using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public interface ISpoolFolderAccess
    {
        bool CanAccess();
    }

    public class SpoolFolderAccess : ISpoolFolderAccess
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISpoolerProvider _spoolerProvider;
        private readonly IDirectory _directory;
        private readonly IDirectoryAccessControl _directoryAccess;

        public SpoolFolderAccess(ISpoolerProvider spoolerProvider, IDirectory directory, IDirectoryAccessControl directoryAccess)
        {
            _spoolerProvider = spoolerProvider;
            _directory = directory;
            _directoryAccess = directoryAccess;
        }

        public bool CanAccess()
        {
            var spoolFolder = _spoolerProvider.SpoolFolder;
            try
            {
                if (!_directory.Exists(spoolFolder))
                {
                    _directory.CreateDirectory(spoolFolder);
                }

                _directoryAccess.GetAccessControl(spoolFolder);

                foreach (var directory in _directory.EnumerateDirectories(spoolFolder, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        _logger.Debug("Checking directory " + directory);
                        _directoryAccess.GetAccessControl(directory);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.Info("Exception while checking spool folder: " + ex);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Debug("The spool folder seems to be broken: " + ex);
                return false;
            }

            return true;
        }
    }
}
