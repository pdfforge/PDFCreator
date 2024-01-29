using Translatable;

// ReSharper disable InconsistentNaming

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    [Translatable]
    public enum ErrorCode
    {
        [Translation("System could not open output file.")]
        Viewer_CouldNotOpenOutputFile = 10101,

        [Translation("Could not open output file with default viewer.")]
        Viewer_CouldNotOpenOutputFileWithDefaultViewer = 10102,

        [Translation("No compatible e-mail client installed.")]
        MailClient_NoCompatibleEmailClientInstalled = 11101,

        [Translation("At least one of the attachment files for the mail client action is unavailable.")]
        MailClient_InvalidAttachmentFiles = 11102,

        [Translation("Unknown error in e-mail client action.")]
        MailClient_GenericError = 11999,

        [Translation("No certificate file is specified.")]
        ProfileCheck_NoCertificationFile = 12100,

        [Translation("The certificate file is unavailable.")]
        CertificateFile_CertificateFileDoesNotExist = 12101,

        [Translation("Automatic saving requires setting the certificate password.")]
        Signature_AutoSaveWithoutCertificatePassword = 12102,

        [Translation("Missing user name for the secured time server.")]
        Signature_SecuredTimeServerWithoutUsername = 12103,

        [Translation("Missing password for secured time server.")]
        Signature_SecuredTimeServerWithoutPassword = 12104,

        [Translation("Incorrect certificate password.")]
        Signature_WrongCertificatePassword = 12200,

        [Translation("The certificate has no private key.")]
        Signature_NoPrivateKey = 12201,

        [Translation("Not enough space for signature.")]
        Signature_NotEnoughSpaceForSignature = 12202,

        [Translation("Not enough space for signature.")]
        Signature_ProfileCheck_NotEnoughSpaceForSignature = 12203,

        [Translation("Missing certificate password.")]
        Signature_LaunchedSigningWithoutPassword = 12204,

        [Translation("Error while signing. Can not connect to time server.")]
        Signature_NoTimeServerConnection = 12205,

        [Translation("Error while signing. The certificate is invalid or has expired.")]
        Signature_Invalid = 12206,

        [Translation("The specified time server account is not configured.")]
        Signature_NoTimeServerAccount = 12207,

        [Translation("The certificate file is unavailable.")]
        Signature_FileNotFound = 12208,

        [Translation("The path to the signature image file has to be an absolute path, e.g. 'C:\\Images\\signature.png'")]
        Signature_ImageFileInvalidRootedPath = 12209,

        [Translation("The path to the signature image file is too long.")]
        Signature_ImageFilePathTooLong = 12210,

        [Translation("The path to the signature image file contains illegal characters.")]
        Signature_ImageFileIllegalCharacters = 12211,

        [Translation("The signature image file is unavailable.")]
        Signature_ImageFileDoesNotExist = 12212,

        [Translation("The signature image file type is not supported.")]
        Signature_ImageFileUnsupportedType = 12213,

        [Translation("The signature font is unavailable.")]
        Signature_FontNotFound = 12214,

        [Translation("Error while signing the document.")]
        Signature_GenericError = 12999,

        [Translation("The default printer is unavailable.")]
        Printing_InvalidDefaultPrinter = 13100,

        [Translation("The selected printer is unavailable.")]
        Printing_InvalidSelectedPrinter = 13101,

        [Translation("The selected printer forwards to another PDFCreator printer and causes an infinite loop.")]
        Printing_CyclicDependencyError = 13102,

        [Translation("Error while printing the file.")]
        Printing_GenericError = 13999,

        [Translation("No script file is specified.")]
        Script_NoScriptFileSpecified = 14100,

        [Translation("The script file is unavailable.")]
        Script_FileDoesNotExist = 14101,

        [Translation("The script file path contains illegal characters.")]
        Script_IllegalCharacters = 14102,

        [Translation("The script file path is an invalid absolute path.")]
        Script_InvalidRootedPath = 14103,

        [Translation("The script file path is too long.")]
        Script_PathTooLong = 14104,

        [Translation("Error while running the script action.")]
        Script_GenericError = 14999,

        [Translation("No SMTP e-mail address is specified.")]
        Smtp_NoEmailAddress = 15100,

        [Translation("No SMTP e-mail recipients are specified.")]
        Smtp_NoRecipients = 15101,

        [Translation("No SMTP e-mail server is specified.")]
        Smtp_NoServerSpecified = 15102,

        [Translation("Invalid SMTP port.")]
        Smtp_InvalidPort = 15103,

        [Translation("No SMTP e-mail user name is specified.")]
        Smtp_NoUserSpecified = 15104,

        [Translation("The specified SMTP account is not configured.")]
        Smtp_NoAccount = 15105,

        [Translation("Missing password for e-mail via SMTP.")]
        Smtp_NoPasswordSpecified = 15111,

        [Translation("Automatic saving requires setting the password for e-mail via SMTP.")]
        Smtp_AutoSaveWithoutPassword = 15112,

        [Translation("The e-mail via SMTP could not be delivered to one or more recipients.")]
        Smtp_EmailNotDelivered = 15106,

        [Translation("Could not authorize to SMTP server.")]
        Smtp_AuthenticationDenied = 15107,

        [Translation("User cancelled retyping SMTP e-mail password.")]
        Smtp_UserCancelled = 15108,

        [Translation("Invalid SMTP e-mail recipients.")]
        Smtp_InvalidRecipients = 15109,

        [Translation("At least one of the attachment files for the SMTP action is unavailable.")]
        Smtp_InvalidAttachmentFiles = 15110,

        [Translation("Could not reach the SMTP server. Check the hostname and port configuration.")]
        Smtp_ConnectionError = 15113,

        [Translation("Error while sending e-mail via SMTP.")]
        Smtp_GenericError = 15999,

        [Translation("No background file is specified.")]
        Background_NoFileSpecified = 17100,

        [Translation("The background file is unavailable.")]
        Background_FileDoesNotExist = 17101,

        [Translation("The background file is not a PDF file.")]
        Background_NoPdf = 17102,

        [Translation("Unable to add the background. The PDF format versions are not compatible.")]
        Background_ConformanceError = 17103,

        [Translation("Unable to add the background. The background PDF may not be password protected.")]
        Background_BadPasswordError = 17104,

        [Translation("Error while adding background to the document.")]
        Background_GenericError = 17999,

        [Translation("The FTP server is not specified.")]
        Ftp_NoServer = 18100,

        [Translation("The FTP server user name is not specified.")]
        Ftp_NoUser = 18101,

        [Translation("The FTP server password is not specified.")]
        Ftp_NoPassword = 18103,

        [Translation("Could not login to FTP server.")]
        Ftp_LoginError = 18104,

        [Translation("Failure in directory on the FTP server.")]
        Ftp_DirectoryError = 18105,

        [Translation("Could not read from FTP server directory to ensure unique file names.")]
        Ftp_DirectoryReadError = 18106,

        [Translation("Could not upload file to the FTP server.")]
        Ftp_UploadError = 18107,

        [Translation("Could not login to the FTP server. Please check your internet connection.")]
        Ftp_ConnectionError = 18108,

        [Translation("Automatic saving requires setting the FTP server password.")]
        Ftp_AutoSaveWithoutPassword = 18109,

        [Translation("User cancelled retyping the FTP server password.")]
        Ftp_UserCancelled = 18110,

        [Translation("The specified FTP account is not configured.")]
        Ftp_NoAccount = 18111,

        [Translation("The FTP server URL has an invalid format.")]
        Ftp_InvalidServerFormat = 18112,

        [Translation("Error while uploading file to the FTP server.")]
        Ftp_GenericError = 18999,

        [Translation("The specified Dropbox account is not configured.")]
        Dropbox_AccountNotSpecified = 19001,

        [Translation("The specified Dropbox access token is not configured.")]
        Dropbox_AccessTokenNotSpecified = 19002,

        [Translation("These characters are not allowed as Dropbox directory name: < > : ? * \" |")]
        Dropbox_InvalidDirectoryName = 19003,

        [Translation("Automatic saving requires setting the file name template.")]
        AutoSave_NoFilenameTemplate = 21101,

        [Translation("The file name template contains illegal characters.")]
        FilenameTemplate_IllegalCharacters = 21102,

        [Translation("No cover file is specified.")]
        Cover_NoFileSpecified = 22100,

        [Translation("One of the cover files is unavailable.")]
        Cover_FileDoesNotExist = 22101,

        [Translation("One of the cover files is no PDF file.")]
        Cover_NoPdf = 22102,

        [Translation("One of the cover file paths is an invalid absolute path.")]
        Cover_InvalidRootedPath = 22103,

        [Translation("One of the cover file paths is too long.")]
        Cover_PathTooLong = 22104,

        [Translation("One of the cover file paths contains illegal characters.")]
        Cover_IllegalCharacters = 22105,

        [Translation("Unable to add one of the cover files. The PDF format versions are not compatible.")]
        Cover_ConformanceError = 22106,

        [Translation("Error while adding cover to the document.")]
        Cover_GenericError = 22999,

        [Translation("No attachment file is specified.")]
        Attachment_NoFileSpecified = 23100,

        [Translation("One of the attachment files is unavailable.")]
        Attachment_FileDoesNotExist = 23101,

        [Translation("One of the attachment files is no PDF file.")]
        Attachment_NoPdf = 23102,

        [Translation("Unable to add one of the attachment files. The PDF format versions are not compatible.")]
        Attachment_ConformanceError = 23103,

        [Translation("One of the attachment file paths is an invalid absolute path.")]
        Attachment_InvalidRootedPath = 23104,

        [Translation("One of the attachment file paths is too long.")]
        Attachment_PathTooLong = 23105,

        [Translation("One of the attachment file paths contains illegal characters.")]
        Attachment_IllegalCharacters = 23106,

        [Translation("Error while adding attachment to the document.")]
        Attachment_GenericError = 23999,

        [Translation("No stamp text is specified.")]
        Stamp_NoText = 24100,

        [Translation("The stamp font is unavailable.")]
        Stamp_FontNotFound = 24101,

        [Translation("Error while stamping the document. Make sure that the selected font contains all glyphs used in the stamping text.")]
        Stamp_GenericError = 24999,

        [Translation("Automatic saving requires setting the owner password.")]
        AutoSave_NoOwnerPassword = 25100,

        [Translation("Automatic saving requires setting the user password.")]
        AutoSave_NoUserPassword = 25101,

        [Translation("Automatic merging ís not available for non-PDF output formats.")]
        AutoSave_NonPdfAutoMerge = 25102,

        [Translation("Error while encrypting the document.")]
        Encryption_Error = 25200,

        [Translation("Missing owner password for encryption action.")]
        Encryption_NoOwnerPassword = 25201,

        [Translation("Missing user password for encryption action.")]
        Encryption_NoUserPassword = 25202,

        [Translation("Error while encrypting the document.")]
        Encryption_GenericError = 25999,

        [Translation("Missing output file for PDF processing.")]
        Processing_OutputFileMissing = 26100,

        [Translation("At least one PDF file is applied that does not conform to the PDF/A standard.")]
        Processing_ConformanceMismatch = 26200,

        [Translation("Error while processing the document.")]
        Processing_GenericError = 26999,

        [Translation("Error while copying the output file.")]
        Conversion_ErrorWhileCopyingOutputFile = 28200,

        [Translation("The file path is too long.")]
        Conversion_PathTooLong = 28201,

        [Translation("Internal Ghostscript error.")]
        Conversion_GhostscriptError = 29100,

        [Translation("You have printed a password-protected PDF file and Ghostscript ist not able to convert such files.")]
        Conversion_Ghostscript_PasswordProtectedPDFError = 29101,

        [Translation("Unknown internal error.")]
        Conversion_UnknownError = 29200,

        [Translation("Error during PDF/A conversion.")]
        Conversion_PdfAError = 30999,

        [Translation("Error while uploading the document to Dropbox.")]
        Dropbox_Upload_Error = 31201,

        [Translation("Error while uploading and sharing the document to Dropbox.")]
        Dropbox_Upload_And_Share_Error = 31202,

        [Translation("Missing or invalid URL for HTTP upload.")]
        HTTP_MissingOrInvalidUrl = 32101,

        [Translation("Missing user name for HTTP authentication.")]
        HTTP_NoUserNameForAuth = 32102,

        [Translation("Automatic saving requires setting the password for the HTTP authentication.")]
        HTTP_NoPasswordForAuthWithAutoSave = 32103,

        [Translation("The specified HTTP account is not configured.")]
        HTTP_NoAccount = 32104,

        [Translation("User cancelled retyping HTTP authentication password.")]
        HTTP_UserCancelled = 32105,

        [Translation("The HTTP URL must start with 'http://' or 'https://'.")]
        HTTP_MustStartWithHttp = 32106,

        [Translation("Error while uploading to HTTP server. 401 interaction is not authorized.")]
        HTTP_UnAuthorized_Request_Error = 32107,

        [Translation("Error while uploading to the HTTP server.")]
        HTTP_Generic_Error = 32999,

        //?????????????????????????????????????????????????????????????????????????????????????
        [Translation("Error while trying to log-in.")]
        PasswordAction_Login_Error = 33001,

        [Translation("Automatic saving requires setting the target directory.")]
        TargetDirectory_NotSetForAutoSave = 34001,

        [Translation("The target directory is an invalid absolute path.")]
        TargetDirectory_InvalidRootedPath = 34002,

        [Translation("The target directory is too long.")]
        TargetDirectory_TooLong = 34003,

        [Translation("The target directory contains illegal characters.")]
        TargetDirectory_IllegalCharacters = 34004,

        [Translation("The path to the background file has to be an absolute path, e.g. 'C:\\documents\\background.pdf'")]
        Background_InvalidRootedPath = 36001,

        [Translation("The background file path is too long.")]
        Background_PathTooLong = 36002,

        [Translation("The background file contains illegal characters.")]
        Background_IllegalCharacters = 36003,

        [Translation("The certificate file path is invalid absolute path.")]
        CertificateFile_InvalidRootedPath = 38001,

        [Translation("The certificate file path is too long.")]
        CertificateFile_TooLong = 38002,

        [Translation("The certificate file path contains illegal characters.")]
        CertificateFile_IllegalCharacters = 38003,

        [Translation("The FTP directory is an invalid FTP path.")]
        FtpDirectory_InvalidFtpPath = 40001,

        [Translation("The FTP key file path is an invalid path.")]
        FtpKeyFilePath_InvalidKeyFilePath = 40002,

        [Translation("The path to the FTP key file has to be an absolute path.")]
        FtpKeyFilePath_InvalidRootedPath = 40003,

        [Translation("The FTP key file path is too long.")]
        FtpKeyFilePath_PathTooLong = 40004,

        [Translation("The FTP key file path contains illegal characters.")]
        FtpKeyFilePath_IllegalCharacters = 40005,

        [Translation("The FTP key file is unavailable.")]
        FtpKeyFilePath_FileDoesNotExist = 40006,

        [Translation("The custom viewer was not found.")]
        DefaultViewer_Not_Found = 41000,

        [Translation("The path for the custom PDF viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Pdf = 41001,

        [Translation("The file for the custom PDF viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Pdf = 41002,

        [Translation("The path for the custom JPEG viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Jpeg = 41003,

        [Translation("The file for the custom JPEG viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Jpeg = 41004,

        [Translation("The path for the custom PNG viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Png = 41005,

        [Translation("The file for the custom PNG viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Png = 41006,

        [Translation("The path for the custom TIFF viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Tif = 41007,

        [Translation("The file for the custom TIFF viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Tif = 41008,

        [Translation("The path for the custom TXT viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Txt = 41009,

        [Translation("The file for the custom TXT viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Txt = 41010,

        [Translation("The file path is not valid. Please enter a valid absolute path.")]
        FilePath_InvalidRootedPath = 42000,

        [Translation("The file path is not valid or empty. Please enter a valid path.\nThe file path must not contain any of the following characters: \n" + @" \ / : * ? \" + "< >")]
        FilePath_InvalidCharacters = 42001,

        [Translation("The file path is too long.")]
        FilePath_TooLong = 42002,

        [Translation("The file path is not set.")]
        FilePath_NullOrEmpty = 42003,

        [Translation("No CS-script file is specified.")]
        CustomScript_NoScriptFileSpecified = 43001,

        [Translation("The CS-script file does not exist in the program data directory 'CS-Scripts' folder.")]
        CustomScript_FileDoesNotExistInScriptFolder = 43002,

        [Translation("Could not compile the CS-Script.")]
        CustomScript_ErrorDuringCompilation = 43004,

        [Translation("The pre-conversion CS-script aborted the print job.")]
        CustomScriptPreConversion_ScriptResultAbort = 44001,

        [Translation("Exception during pre-conversion CS-Script.")]
        CustomScriptPreConversion_Exception = 44002,

        [Translation("The post-conversion CS-script aborted the print job.")]
        CustomScriptPostConversion_ScriptResultAbort = 45001,

        [Translation("Exception during post-conversion CS-Script.")]
        CustomScriptPostConversion_Exception = 45002,

        [Translation("Error during forwarding action.")]
        ForwardToFurtherProfile_GeneralError = 46000,

        [Translation("The forwarding action forwards to itself.")]
        ForwardToFurtherProfile_ForwardToItself = 46001,

        [Translation("The forwarding action is linked to an unknown target profile.")]
        ForwardToFurtherProfile_UnknownProfile = 46002,

        [Translation("The forwarding action causes a circular dependency between the profiles that leads to an infinite loop.")]
        ForwardToFurtherProfile_CircularDependency = 46003,

        [Translation("No watermark file is specified.")]
        Watermark_NoFileSpecified = 47001,

        [Translation("The watermark file is no PDF file.")]
        Watermark_NoPdf = 47002,

        [Translation("The path to the watermark file has to be an absolute path, e.g. 'C:\\documents\\watermark.pdf'")]
        Watermark_InvalidRootedPath = 47003,

        [Translation("The watermark file path is too long.")]
        Watermark_PathTooLong = 47004,

        [Translation("The watermark file path contains illegal characters.")]
        Watermark_IllegalCharacters = 47005,

        [Translation("The watermark file is unavailable.")]
        Watermark_FileDoesNotExist = 47006,

        [Translation("Unable to add the watermark. The PDF format versions are not compatible.")]
        Watermark_ConformanceError = 47007,

        [Translation("Unable to add the watermark. The watermark PDF file may not be password protected.")]
        Watermark_BadPassword = 47008,

        [Translation("The watermark file type is not supported.")]
        Watermark_UnsupportedType = 47009,

        [Translation("Error while adding the watermark to the document.")]
        Watermark_GenericError = 47999,

        [Translation("No page number format is specified.")]
        PageNumbers_NoFormat = 48000,

        [Translation("The page number format does not include the <PageNumber> token.")]
        PageNumbers_NoPageNumberInFormat = 48001,

        [Translation("The page number font is unavailable.")]
        PageNumbers_FontNotFound = 48002,

        [Translation("Error while adding page numbers to the document.")]
        PageNumbers_GenericError = 48999,

        [Translation("Error while uploading the document to Outlook.")]
        Outlook_Web_Upload_Error = 49400,

        [Translation("Error no Outlook Web Access account configured.")]
        Outlook_Web_Account_Missing = 49401,

        [Translation("Could not create draft on account.")]
        Outlook_Web_Mail_Create = 49403,

        [Translation("Too many attachment. The limit to attachments is 100.")]
        Outlook_Web_Attachment_Limit_Count = 49404,

        [Translation("Too big. Outlook only allows for 150mb of attachments.")]
        Outlook_Web_Attachment_Limit_Size = 49405,

        [Translation("General error during Outlook Web Access action.")]
        Outlook_Web_General_Error = 49999,
    }
}
