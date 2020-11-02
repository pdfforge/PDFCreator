using NLog;
using pdfforge.PDFCreator.Utilities.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs.FolderProvider
{
    public interface IJobFolderBuilder
    {
        string CreateJobFolderInSpool(string file);
    }

    public class JobFolderBuilder : IJobFolderBuilder
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDirectory _directory;
        private readonly IUniqueDirectory _uniqueDirectory;
        private readonly ISpoolerProvider _spoolerProvider;

        public JobFolderBuilder(IDirectory directory, IUniqueDirectory uniqueDirectory, ISpoolerProvider spoolerProvider)
        {
            _directory = directory;
            _uniqueDirectory = uniqueDirectory;
            _spoolerProvider = spoolerProvider;
        }

        public string CreateJobFolderInSpool(string file)
        {
            var psFilename = PathSafe.GetFileName(file);
            if (psFilename.Length > 23)
                psFilename = psFilename.Substring(0, 23);
            var jobFolder = PathSafe.Combine(_spoolerProvider.SpoolFolder, psFilename);
            jobFolder = _uniqueDirectory.MakeUniqueDirectory(jobFolder);
            _directory.CreateDirectory(jobFolder);
            Logger.Trace("Created spool directory for job: " + jobFolder);

            return jobFolder;
        }
    }
}
