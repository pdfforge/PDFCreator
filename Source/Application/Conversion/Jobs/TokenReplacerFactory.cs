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
            _tokenReplacer.AddListToken("OutputFilenames", outputFilenames);
            _tokenReplacer.AddStringToken("OutputFilePath", _pathWrap.GetFullPath(job.OutputFiles.First()));

            return _tokenReplacer;
        }

        public TokenReplacer BuildTokenReplacerWithoutOutputfiles(Job job)
        {
            BuildTokenReplacerFromJobInfo(job.JobInfo);
            _tokenReplacer.AddNumberToken("NumberOfPages", job.NumberOfPages);
            _tokenReplacer.AddNumberToken("NumberOfCopies", job.NumberOfCopies);

            return _tokenReplacer;
        }

        private void AddDateToken()
        {
            _tokenReplacer.AddDateToken("DateTime", _dateTimeProvider.Now());
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
            _tokenReplacer.AddStringToken("ClientComputer", sourceFileInfo.ClientComputer);
            _tokenReplacer.AddNumberToken("Counter", sourceFileInfo.JobCounter);
            _tokenReplacer.AddNumberToken("JobId", sourceFileInfo.JobId);
            _tokenReplacer.AddStringToken("PrinterName", sourceFileInfo.PrinterName);
            _tokenReplacer.AddNumberToken("SessionId", sourceFileInfo.SessionId);
        }

        private void AddMetaDataTokens(Metadata metadata)
        {
            _tokenReplacer.AddStringToken("PrintJobAuthor", metadata.PrintJobAuthor);
            _tokenReplacer.AddStringToken("PrintJobName", metadata.PrintJobName);

            var subject = _tokenReplacer.ReplaceTokens(metadata.Subject);
            _tokenReplacer.AddStringToken("Subject", subject);

            var keywords = _tokenReplacer.ReplaceTokens(metadata.Keywords);
            _tokenReplacer.AddStringToken("Keywords", keywords);

            // Author and title token have to be created last,
            // as they can contain other tokens that might need replacing
            var author = _tokenReplacer.ReplaceTokens(metadata.Author);
            _tokenReplacer.AddStringToken("Author", author);

            var title = _tokenReplacer.ReplaceTokens(metadata.Title);
            _tokenReplacer.AddStringToken("Title", title);
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

            _tokenReplacer.AddStringToken("InputFilename", originalFileName);
            _tokenReplacer.AddStringToken("InputDirectory", originalDirectory);
            _tokenReplacer.AddStringToken("InputFilePath", originalDirectory);
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
