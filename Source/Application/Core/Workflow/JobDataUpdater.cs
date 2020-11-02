using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;
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
        private readonly ITokenReplacerFactory _tokenReplacerFactory;

        public JobDataUpdater(ITokenReplacerFactory tokenReplacerFactory, IPageNumberCalculator pageNumberCalculator, IUserTokenExtractor userTokenExtractor, IJobInfoManager jobInfoManager)
        {
            _tokenReplacerFactory = tokenReplacerFactory;
            _pageNumberCalculator = pageNumberCalculator;
            _userTokenExtractor = userTokenExtractor;
            _jobInfoManager = jobInfoManager;
        }

        public void UpdateTokensAndMetadata(Job job)
        {
            job.NumberOfCopies = GetNumberOfCopies(job);
            job.NumberOfPages = _pageNumberCalculator.GetNumberOfPages(job);
            if (job.Profile.UserTokens.Enabled) //must be done before TokenReplacer is built
                SetUserTokensInSourceFileInfos(job.JobInfo.SourceFiles, job.Profile.UserTokens);

            job.TokenReplacer = _tokenReplacerFactory.BuildTokenReplacerWithoutOutputfiles(job);
            job.ReplaceTokensInMetadata();

            _jobInfoManager.SaveToInfFile(job.JobInfo);
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

        private void SetUserTokensInSourceFileInfos(IList<SourceFileInfo> sourceFileInfos, UserTokens userTokensSettings)
        {
            foreach (var sfi in sourceFileInfos)
            {
                if (!sfi.UserTokenEvaluated)
                {
                    sfi.UserToken = _userTokenExtractor.ExtractUserTokenFromPsFile(sfi.Filename, userTokensSettings.Seperator);
                    sfi.UserTokenEvaluated = true;
                }
            }
        }
    }
}
