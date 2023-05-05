using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobDataUpdater
    {
        void UpdateTokensAndMetadata(Job job);

        Task UpdateTokensAndMetadataAsync(Job job);
    }

    public class JobDataUpdater : IJobDataUpdater
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPageNumberCalculator _pageNumberCalculator;
        private readonly IUserTokenExtractor _userTokenExtractor;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly IPsToPdfConverter _psToPdfConverter;
        private readonly ITokenReplacerFactory _tokenReplacerFactory;

        public JobDataUpdater(ITokenReplacerFactory tokenReplacerFactory, IPageNumberCalculator pageNumberCalculator,
            IUserTokenExtractor userTokenExtractor, IJobInfoManager jobInfoManager, IPsToPdfConverter psToPdfConverter)
        {
            _tokenReplacerFactory = tokenReplacerFactory;
            _pageNumberCalculator = pageNumberCalculator;
            _userTokenExtractor = userTokenExtractor;
            _jobInfoManager = jobInfoManager;
            _psToPdfConverter = psToPdfConverter;
        }

        public void UpdateTokensAndMetadata(Job job)
        {
            // Must be done before TokenReplacer is built
            if (job.Profile.UserTokens.Enabled)
            {
                _psToPdfConverter.ConvertJobInfoSourceFilesToPdf(job);
                SetSplitDocumentAndSourceFileInfos(job);
            }

            // Update job after extracting split document to get new number of pages
            job.NumberOfCopies = GetNumberOfCopies(job);
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

        private int GetNumberOfCopies(Job job)
        {
            var copies = 0;
            try
            {
                copies = job.JobInfo.SourceFiles.First().Copies;
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

        private void SetSplitDocumentAndSourceFileInfos(Job job)
        {
            var oldFiles = new List<string>();

            foreach (var sfi in job.JobInfo.SourceFiles)
            {
                if (sfi.UserTokenEvaluated)
                    continue;

                var parsedFile = _userTokenExtractor.ParsePdfFileForUserTokens(sfi.Filename, job.Profile.UserTokens.Separator);

                if (parsedFile.Filename != sfi.Filename)
                    oldFiles.Add(sfi.Filename);

                sfi.Filename = parsedFile.Filename;
                sfi.UserToken = parsedFile.UserToken;
                sfi.UserTokenEvaluated = true;
                job.JobInfo.SplitDocument = parsedFile.SplitDocument;

                if (!string.IsNullOrEmpty(job.JobInfo.SplitDocument))
                    job.JobInfo.SourceFiles.First().TotalPages = parsedFile.NumberOfPages;
                else
                    sfi.TotalPages = parsedFile.NumberOfPages;
            }

            CleanUp(oldFiles);
        }

        private static void CleanUp(List<string> oldFiles)
        {
            foreach (var file in oldFiles)
            {
                File.Delete(file);
            }
        }
    }
}
