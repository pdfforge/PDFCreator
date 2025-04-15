using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IPsToPdfConverter
    {
        Task ConvertSourceFileToPdf(SourceFileInfo sourceFileInfo);
        void CleanUpTaskMappings(IList<SourceFileInfo> sourceFileInfos);
    }

    public class PsToPdfConverter : IPsToPdfConverter
    {
        private readonly IConverter _converter;
        private readonly IJobCleaner _jobCleaner;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IUniqueFilenameFactory _uniqueFilenameFactory;

        private readonly Dictionary<string, Task<string>> _fileTaskMapping = new();

        public PsToPdfConverter(IConverterFactory converterFactory, IJobCleaner jobCleaner, IFile file, IDirectory directory, IUniqueFilenameFactory uniqueFilenameFactory)
        {
            _jobCleaner = jobCleaner;
            _file = file;
            _directory = directory;
            _uniqueFilenameFactory = uniqueFilenameFactory;
            _converter = converterFactory.GetConverter(JobType.PsJob);
        }

        public async Task ConvertSourceFileToPdf(SourceFileInfo sfi)
        {
            var key = GetFileTaskMappingKey(sfi.Filename);
            if (_fileTaskMapping.TryGetValue(key, out var psToPdfTask))
            {
                sfi.Filename = await psToPdfTask;
            }
            else
            {
                _fileTaskMapping[key] = Task.Run(() => ConvertPsToPdf(sfi.Filename));
                sfi.Filename = await _fileTaskMapping[key];
            }
        }

        public void CleanUpTaskMappings(IList<SourceFileInfo> sourceFileInfos)
        {
            foreach (var sfi in sourceFileInfos)
            {
                var key = GetFileTaskMappingKey(sfi.Filename);
                _fileTaskMapping.Remove(key);
            }
        }

        private string ConvertPsToPdf(string sfiFilename)
        {
            var sfiExtension = PathSafe.GetExtension(sfiFilename);
            if (sfiExtension == ".pdf")
                return sfiFilename;

            var sfiFolder = PathSafe.GetDirectoryName(sfiFilename);
            var intermediateFolder = PathSafe.Combine(sfiFolder, "intermediate");
            _directory.CreateDirectory(intermediateFolder);
            var jobTempFolder = PathSafe.Combine(sfiFolder, "temp");
            _directory.CreateDirectory(jobTempFolder);

            var pdfFile = DoConvertPsToPdf(sfiFilename, jobTempFolder, intermediateFolder);

            return pdfFile;
        }

        private string DoConvertPsToPdf(string psFilePath, string tempFolder, string intermediateFolder)
        {
            var job = BuildJobForPsFile(psFilePath, tempFolder, intermediateFolder);
            _converter.CreateIntermediatePdf(job);

            var pdfFile = PathSafe.ChangeExtension(psFilePath, ".pdf");

            if (_file.Exists(pdfFile))
            {
                var unique = _uniqueFilenameFactory.Build(pdfFile);
                pdfFile = unique.CreateUniqueFileName();
            }

            _file.Move(job.IntermediatePdfFile, pdfFile);

            CleanUp(job);
            return pdfFile;
        }

        private static Job BuildJobForPsFile(string psFilePath, string tempFolder, string intermediateFolder)
        {
            var jobInfo = new JobInfo();
            var sourceFileInfo = new SourceFileInfo()
            {
                Filename = psFilePath,
                OriginalFilePath = psFilePath
            };
            jobInfo.SourceFiles.Add(sourceFileInfo);
            var job = new Job(
                jobInfo,
                new ConversionProfile(),
                new CurrentJobSettings(new List<ConversionProfile>(), new List<PrinterMapping>(), new Accounts()))
            {
                JobTempFolder = tempFolder,
                IntermediateFolder = intermediateFolder
            };

            return job;
        }

        private void CleanUp(Job job)
        {
            try
            {
                _jobCleaner.DoCleanUp(job.JobTempFolder, job.JobInfo.SourceFiles, job.JobInfo.InfFile);
                _directory.Delete(job.IntermediateFolder, true);
            }
            catch
            {
                // ignore
            }
        }

        private string GetFileTaskMappingKey(string sfiFilename)
        {
            return PathSafe.ChangeExtension(sfiFilename, "key");
        }
    }
}
