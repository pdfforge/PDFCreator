using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public interface IJobInfoDuplicator
    {
        JobInfo Duplicate(JobInfo jobInfo, string profileGuid = null);

        JobInfo CreateJobInfoForSplitDocument(JobInfo jobInfo, string splitDocument, string splitJobParentFilePath, int totalPages, string profileGUid = null);
    }

    public class JobInfoDuplicator : IJobInfoDuplicator
    {
        private readonly ISourceFileInfoDuplicator _sourceFileInfoDuplicator;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly ISpoolerProvider _spoolerProvider;

        public JobInfoDuplicator(ISourceFileInfoDuplicator sourceFileInfoDuplicator, IJobInfoManager jobInfoManager, ISpoolerProvider spoolerProvider)
        {
            _sourceFileInfoDuplicator = sourceFileInfoDuplicator;
            _jobInfoManager = jobInfoManager;
            _spoolerProvider = spoolerProvider;
        }

        public JobInfo Duplicate(JobInfo jobInfo, string profileGuid = null)
        {
            var jobInfoDuplicate = DuplicateProperties(jobInfo, profileGuid);
            SetInfAndSourceFiles(jobInfo, jobInfoDuplicate, profileGuid);

            return jobInfoDuplicate;
        }

        public JobInfo CreateJobInfoForSplitDocument(JobInfo jobInfo, string splitDocument, string splitJobParentFilePath, int totalPages, string profileGuid)
        {
            if (string.IsNullOrWhiteSpace(splitDocument))
                return null;

            var remainingFile = new ParsedFile(splitDocument);
            var newJobInfo = Move(jobInfo, remainingFile, profileGuid);
            newJobInfo.SplitDocument = null;
            newJobInfo.SourceFiles.First().IsSplitJob = true;
            newJobInfo.SourceFiles.First().SplitJobParentFilePath = splitJobParentFilePath;
            newJobInfo.SourceFiles.First().TotalPages = totalPages;
            return newJobInfo;
        }

        private JobInfo Move(JobInfo jobInfo, ParsedFile parsedFile, string profileGuid)
        {
            var newJobInfo = DuplicateProperties(jobInfo, profileGuid);
            SetInfAndSourceFiles(jobInfo, newJobInfo, profileGuid, parsedFile);

            return newJobInfo;
        }

        private static JobInfo DuplicateProperties(JobInfo currentJobInfo, string profileGuid)
        {
            var newJobInfo = new JobInfo
            {
                InfFile = currentJobInfo.InfFile,
                Metadata = currentJobInfo.Metadata.Copy(),
                JobType = currentJobInfo.JobType,
                PrintDateTime = currentJobInfo.PrintDateTime,
                PrinterName = currentJobInfo.PrinterName,
                PrinterParameter = profileGuid == null
                    ? currentJobInfo.PrinterParameter
                    : "",
                ProfileParameter = profileGuid ?? currentJobInfo.ProfileParameter,
                OutputFileParameter = currentJobInfo.OutputFileParameter,
                OriginalFilePath = currentJobInfo.OriginalFilePath,
                SplitDocument = currentJobInfo.SplitDocument
            };

            return newJobInfo;
        }

        private (string InfFile, string NewJobFolder) GetInfFileAndJobFolder(JobInfo jobInfo)
        {
            var oldSfiFilename = jobInfo.SourceFiles.First().Filename;
            var newJobFolder = _spoolerProvider.SpoolFolder;
            return (PathSafe.Combine(newJobFolder, oldSfiFilename + ".inf"), newJobFolder);
        }

        private void SetInfAndSourceFiles(JobInfo jobInfo, JobInfo newJobInfo, string profileGuid, ParsedFile parsedFile = null)
        {
            var result = GetInfFileAndJobFolder(jobInfo);
            newJobInfo.InfFile = result.InfFile;

            foreach (var sfi in jobInfo.SourceFiles)
            {
                var newSfi = UpdateSourceFileInfo(sfi, result.NewJobFolder, profileGuid, parsedFile);
                newJobInfo.SourceFiles.Add(newSfi);
            }

            _jobInfoManager.SaveToInfFile(newJobInfo);
        }

        private SourceFileInfo UpdateSourceFileInfo(SourceFileInfo sfi, string newJobFolder, string profileGuid, ParsedFile parsedFile)
        {
            SourceFileInfo newSfi;
            if (parsedFile != null)
            {
                sfi.Filename = parsedFile.Filename;
                sfi.UserToken = parsedFile.UserToken;
                sfi.UserTokenEvaluated = false;
                sfi.TotalPages = parsedFile.NumberOfPages;
                newSfi = _sourceFileInfoDuplicator.Move(sfi, newJobFolder, profileGuid);
            }
            else
            {
                newSfi = _sourceFileInfoDuplicator.Duplicate(sfi, newJobFolder, profileGuid);
            }

            return newSfi;
        }
    }
}
