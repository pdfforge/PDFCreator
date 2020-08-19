using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class FtpAction : RetypePasswordActionBase, IPostConversionAction
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IFtpConnectionFactory _ftpConnectionFactory;
        private readonly IPathUtil _pathUtil;
        private readonly IFile _file;

        protected override string PasswordText => "FTP";

        public FtpAction(IFtpConnectionFactory ftpConnectionFactory, IPathUtil pathUtil, IFile file)
        {
            _ftpConnectionFactory = ftpConnectionFactory;
            _pathUtil = pathUtil;
            _file = file;
        }

        public override void ApplyPreSpecifiedTokens(Job job)
        {
            job.Profile.Ftp.Directory = job.TokenReplacer.ReplaceTokens(job.Profile.Ftp.Directory);
            job.Profile.Ftp.Directory = ValidName.MakeValidFtpPath(job.Profile.Ftp.Directory);
        }

        public override ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel)
        {
            var actionResult = new ActionResult();
            if (!IsEnabled(profile))
                return actionResult;

            var isFinal = checkLevel == CheckLevel.Job;

            var ftpAccount = accounts.GetFtpAccount(profile);
            if (ftpAccount == null)
            {
                Logger.Error($"The specified FTP account with ID '{profile.Ftp.AccountId}' is not configured.");
                actionResult.Add(ErrorCode.Ftp_NoAccount);

                return actionResult;
            }

            if (string.IsNullOrEmpty(ftpAccount.Server))
            {
                Logger.Error("No FTP server specified.");
                actionResult.Add(ErrorCode.Ftp_NoServer);
            }

            if (string.IsNullOrEmpty(ftpAccount.UserName))
            {
                Logger.Error("No FTP username specified.");
                actionResult.Add(ErrorCode.Ftp_NoUser);
            }

            if (ftpAccount.AuthenticationType == AuthenticationType.KeyFileAuthentication)
            {
                var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(ftpAccount.PrivateKeyFile);
                switch (pathUtilStatus)
                {
                    case PathUtilStatus.InvalidRootedPath:
                        return new ActionResult(ErrorCode.FtpKeyFilePath_InvalidRootedPath);

                    case PathUtilStatus.PathTooLongEx:
                        return new ActionResult(ErrorCode.FtpKeyFilePath_PathTooLong);

                    case PathUtilStatus.NotSupportedEx:
                        return new ActionResult(ErrorCode.FtpKeyFilePath_InvalidRootedPath);

                    case PathUtilStatus.ArgumentEx:
                        return new ActionResult(ErrorCode.FtpKeyFilePath_IllegalCharacters);
                }

                if (!isFinal && ftpAccount.PrivateKeyFile.StartsWith(@"\\"))
                    return new ActionResult();

                if (!_file.Exists(ftpAccount.PrivateKeyFile))
                {
                    Logger.Error("The private key file \"" + ftpAccount.PrivateKeyFile + "\" does not exist.");
                    return new ActionResult(ErrorCode.FtpKeyFilePath_FileDoesNotExist);
                }
            }

            if (profile.AutoSave.Enabled && string.IsNullOrEmpty(ftpAccount.Password) || KeyFilePasswordIsRequired(ftpAccount))
            {
                Logger.Error("Automatic saving without ftp password.");
                actionResult.Add(ErrorCode.Ftp_AutoSaveWithoutPassword);
            }

            if (!isFinal && TokenIdentifier.ContainsTokens(profile.Ftp.Directory))
                return actionResult;

            if (!ValidName.IsValidFtpPath(profile.Ftp.Directory))
                actionResult.Add(ErrorCode.FtpDirectory_InvalidFtpPath);

            return actionResult;
        }

        private bool KeyFilePasswordIsRequired(FtpAccount ftpAccount)
        {
            return ftpAccount.AuthenticationType == AuthenticationType.KeyFileAuthentication
                   && ftpAccount.KeyFileRequiresPass
                && string.IsNullOrEmpty(ftpAccount.Password);
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

        protected override void SetPassword(Job job, string password)
        {
            job.Passwords.FtpPassword = password;
        }

        public override bool IsEnabled(ConversionProfile profile)
        {
            return profile.Ftp.Enabled;
        }
    }
}
