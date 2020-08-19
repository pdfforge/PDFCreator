using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using System;
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

        public JobInfoDuplicator(IJobFolderBuilder jobFolderBuilder, ISourceFileInfoDuplicator sourceFileInfoDuplicator)
        {
            _jobFolderBuilder = jobFolderBuilder;
            _sourceFileInfoDuplicator = sourceFileInfoDuplicator;
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
            jobInfoDuplicate.InfFile = "DuplicateInfFileDummy_" + Guid.NewGuid();

            var sfiFilename = PathSafe.GetFileName(jobInfo.SourceFiles[0].Filename);
            var duplicateJobFolder = _jobFolderBuilder.CreateJobFolderInSpool(sfiFilename);

            foreach (var sfi in jobInfo.SourceFiles)
            {
                var sfiDuplicate = _sourceFileInfoDuplicator.Duplicate(sfi, duplicateJobFolder, profileGuid);
                jobInfoDuplicate.SourceFiles.Add(sfiDuplicate);
            }

            return jobInfoDuplicate;
        }
    }
}
