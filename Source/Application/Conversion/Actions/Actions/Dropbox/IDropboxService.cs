using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox
{
    public interface IDropboxService
    {
        Uri GetAuthorizeUri(string appKey, string redirectUri);

        string ParseAccessToken(Uri uri);

        bool UploadFiles(string accessToken, string refreshToken, string folder, IEnumerable<string> listOfFilePaths, bool ensureUniqueFilenames, string baseFolderName);

        DropboxFileMetaData UploadFileWithSharing(string accessToken, string refreshToken, string folder, IEnumerable<string> listOfFilePaths, bool ensureUniqueFilenames, string baseFolderName);

        DropboxUserInfo GetDropUserInfo(string accessToken, string refreshToken);

        void RevokeToken(string accountAccessToken, string refreshToken);
    }
}
