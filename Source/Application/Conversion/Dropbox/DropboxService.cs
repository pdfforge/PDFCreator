using Dropbox.Api;
using Dropbox.Api.Files;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Dropbox
{
    public class DropboxService : IDropboxService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Uri GetAuthorizeUri(string appKey, string redirectUri)
        {
            return DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, appKey, new Uri(redirectUri), string.Empty);
        }

        public string ParseAccessToken(Uri uri)
        {
            if (IsErrorInUrl(uri))
                throw new ArgumentException(nameof(uri));
            // make handling if bereaer token is not reachable
            var result = DropboxOAuth2Helper.ParseTokenFragment(uri);

            return result.AccessToken;
        }

        #region Upload without sharing

        public bool UploadFiles(string accessToken, string folder, IEnumerable<string> listOfFilePaths, bool ensureUniqueFilenames, string baseFileName)
        {
            try
            {
                var dbxClient = MakeInstanceOfClient(accessToken);
                var fullFolder = GetFullFolderToUpload(folder, Path.GetFileNameWithoutExtension(baseFileName), listOfFilePaths.Count());
                foreach (var pathOfCurrentItem in listOfFilePaths)
                {
                    var currentFileName = Path.GetFileName(pathOfCurrentItem);

                    // path to upload is just /FileName
                    var currentFilePath = fullFolder + currentFileName;
                    // if folder to upload is not empty add it to beginning of pathToUpload.
                    //If it doesnt exists dropbox will create it

                    using (var mem = GetFileStream(pathOfCurrentItem))
                    {
                        if (ensureUniqueFilenames)
                            dbxClient.Files.UploadAsync(currentFilePath, WriteMode.Add.Instance, true, body: mem).Wait();
                        else
                            dbxClient.Files.UploadAsync(currentFilePath, WriteMode.Overwrite.Instance, true, body: mem).Wait();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while uploading files to DropBox: ");
                return false;
            }
        }

        #endregion Upload without sharing

        #region Upload with sharing

        public DropboxFileMetaData UploadFileWithSharing(string accessToken, string folder, IEnumerable<string> listOfFilePaths, bool ensureUniqueFilenames, string subFolderForMultipleFiles)
        {
            try
            {
                var result = new DropboxFileMetaData();
                var dbxClient = MakeInstanceOfClient(accessToken);
                var folderToUpload = GetFullFolderToUpload(folder, Path.GetFileNameWithoutExtension(subFolderForMultipleFiles), listOfFilePaths.Count());
                foreach (var pathOfCurrentItem in listOfFilePaths)
                {
                    var currentFileName = Path.GetFileName(pathOfCurrentItem);
                    var fullPuthToUpload = folderToUpload + currentFileName;
                    var fileMetaData = UploadOneFile(ensureUniqueFilenames, pathOfCurrentItem, dbxClient, fullPuthToUpload);
                    if (listOfFilePaths.Count() == 1)
                    {
                        result = MakeSharedLinksOfFile(dbxClient, fileMetaData);
                    }
                }
                if (string.IsNullOrEmpty(result.Filename) || string.IsNullOrEmpty(result.ShareUrl))
                {
                    result = MakeSharedLinksOfFolder(dbxClient, folderToUpload);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while uploading and sharing file to DropBox: ");
                return new DropboxFileMetaData();
            }
        }

        private FileMetadata UploadOneFile(bool ensureUniqueFilenames, string pathOfCurrentItem, DropboxClient dbxClient, string fullPuthToUpload)
        {
            using (var mem = GetFileStream(pathOfCurrentItem))
            {
                return UploadAndGetSharedLink(ensureUniqueFilenames, dbxClient, fullPuthToUpload, mem);
            }
        }

        private DropboxFileMetaData MakeSharedLinksOfFolder(DropboxClient dbxClient, string folder)
        {
            folder = folder.TrimEnd('/');
            var f = dbxClient.Sharing.ListSharedLinksAsync(folder).Result.Links;

            if (f.Count > 0)
                return new DropboxFileMetaData { Filename = folder, ShareUrl = f.First().Url };

            var x = dbxClient.Sharing.CreateSharedLinkWithSettingsAsync(folder).Result;

            return new DropboxFileMetaData { Filename = x.PathLower, ShareUrl = x.Url };
        }

        private DropboxFileMetaData MakeSharedLinksOfFile(DropboxClient dbxClient, FileMetadata uploadFileMethaData)
        {
            var sharedLinkExists = dbxClient.Sharing.ListSharedLinksAsync(uploadFileMethaData.PathDisplay).Result;
            if (sharedLinkExists.Links.Any())
            {
                return new DropboxFileMetaData { Filename = uploadFileMethaData.PathDisplay, ShareUrl = sharedLinkExists.Links.First().Url };
            }
            var sharedLink = dbxClient.Sharing.CreateSharedLinkWithSettingsAsync(uploadFileMethaData.PathDisplay).Result;
            return new DropboxFileMetaData { Filename = uploadFileMethaData.PathDisplay, ShareUrl = sharedLink.Url };
        }

        private static FileMetadata UploadAndGetSharedLink(bool ensureUniqueFilenames, DropboxClient dbxClient, string fullPuthToUpload,

        #endregion Upload with sharing

        FileStream mem)
        {
            FileMetadata uploadFileMethaData;
            if (ensureUniqueFilenames)
                uploadFileMethaData = dbxClient.Files.UploadAsync(fullPuthToUpload, WriteMode.Add.Instance, true, body: mem).Result;
            else
                uploadFileMethaData = dbxClient.Files.UploadAsync(fullPuthToUpload, WriteMode.Overwrite.Instance, true, body: mem).Result;
            return uploadFileMethaData;
        }

        #region Private members

        /// <summary>
        /// Creating folder for dropbox upload.
        /// If there is more than one file to upload
        /// Than upload to folder/commonstrings in output files
        /// </summary>
        /// <param name="mainFolder"></param>
        /// <param name="subFolderForMultipleFiles"></param>
        /// <param name="numberOfFiles"></param>
        /// <returns></returns>
        private string GetFullFolderToUpload(string mainFolder, string subFolderForMultipleFiles, int numberOfFiles)
        {
            var folder = "/";

            if (!string.IsNullOrWhiteSpace(mainFolder))
            {
                folder += mainFolder + "/";
            }
            if (!string.IsNullOrWhiteSpace(subFolderForMultipleFiles) && numberOfFiles > 1)
            {
                folder += subFolderForMultipleFiles + "/";
            }

            return folder;
        }

        public DropboxUserInfo GetDropUserInfo(string accessToken)
        {
            var currentUser = MakeInstanceOfClient(accessToken).Users.GetCurrentAccountAsync().Result;
            if (currentUser != null)
                return new DropboxUserInfo
                {
                    AccessToken = accessToken,
                    AccountId = currentUser.AccountId,
                    AccountInfo = currentUser.Email + " - " + currentUser.Name.DisplayName
                };
            return new DropboxUserInfo();
        }

        public void RevokeToken(string accountAccessToken)
        {
            using (var dbxClient = MakeInstanceOfClient(accountAccessToken))
            {
                dbxClient.Auth.TokenRevokeAsync().Wait();
            }
        }

        private FileStream GetFileStream(string fileUri)
        {
            return new FileStream(fileUri, FileMode.Open, FileAccess.Read);
        }

        private DropboxClient MakeInstanceOfClient(string accessToken)
        {
            return new DropboxClient(accessToken);
        }

        private bool IsErrorInUrl(Uri eUri)
        {
            var fragment = eUri.Fragment;
            if (string.IsNullOrWhiteSpace(fragment))
                throw new ArgumentException("The supplied uri doesn't contain a fragment", "redirectedUri");
            fragment = fragment.Trim('#');

            var errorDescription = string.Empty;
            var error = string.Empty;
            foreach (var pair in fragment.Split('&'))
            {
                var elements = pair.Split('=');
                if (elements.Length != 2)
                    continue;

                switch (elements[0])
                {
                    case "error_description":
                        errorDescription = Uri.UnescapeDataString(elements[1]);
                        break;

                    case "error":
                        error = Uri.UnescapeDataString(elements[1]);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(errorDescription) && !string.IsNullOrEmpty(error))
            {
                Logger.Error("Dropbox error: Error_description = " + errorDescription + " Error = " + error);
                return true;
            }
            return false;
        }

        #endregion Private members
    }
}
