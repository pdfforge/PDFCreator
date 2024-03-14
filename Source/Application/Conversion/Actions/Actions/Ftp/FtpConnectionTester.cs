using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp
{
    public interface IFtpConnectionTester
    {
        bool TestFtpConnection(FtpAccount ftpAccount);

        ActionResult CheckAccount(FtpAccount ftpAccount, bool ignoreMissingPassword);
    }

    public class FtpConnectionTester : IFtpConnectionTester
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFtpConnectionFactory _ftpConnectionFactory;
        private readonly IPathUtil _pathUtil;
        private readonly IFile _file;

        public FtpConnectionTester(IFtpConnectionFactory ftpConnectionFactory, IPathUtil pathUtil, IFile file)
        {
            _ftpConnectionFactory = ftpConnectionFactory;
            _pathUtil = pathUtil;
            _file = file;
        }

        public bool TestFtpConnection(FtpAccount ftpAccount)
        {
            _logger.Debug("Creating ftp connection.\r\nServer: " + ftpAccount.Server + "\r\nUsername: " + ftpAccount.UserName);
            var ftpClient = _ftpConnectionFactory.BuildConnection(ftpAccount, ftpAccount.Password);

            try
            {
                ftpClient.Connect();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception while logging in to ftp server: {ftpAccount.Server}");
                return false;
            }
            finally
            {
                ftpClient.Disconnect();
            }
        }

        public ActionResult CheckAccount(FtpAccount ftpAccount, bool ignoreMissingPassword)
        {
            var actionResult = new ActionResult();

            if (string.IsNullOrEmpty(ftpAccount.Server))
            {
                _logger.Error("No FTP server specified.");
                actionResult.Add(ErrorCode.Ftp_NoServer);
            }
            else if (!ftpAccount.IsHostValid() || !ftpAccount.IsPortValid())
            {
                _logger.Error("FTP server url has an invalid format.");
                actionResult.Add(ErrorCode.Ftp_InvalidServerFormat);
            }

            if (string.IsNullOrEmpty(ftpAccount.UserName))
            {
                _logger.Error("No FTP username specified.");
                actionResult.Add(ErrorCode.Ftp_NoUser);
            }

            if (ftpAccount.AuthenticationType == AuthenticationType.KeyFileAuthentication)
            {
                var pathUtilStatus = _pathUtil.IsValidRootedPathWithResponse(ftpAccount.PrivateKeyFile);
                switch (pathUtilStatus)
                {
                    case PathUtilStatus.InvalidRootedPath:
                        return new ActionResult(ErrorCode.Ftp_KeyFilePath_InvalidRootedPath);

                    case PathUtilStatus.PathTooLongEx:
                        return new ActionResult(ErrorCode.Ftp_KeyFilePath_PathTooLong);

                    case PathUtilStatus.NotSupportedEx:
                        return new ActionResult(ErrorCode.Ftp_KeyFilePath_InvalidRootedPath);

                    case PathUtilStatus.ArgumentEx:
                        return new ActionResult(ErrorCode.Ftp_KeyFilePath_IllegalCharacters);
                }

                if (ignoreMissingPassword && ftpAccount.PrivateKeyFile.StartsWith(@"\\"))
                    return new ActionResult();

                if (!_file.Exists(ftpAccount.PrivateKeyFile))
                {
                    _logger.Error("The private key file \"" + ftpAccount.PrivateKeyFile + "\" does not exist.");
                    return new ActionResult(ErrorCode.Ftp_KeyFilePath_FileDoesNotExist);
                }

                if (string.IsNullOrEmpty(ftpAccount.Password) && KeyFilePasswordIsRequired(ftpAccount))
                {
                    _logger.Error("Automatic saving without ftp password.");
                    actionResult.Add(ErrorCode.Ftp_AutoSaveWithoutPassword);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ftpAccount.Password) && !ignoreMissingPassword)
                {
                    _logger.Error("Automatic saving without ftp password.");
                    actionResult.Add(ErrorCode.Ftp_AutoSaveWithoutPassword);
                }
            }

            return actionResult;
        }

        private static bool KeyFilePasswordIsRequired(FtpAccount ftpAccount)
        {
            return ftpAccount.AuthenticationType == AuthenticationType.KeyFileAuthentication
                   && ftpAccount.KeyFileRequiresPass
                   && string.IsNullOrEmpty(ftpAccount.Password);
        }
    }
}
