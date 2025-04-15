using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;
using System.Linq;
using SystemInterface;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public interface ITokenReplacerFactory
    {
        TokenReplacer BuildTokenReplacerWithOutputfiles(Job job);

        TokenReplacer BuildTokenReplacerWithoutOutputfiles(Job job);
    }

    public class TokenReplacerFactory : ITokenReplacerFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEnvironment _environmentWrap;
        private readonly IPathUtil _pathUtil;
        private readonly IPath _pathWrap;
        private TokenReplacer _tokenReplacer;

        public TokenReplacerFactory(IDateTimeProvider dateTime, IEnvironment environment, IPath path, IPathUtil pathUtil)
        {
            _dateTimeProvider = dateTime;
            _environmentWrap = environment;
            _pathWrap = path;
            _pathUtil = pathUtil;
        }

        public TokenReplacer BuildTokenReplacerFromJobInfo(JobInfo.JobInfo jobInfo)
        {
            _tokenReplacer = new TokenReplacer();

            AddEnvironmentTokens();
            AddDateToken();
            AddSourceFileTokens(jobInfo.SourceFiles[0]);
            AddTokensFromOriginalFilePath(jobInfo.SourceFiles[0], jobInfo.Metadata, jobInfo);
            AddUserTokens(jobInfo.SourceFiles);

            // AddMetaDataTokens should be called last
            // as they can contain other tokens that might need replacing
            AddMetaDataTokens(jobInfo.Metadata);
            return _tokenReplacer;
        }

        public TokenReplacer BuildTokenReplacerWithOutputfiles(Job job)
        {
            BuildTokenReplacerWithoutOutputfiles(job);

            var outputFilenames = job.OutputFiles.Select(outputFile => _pathWrap.GetFileName(outputFile)).ToList();
            _tokenReplacer.AddListToken(TokenNames.OutputFilenames, outputFilenames);
            _tokenReplacer.AddStringToken(TokenNames.OutputFilePath, _pathWrap.GetFullPath(job.OutputFiles.First()));

            return _tokenReplacer;
        }

        public TokenReplacer BuildTokenReplacerWithoutOutputfiles(Job job)
        {
            BuildTokenReplacerFromJobInfo(job.JobInfo);
            _tokenReplacer.AddNumberToken(TokenNames.NumberOfPages, job.NumberOfPages);
            _tokenReplacer.AddNumberToken(TokenNames.NumberOfCopies, job.NumberOfCopies);

            return _tokenReplacer;
        }

        private void AddDateToken()
        {
            _tokenReplacer.AddDateToken(TokenNames.DateTime, _dateTimeProvider.Now());
        }

        private void AddEnvironmentTokens()
        {
            _tokenReplacer.AddToken(new SingleEnvironmentToken(EnvironmentVariable.ComputerName, _environmentWrap));
            _tokenReplacer.AddToken(new SingleEnvironmentToken(EnvironmentVariable.Username, _environmentWrap));
            _tokenReplacer.AddToken(new SingleEnvironmentToken(EnvironmentVariable.Desktop, _environmentWrap));
            _tokenReplacer.AddToken(new SingleEnvironmentToken(EnvironmentVariable.MyDocuments, _environmentWrap));
            _tokenReplacer.AddToken(new SingleEnvironmentToken(EnvironmentVariable.MyPictures, _environmentWrap));

            _tokenReplacer.AddToken(new EnvironmentToken(_environmentWrap, "Environment"));
        }

        private void AddSourceFileTokens(SourceFileInfo sourceFileInfo)
        {
            _tokenReplacer.AddStringToken(TokenNames.ClientComputer, sourceFileInfo.ClientComputer);
            _tokenReplacer.AddNumberToken(TokenNames.Counter, sourceFileInfo.JobCounter);
            _tokenReplacer.AddNumberToken(TokenNames.JobId, sourceFileInfo.JobId);
            _tokenReplacer.AddStringToken(TokenNames.PrinterName, sourceFileInfo.PrinterName);
            _tokenReplacer.AddNumberToken(TokenNames.SessionId, sourceFileInfo.SessionId);
        }

        private void AddMetaDataTokens(Metadata metadata)
        {
            _tokenReplacer.AddStringToken(TokenNames.PrintJobAuthor, metadata.PrintJobAuthor);
            _tokenReplacer.AddStringToken(TokenNames.PrintJobName, metadata.PrintJobName);

            var subject = _tokenReplacer.ReplaceTokens(metadata.Subject);
            _tokenReplacer.AddStringToken(TokenNames.Subject, subject);

            var keywords = _tokenReplacer.ReplaceTokens(metadata.Keywords);
            _tokenReplacer.AddStringToken(TokenNames.Keywords, keywords);

            // Author and title token have to be created last,
            // as they can contain other tokens that might need replacing
            var author = _tokenReplacer.ReplaceTokens(metadata.Author);
            _tokenReplacer.AddStringToken(TokenNames.Author, author);

            var title = _tokenReplacer.ReplaceTokens(metadata.Title);
            _tokenReplacer.AddStringToken(TokenNames.Title, title);
        }

        private void AddTokensFromOriginalFilePath(SourceFileInfo sfi, Metadata metadata, JobInfo.JobInfo jobInfo)
        {
            var originalFileName = metadata.PrintJobName;
            var originalDirectory = "";

            if (!string.IsNullOrEmpty(jobInfo.OriginalFilePath))
            {
                originalFileName = PathSafe.GetFileNameWithoutExtension(jobInfo.OriginalFilePath);
                originalDirectory = PathSafe.GetDirectoryName(jobInfo.OriginalFilePath);
            }
            else if (_pathUtil.IsValidRootedPath(sfi.DocumentTitle))
            {
                originalFileName = PathSafe.GetFileNameWithoutExtension(sfi.DocumentTitle);
                originalDirectory = PathSafe.GetDirectoryName(sfi.DocumentTitle);
            }

            _tokenReplacer.AddStringToken(TokenNames.InputFilename, originalFileName);
            _tokenReplacer.AddStringToken(TokenNames.InputDirectory, originalDirectory);
            _tokenReplacer.AddStringToken(TokenNames.InputFilePath, originalDirectory);
        }

        private void AddUserTokens(IList<SourceFileInfo> sourceFileInfos)
        {
            var userToken = new UserToken();
            foreach (var sfi in sourceFileInfos)
            {
                userToken.Merge(sfi.UserToken);
            }
            _tokenReplacer.AddToken(userToken);
        }
    }
}
