using System;
using System.Collections.Generic;
using System.IO;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using Renci.SshNet;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp
{
    public class SftpClientWrap : IFtpClient
    {
        private readonly SftpClient _sftpClient;

        public SftpClientWrap(string host, string userName, string password, string keyFilePath, AuthenticationType authenticationType)
        {
            var methods = new List<AuthenticationMethod>();
            if (authenticationType == AuthenticationType.KeyFileAuthentication)
            {
                var keyFiles = new[] { new PrivateKeyFile(keyFilePath, password) };
                methods.Add(new PrivateKeyAuthenticationMethod(userName, keyFiles));
            }
            else
                methods.Add(new PasswordAuthenticationMethod(userName, password));

            var con = new ConnectionInfo(host, 22, userName, methods.ToArray());
            _sftpClient = new SftpClient(con);
        }

        public void Connect()
        {
            _sftpClient.Connect();
        }

        public void Disconnect()
        {
            _sftpClient.Disconnect();
        }

        public bool FileExists(string filePath)
        {
            return _sftpClient.Exists(filePath);
        }

        public void CreateDirectory(string path)
        {
            var currentDir = "";
            var directories = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var directory in directories)
            {
                if (currentDir == "")
                    currentDir = directory;
                else
                    currentDir += "/" + directory;

                if (!_sftpClient.Exists(currentDir))
                {
                    _sftpClient.CreateDirectory(currentDir);
                }
            }
        }

        public bool DirectoryExists(string directory)
        {
            return _sftpClient.Exists(directory);
        }

        public void UploadFile(string localFile, string remoteFile)
        {
            using (Stream file = File.OpenRead(localFile))
            {
                _sftpClient.UploadFile(file, remoteFile);
            }
        }
    }
}
