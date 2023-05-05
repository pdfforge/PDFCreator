using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.IO;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IPsToPdfConverter
    {
        void ConvertJobInfoSourceFilesToPdf(Job job);
    }

    public class PsToPdfConverter : IPsToPdfConverter
    {
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IConverter _converter;
        private readonly IJobCleanUp _jobCleanUp;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IUniqueFilenameFactory _uniqueFilenameFactory;

        public PsToPdfConverter(ITempFolderProvider tempFolderProvider, IConverterFactory converterFactory, IJobCleanUp jobCleanUp, IFile file, IDirectory directory, IUniqueFilenameFactory uniqueFilenameFactory)
        {
            _tempFolderProvider = tempFolderProvider;
            _jobCleanUp = jobCleanUp;
            _file = file;
            _directory = directory;
            _uniqueFilenameFactory = uniqueFilenameFactory;
            _converter = converterFactory.GetConverter(JobType.PsJob);
        }

        // TODO FOR FUTURE: Check if it's possible to refactor in a way where you only pass source files rather than the whole job.
        public void ConvertJobInfoSourceFilesToPdf(Job job)
        {
            foreach (var sourceFileInfo in job.JobInfo.SourceFiles)
            {
                var sfiExtension = PathSafe.GetExtension(sourceFileInfo.Filename);
                if (sfiExtension == ".pdf")
                    continue;

                if (job.IntermediateFolder == null)
                    job.IntermediateFolder = _tempFolderProvider.CreatePrefixTempFolder("intermediate");

                var psFile = sourceFileInfo.Filename;
                var jobTempFolder = _tempFolderProvider.CreatePrefixTempFolder("Job");
                var pdfFile = ConvertPsToPdf(psFile, jobTempFolder, job.IntermediateFolder);

                sourceFileInfo.Filename = pdfFile;

                foreach (var fileInfo in job.JobInfo.SourceFiles.Where(sfi => sfi.Filename == psFile))
                {
                    fileInfo.Filename = pdfFile;
                }
            }
        }

        private string ConvertPsToPdf(string psFilePath, string tempFolder, string intermediateFolder)
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
                _jobCleanUp.DoCleanUp(job.JobTempFolder, job.JobInfo.SourceFiles, job.JobInfo.InfFile);
                _directory.Delete(job.IntermediateFolder, true);
            }
            catch
            {
                // ignore
            }
        }
    }
}
