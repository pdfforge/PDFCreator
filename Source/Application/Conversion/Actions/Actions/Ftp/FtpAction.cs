using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp
{
    public class FtpAction : RetypePasswordActionBase<Settings.Ftp>, IPostConversionAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IFtpConnectionFactory _ftpConnectionFactory;
        private readonly IPathUtil _pathUtil;
        private readonly IFtpConnectionTester _ftpConnectionTester;

        protected override string PasswordText => "FTP";

        public FtpAction(IFtpConnectionFactory ftpConnectionFactory, IPathUtil pathUtil, IFtpConnectionTester ftpConnectionTester)
            : base(p => p.Ftp)
        {
            _ftpConnectionFactory = ftpConnectionFactory;
            _pathUtil = pathUtil;
            _ftpConnectionTester = ftpConnectionTester;
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.Ftp.Directory = job.TokenReplacer.ReplaceTokens(job.Profile.Ftp.Directory);
            job.Profile.Ftp.Directory = ValidName.MakeValidFtpPath(job.Profile.Ftp.Directory);
        }

        public override ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();
            if (!IsEnabled(profile))
                return actionResult;

            var isRunningJob = checkLevel == CheckLevel.RunningJob;

            var ftpAccount = settings.Accounts.GetFtpAccount(profile);
            if (ftpAccount == null)
            {
                Logger.Error($"The specified FTP account with ID '{profile.Ftp.AccountId}' is not configured.");
                actionResult.Add(ErrorCode.Ftp_NoAccount);

                return actionResult;
            }

            var ignoreMissingPassword = !profile.AutoSave.Enabled;
            var checkAccountResult = _ftpConnectionTester.CheckAccount(ftpAccount, ignoreMissingPassword);
            actionResult.Add(checkAccountResult);

            if (!isRunningJob && TokenIdentifier.ContainsTokens(profile.Ftp.Directory))
                return actionResult;

            if (!ValidName.IsValidFtpPath(profile.Ftp.Directory))
                actionResult.Add(ErrorCode.FtpDirectory_InvalidFtpPath);

            return actionResult;
        }

        protected override ActionResult DoActionProcessing(Job job)
        {
            var ftpAccount = job.Accounts.GetFtpAccount(job.Profile);

            Logger.Debug("Creating ftp connection.\r\nServer: " + ftpAccount.Server + "\r\nUsername: " + ftpAccount.UserName);

            var ftpClient = _ftpConnectionFactory.
                BuildConnection(ftpAccount, job.Passwords.FtpPassword);

            try
            {
                ftpClient.Connect();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while login to ftp server: ");
                ftpClient.Disconnect();
                return new ActionResult(ErrorCode.Ftp_LoginError);
            }

            var fullDirectory = job.TokenReplacer.ReplaceTokens(job.Profile.Ftp.Directory).Trim();
            if (!ValidName.IsValidFtpPath(fullDirectory))
            {
                Logger.Warn("Directory contains invalid characters \"" + fullDirectory + "\"");
                fullDirectory = ValidName.MakeValidFtpPath(fullDirectory);
            }

            Logger.Debug("Directory on ftp server: " + fullDirectory);

            try
            {
                if (!ftpClient.DirectoryExists(fullDirectory))
                    ftpClient.CreateDirectory(fullDirectory);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while setting directory on ftp server: ");
                ftpClient.Disconnect();
                return new ActionResult(ErrorCode.Ftp_DirectoryError);
            }

            foreach (var file in job.OutputFiles)
            {
                var targetFile = PathSafe.GetFileName(file);
                targetFile = ValidName.MakeValidFtpPath(targetFile);
                if (job.Profile.Ftp.EnsureUniqueFilenames)
                {
                    Logger.Debug("Make unique filename for " + targetFile);
                    try
                    {
                        var uf = new UniqueFilenameForFtp(targetFile, ftpClient, _pathUtil);
                        targetFile = uf.CreateUniqueFileName();
                        Logger.Debug("-> The unique filename is \"" + targetFile + "\"");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Exception while generating unique filename: ");
                        ftpClient.Disconnect();
                        return new ActionResult(ErrorCode.Ftp_DirectoryReadError);
                    }
                }

                try
                {
                    ftpClient.UploadFile(file, fullDirectory + "/" + targetFile);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception while uploading the file \"" + file);
                    ftpClient.Disconnect();
                    return new ActionResult(ErrorCode.Ftp_UploadError);
                }
            }

            ftpClient.Disconnect();
            return new ActionResult();
        }

        public override bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        protected override void ApplyActionSpecificRestrictions(Job job)
        { }

        protected override void SetPassword(Job job, string password)
        {
            job.Passwords.FtpPassword = password;
        }
    }
}
