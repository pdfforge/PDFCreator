using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public interface ISpooledJobFinder
    {
        IEnumerable<JobInfo.JobInfo> GetJobs();
    }

    public class SpooledJobFinder : ISpooledJobFinder
    {
        private readonly IDirectory _directory;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISpoolerProvider _spoolerProvider;

        public SpooledJobFinder(ISpoolerProvider spoolerProvider, IDirectory directory, IJobInfoManager jobInfoManager)
        {
            _spoolerProvider = spoolerProvider;
            _directory = directory;
            _jobInfoManager = jobInfoManager;
        }

        public IEnumerable<JobInfo.JobInfo> GetJobs()
        {
            var spoolFolder = _spoolerProvider.SpoolFolder;
            _logger.Debug("Looking for spooled jobs in '{0}'", spoolFolder);

            var jobs = new List<JobInfo.JobInfo>();

            if (!_directory.Exists(spoolFolder))
            {
                _logger.Error("Spool folder '{0}' does not exist!", spoolFolder);
                return jobs;
            }

            var files = _directory.GetFiles(spoolFolder, "*.inf", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    _logger.Debug("Found inf file: " + file);
                    var jobInfo = _jobInfoManager.ReadFromInfFile(file);

                    jobs.Add(jobInfo);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, $"There was an error while reading the inf file '{file}': ");
                }
            }

            return jobs.OrderBy(job => job.PrintDateTime);
        }
    }
}
