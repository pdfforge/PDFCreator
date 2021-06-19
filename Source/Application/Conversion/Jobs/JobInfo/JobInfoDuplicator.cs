using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public interface IJobInfoDuplicator
    {
        JobInfo Duplicate(JobInfo jobInfo, string profileGuid = null);
    }

    public class JobInfoDuplicator : IJobInfoDuplicator
    {
        private readonly IJobFolderBuilder _jobFolderBuilder;
        private readonly ISourceFileInfoDuplicator _sourceFileInfoDuplicator;
        private readonly IJobInfoManager _jobInfoManager;

        public JobInfoDuplicator(IJobFolderBuilder jobFolderBuilder, ISourceFileInfoDuplicator sourceFileInfoDuplicator, IJobInfoManager jobInfoManager)
        {
            _jobFolderBuilder = jobFolderBuilder;
            _sourceFileInfoDuplicator = sourceFileInfoDuplicator;
            _jobInfoManager = jobInfoManager;
        }

        public JobInfo Duplicate(JobInfo jobInfo, string profileGuid = null)
        {
            var jobInfoDuplicate = new JobInfo();

            jobInfoDuplicate.Metadata = jobInfo.Metadata.Copy();
            jobInfoDuplicate.JobType = jobInfo.JobType;
            jobInfoDuplicate.PrintDateTime = jobInfo.PrintDateTime;
            jobInfoDuplicate.PrinterName = jobInfo.PrinterName;

            jobInfoDuplicate.PrinterParameter = profileGuid == null ? jobInfo.PrinterParameter : "";
            jobInfoDuplicate.ProfileParameter = profileGuid ?? jobInfo.ProfileParameter;
            jobInfoDuplicate.OutputFileParameter = jobInfo.OutputFileParameter;

            jobInfoDuplicate.OriginalFilePath = jobInfo.OriginalFilePath;

            var sfiFilename = PathSafe.GetFileName(jobInfo.SourceFiles[0].Filename);
            var duplicateJobFolder = _jobFolderBuilder.CreateJobFolderInSpool(sfiFilename);
            jobInfoDuplicate.InfFile = PathSafe.Combine(duplicateJobFolder, "DuplicateInfFile.inf");

            foreach (var sfi in jobInfo.SourceFiles)
            {
                var sfiDuplicate = _sourceFileInfoDuplicator.Duplicate(sfi, duplicateJobFolder, profileGuid);
                jobInfoDuplicate.SourceFiles.Add(sfiDuplicate);
            }

            _jobInfoManager.SaveToInfFile(jobInfoDuplicate);

            return jobInfoDuplicate;
        }
    }
}
