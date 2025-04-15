using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Settings;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobDataUpdater
    {
        void UpdateTokensAndMetadata(Job job);
        Task UpdateTokensAndMetadataAsync(Job job);
    }

    public class JobDataUpdater(
        ITokenReplacerFactory tokenReplacerFactory,
        IPageNumberCalculator pageNumberCalculator,
        IUserTokenExtractor userTokenExtractor,
        IJobInfoManager jobInfoManager,
        IPsToPdfConverter psToPdfConverter,
        IFile file,
        IPreviewManager previewManager)
        : JobDataUpdaterBase(tokenReplacerFactory, pageNumberCalculator, userTokenExtractor, jobInfoManager, psToPdfConverter, file)
    {
        protected override void AbortPreviewTasks(string sfiFilename)
        {
            previewManager.AbortAndCleanUpPreview(sfiFilename);
        }
    }


    public abstract class JobDataUpdaterBase : IJobDataUpdater
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPageNumberCalculator _pageNumberCalculator;
        private readonly IUserTokenExtractor _userTokenExtractor;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IPsToPdfConverter _psToPdfConverter;
        private readonly IFile _file;
        private readonly ITokenReplacerFactory _tokenReplacerFactory;

        public JobDataUpdaterBase(ITokenReplacerFactory tokenReplacerFactory, IPageNumberCalculator pageNumberCalculator,
            IUserTokenExtractor userTokenExtractor, IJobInfoManager jobInfoManager, IPsToPdfConverter psToPdfConverter,
            IFile file)
        {
            _tokenReplacerFactory = tokenReplacerFactory;
            _pageNumberCalculator = pageNumberCalculator;
            _userTokenExtractor = userTokenExtractor;
            _jobInfoManager = jobInfoManager;
            _psToPdfConverter = psToPdfConverter;
            _file = file;
        }

        public void UpdateTokensAndMetadata(Job job)
        {
            // Must be done before TokenReplacer is built
            if (job.Profile.UserTokens.Enabled)
            {
                _logger.Debug("Trigger UpdateTokensAndMetadata for " + job.JobInfo.InfFile);
                SetSplitDocumentAndSourceFileInfos(job.JobInfo, job.Profile);
            }

            // Update job after extracting split document to get new number of pages
            job.NumberOfCopies = GetNumberOfCopies(job.JobInfo.SourceFiles);
            job.NumberOfPages = _pageNumberCalculator.GetNumberOfPages(job);

            job.TokenReplacer = _tokenReplacerFactory.BuildTokenReplacerWithoutOutputfiles(job);
            job.ReplaceTokensInMetadata();

            try
            {
                _jobInfoManager.SaveToInfFile(job.JobInfo);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Could not save inf file " + job.JobInfo.InfFile);
            }
        }

        public async Task UpdateTokensAndMetadataAsync(Job job)
        {
            await Task.Run(() => UpdateTokensAndMetadata(job));
        }

        private int GetNumberOfCopies(IList<SourceFileInfo> sourceFileInfos)
        {
            var copies = 0;
            try
            {
                copies = sourceFileInfos.First().Copies;
            }
            catch
            { }

            if (copies <= 0)
            {
                _logger.Warn("Problem detecting number of copies from source file(s). Set to 1.");
                copies = 1;
            }

            _logger.Debug("Number of copies from source files: " + copies);
            return copies;
        }

        private void SetSplitDocumentAndSourceFileInfos(JobInfo jobInfo, ConversionProfile profile)
        {
            foreach (var sfi in jobInfo.SourceFiles)
            {
                if (sfi.UserTokenEvaluated)
                    continue;

                _logger.Debug("Parse user tokens for " + sfi.Filename);

                _psToPdfConverter.ConvertSourceFileToPdf(sfi).GetAwaiter().GetResult();
                var parsedFile = _userTokenExtractor.ParsePdfFileForUserTokens(sfi.Filename, profile.UserTokens.Separator);

                //Abort preview task to regenerate the preview and unblock the file for deletion
                AbortPreviewTasks(sfi.Filename);
                if (parsedFile.Filename != sfi.Filename)
                    _file.Delete(sfi.Filename);

                sfi.Filename = parsedFile.Filename;
                sfi.UserToken = parsedFile.UserToken;
                sfi.UserTokenEvaluated = true;
                jobInfo.SplitDocument = parsedFile.SplitDocument;

                if (!string.IsNullOrEmpty(jobInfo.SplitDocument))
                    jobInfo.SourceFiles.First().TotalPages = parsedFile.NumberOfPages;
                else
                    sfi.TotalPages = parsedFile.NumberOfPages;
            }
        }

        protected abstract void AbortPreviewTasks(string sfiFilename);
    }
}
