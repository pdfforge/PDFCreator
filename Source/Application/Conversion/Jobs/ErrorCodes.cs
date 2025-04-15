﻿using Translatable;

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

        [Translation("No compatible email client installed.")]
        MailClient_NoCompatibleEmailClientInstalled = 11101,

        [Translation("At least one of the attachment files for the mail client action is unavailable.")]
        MailClient_InvalidAttachmentFiles = 11102,

        [Translation("User tokens in mail client recipient require the user token action to be enabled.")]
        MailClient_Recipients_RequiresUserToken = 11103,

        [Translation("User tokens in mail client recipient cc require the user token action to be enabled.")]
        MailClient_RecipientsCc_RequiresUserToken = 11104,

        [Translation("User tokens in mail client recipient bcc require the user token action to be enabled.")]
        MailClient_RecipientsBcc_RequiresUserToken = 11105,

        [Translation("User tokens in mail client subject require the user token action to be enabled.")]
        MailClient_Subject_RequiresUserToken = 11106,

        [Translation("User tokens in mail client content require the user token action to be enabled.")]
        MailClient_Content_RequiresUserToken = 11107,

        [Translation("User tokens in the additional mail client attachment files require the user token action to be enabled.")]
        MailClient_AdditionalAttachment_RequiresUserToken = 11108,

        [Translation("Unknown error in email client action.")]
        MailClient_GenericError = 11999,

        [Translation("No certificate file is specified.")]
        Signature_NoCertificationFile = 12100,

        [Translation("The certificate file is unavailable.")]
        Signature_CertificateFileDoesNotExist = 12101,

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

        [Translation("Time server account not set or not available.\nPlease check your configured accounts.")]
        Signature_NoTimeServerAccount = 12207,

        [Translation("The certificate file is unavailable.")]
        Signature_FileNotFound = 12208,

        [Translation("The path to the signature image file has to be a valid absolute path, e.g. 'C:\\Images\\signature.png'")]
        Signature_ImageFileInvalidRootedPath = 12209,

        [Translation("The path to the signature image file is too long.")]
        Signature_ImageFilePathTooLong = 12210,

        [Translation("The signature image file is unavailable.")]
        Signature_ImageFileDoesNotExist = 12212,

        [Translation("The signature image file type is not supported.")]
        Signature_ImageFileUnsupportedType = 12213,

        [Translation("The signature font is unavailable.")]
        Signature_FontNotFound = 12214,

        [Translation("User tokens in signature reason path require the user token action to be enabled.")]
        Signature_Reason_RequiresUserTokens = 12215,

        [Translation("User tokens in signature contact path require the user token action to be enabled.")]
        Signature_Contact_RequiresUserTokens = 12216,

        [Translation("User tokens in signature location require the user token action to be enabled.")]
        Signature_Location_RequiresUserTokens = 12217,

        [Translation("No signature image file is specified.")]
        Signature_ImageFileNotSpecified = 12218,

        [Translation("User tokens in signature image file path require the user token action to be enabled.")]
        Signature_ImageFile_RequiresUserTokens = 12219,

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

        [Translation("No program file is specified.")]
        RunProgram_NoScriptFileSpecified = 14100,

        [Translation("The program file is unavailable.")]
        RunProgram_FileDoesNotExist = 14101,

        [Translation("The program file path is an invalid absolute path.")]
        RunProgram_InvalidRootedPath = 14103,

        [Translation("The program file path is too long.")]
        RunProgram_PathTooLong = 14104,

        [Translation("User tokens in program file path require the user token action to be enabled.")]
        RunProgram_ScriptFile_RequiresUserTokens = 14105,

        [Translation("Error while running the script action.")]
        RunProgram_GenericError = 14999,

        [Translation("No SMTP email address is specified.")]
        Smtp_NoEmailAddress = 15100,

        [Translation("No SMTP email recipients are specified.")]
        Smtp_NoRecipients = 15101,

        [Translation("No SMTP email server is specified.")]
        Smtp_NoServerSpecified = 15102,

        [Translation("Invalid SMTP port.")]
        Smtp_InvalidPort = 15103,

        [Translation("No SMTP email user name is specified.")]
        Smtp_NoUserSpecified = 15104,

        [Translation("SMTP account not set or not available.\nPlease check your configured accounts.")]
        Smtp_NoAccount = 15105,

        [Translation("Missing password for email via SMTP.")]
        Smtp_NoPasswordSpecified = 15111,

        [Translation("Automatic saving requires setting the password for email via SMTP.")]
        Smtp_AutoSaveWithoutPassword = 15112,

        [Translation("The email via SMTP could not be delivered to one or more recipients.")]
        Smtp_EmailNotDelivered = 15106,

        [Translation("Could not authorize to SMTP server.")]
        Smtp_AuthenticationDenied = 15107,

        [Translation("User cancelled retyping SMTP email password.")]
        Smtp_UserCancelled = 15108,

        [Translation("Invalid SMTP email recipients.")]
        Smtp_InvalidRecipients = 15109,

        [Translation("At least one of the attachment files for the SMTP action is unavailable.")]
        Smtp_InvalidAttachmentFiles = 15110,

        [Translation("Could not reach the SMTP server. Check the hostname and port configuration.")]
        Smtp_ConnectionError = 15113,

        [Translation("User tokens in SMTP 'on behalf of' require the user token action to be enabled.")]
        Smtp_OnBehalfOf_RequiresUserToken = 15114,

        [Translation("User tokens in SMTP display name require the user token action to be enabled.")]
        Smtp_DisplayName_RequiresUserToken = 15115,

        [Translation("User tokens in SMTP recipient require the user token action to be enabled.")]
        Smtp_ReplyTo_RequiresUserToken = 15116,

        [Translation("User tokens in SMTP recipient require the user token action to be enabled.")]
        Smtp_Recipients_RequiresUserToken = 15117,

        [Translation("User tokens in SMTP recipient cc require the user token action to be enabled.")]
        Smtp_RecipientsCc_RequiresUserToken = 15118,

        [Translation("User tokens in SMTP recipient bcc require the user token action to be enabled.")]
        Smtp_RecipientsBcc_RequiresUserToken = 15119,

        [Translation("User tokens in SMTP subject require the user token action to be enabled.")]
        Smtp_Subject_RequiresUserToken = 15120,

        [Translation("User tokens in SMTP content require the user token action to be enabled.")]
        Smtp_Content_RequiresUserToken = 15121,

        [Translation("User tokens in the additional SMTP attachment files require the user token action to be enabled.")]
        Smtp_AdditionalAttachment_RequiresUserToken = 15122,

        [Translation("Error while sending email via SMTP.")]
        Smtp_GenericError = 15999,

        [Translation("No background file is specified.")]
        Background_NoFileSpecified = 17100,

        [Translation("The background file is unavailable.")]
        Background_FileDoesNotExist = 17101,

        [Translation("The background file is not a PDF file.")]
        Background_NoPdf = 17102,

        [Translation("Unable to add the background. The PDF format versions are not compatible.")]
        Background_ConformanceError = 17103,

        [Translation("Unable to add the background. The background PDF file may not be password protected.")]
        Background_BadPasswordError = 17104,

        [Translation("User tokens in background file path require the user token action to be enabled.")]
        Background_RequiresUserTokens = 17105,

        [Translation("Error while adding background to the document.")]
        Background_GenericError = 17999,

        [Translation("The path to the background file has to be a valid absolute path, e.g. 'C:\\documents\\background.pdf'")]
        Background_InvalidRootedPath = 36001,

        [Translation("The background file path is too long.")]
        Background_PathTooLong = 36002,

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

        [Translation("FTP account not set or not available.\nPlease check your configured accounts.")]
        Ftp_NoAccount = 18111,

        [Translation("The FTP server URL has an invalid format.")]
        Ftp_InvalidServerFormat = 18112,

        [Translation("Error while uploading file to the FTP server.")]
        Ftp_GenericError = 18999,

        [Translation("Dropbox account not set or not available.\nPlease check your configured accounts.")]
        Dropbox_AccountNotSpecified = 19001,

        [Translation("The specified Dropbox access token is not configured.")]
        Dropbox_AccessTokenNotSpecified = 19002,

        [Translation("These characters are not allowed as Dropbox directory name: < > : ? * \" |")]
        Dropbox_InvalidDirectoryName = 19003,

        [Translation("User tokens in Dropbox directory require the user token action to be enabled.")]
        Dropbox_SharedFolder_RequiresUserTokens = 19004,

        [Translation("Automatic saving requires setting the file name template.")]
        AutoSave_NoFilenameTemplate = 21101,

        [Translation("The file name template contains illegal characters.")]
        FilenameTemplate_IllegalCharacters = 21102,

        [Translation("User tokens in file name template require the user token action to be enabled.")]
        FilenameTemplate_RequiresUserTokens = 21103,

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

        [Translation("Unable to add one of the cover files. The PDF format versions are not compatible.")]
        Cover_ConformanceError = 22106,

        [Translation("User tokens in the cover file paths require the user token action to be enabled.")]
        Cover_RequiresUserTokens = 22107,

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

        [Translation("User tokens in the attachment file paths require the user token action to be enabled.")]
        Attachment_RequiresUserTokens = 23107,

        [Translation("Error while adding attachment to the document.")]
        Attachment_GenericError = 23999,

        [Translation("No stamp text is specified.")]
        Stamp_NoText = 24100,

        [Translation("The stamp font is unavailable.")]
        Stamp_FontNotFound = 24101,

        [Translation("User tokens in stamp text require the user token action to be enabled.")]
        Stamp_RequiresUserTokens = 24102,

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

        [Translation("HTTP account not set or not available.\nPlease check your configured accounts.")]
        HTTP_NoAccount = 32104,

        [Translation("User cancelled retyping HTTP authentication password.")]
        HTTP_UserCancelled = 32105,

        [Translation("The HTTP URL must start with 'http://' or 'https://'.")]
        HTTP_MustStartWithHttp = 32106,

        [Translation("Error while uploading to HTTP server. 401 interaction is not authorized.")]
        HTTP_UnAuthorized_Request_Error = 32107,

        [Translation("User tokens in HTTP account URL require the user token action to be enabled.")]
        HTTP_AccountUrl_RequiresUserToken = 32108,

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

        [Translation("User tokens in target directory require the user token action to be enabled.")]
        TargetDirectory_RequiresUserTokens = 34005,

        [Translation("The certificate file path is invalid absolute path.")]
        Signature_CertificateFile_InvalidRootedPath = 38001,

        [Translation("The certificate file path is too long.")]
        Signature_CertificateFile_TooLong = 38002,

        [Translation("User tokens in certificate file path require the user token action to be enabled.")]
        Signature_CertificateFile_RequiresUserTokens = 38004,

        [Translation("The FTP directory is an invalid FTP path.")]
        Ftp_Directory_InvalidFtpPath = 40001,

        [Translation("The path to the FTP key file has to be a valid absolute path.")]
        Ftp_KeyFilePath_InvalidRootedPath = 40003,

        [Translation("The FTP key file path is too long.")]
        Ftp_KeyFilePath_PathTooLong = 40004,

        [Translation("The FTP key file is unavailable.")]
        Ftp_KeyFilePath_FileDoesNotExist = 40006,

        [Translation("User tokens in FTP directory require the user token action to be enabled.")]
        Ftp_Directory_RequiresUserToken = 40007,

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

        [Translation("The file path is too long.")]
        FilePath_TooLong = 42002,

        [Translation("The file path is not set.")]
        FilePath_NullOrEmpty = 42003,

        [Translation("No CS-Script file is specified.")]
        CustomScript_NoScriptFileSpecified = 43001,

        [Translation("The CS-Script file does not exist in the program data directory 'CS-Scripts' folder.")]
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

        [Translation("The path to the watermark file has to be a valid absolute path, e.g. 'C:\\documents\\watermark.pdf'")]
        Watermark_InvalidRootedPath = 47003,

        [Translation("The watermark file path is too long.")]
        Watermark_PathTooLong = 47004,

        [Translation("The watermark file is unavailable.")]
        Watermark_FileDoesNotExist = 47006,

        [Translation("Unable to add the watermark. The PDF format versions are not compatible.")]
        Watermark_ConformanceError = 47007,

        [Translation("Unable to add the watermark. The watermark PDF file may not be password protected.")]
        Watermark_BadPassword = 47008,

        [Translation("The watermark file type is not supported.")]
        Watermark_UnsupportedType = 47009,

        [Translation("User tokens in watermark file path require the user token action to be enabled.")]
        Watermark_RequiresUserTokens = 47010,

        [Translation("Error while adding the watermark to the document.")]
        Watermark_GenericError = 47999,

        [Translation("No page number format is specified.")]
        PageNumbers_NoFormat = 48000,

        [Translation("The page number format does not include the <PageNumber> token.")]
        PageNumbers_NoPageNumberInFormat = 48001,

        [Translation("The page number font is unavailable.")]
        PageNumbers_FontNotFound = 48002,

        [Translation("User tokens in page number format require the user token action to be enabled.")] 
        PageNumbers_RequiresUserTokens = 48003,

        [Translation("Error while adding page numbers to the document.")]
        PageNumbers_GenericError = 48999,

        [Translation("The permissions for your Microsoft account have expired.\nPlease request them again in the Accounts tab.")]
        Microsoft_Account_Expired = 49501,

        [Translation("The upload to OneDrive failed.")]
        OneDrive_Upload_Failed = 49201,

        [Translation("User tokens in the OneDrive shared folder path require the user token action to be enabled.")]
        OneDrive_SharedFolder_RequiresUserTokenAction = 49202,

        [Translation("The provided OneDrive shared folder path is not a valid path.")]
        OneDrive_InvalidSharedFolder = 49203,

        [Translation("You don't have the permissions to upload to OneDrive.\nPlease check your Microsoft account permissions.")]
        OneDrive_MissingPermissions = 49204,

        [Translation("A OneDrive share link to the file could not be created.")]
        OneDrive_CouldNotCreateShareLink = 49205,

        [Translation("Error while uploading the document to Outlook.")]
        Outlook_Web_Upload_Error = 49400,

        [Translation("Microsoft account not set or not available.\nPlease check your configured accounts.")]
        Microsoft_Account_Missing = 49401,

        [Translation("Could not create draft on account.")]
        Outlook_Web_Mail_Create = 49403,

        [Translation("Too many attachment. The limit to attachments is 100.")]
        Outlook_Web_Attachment_Limit_Count = 49404,

        [Translation("Too big. Outlook only allows for 150mb of attachments.")]
        Outlook_Web_Attachment_Limit_Size = 49405,

        [Translation("User tokens in Outlook Web Access recipient require the user token action to be enabled.")]
        Outlook_Web_Recipients_RequiresUserToken = 49406,

        [Translation("User tokens in Outlook Web Access recipient cc require the user token action to be enabled.")]
        Outlook_Web_RecipientsCc_RequiresUserToken = 49407,

        [Translation("User tokens in Outlook Web Access recipient bcc require the user token action to be enabled.")]
        Outlook_Web_RecipientsBcc_RequiresUserToken = 49408,

        [Translation("User tokens in Outlook Web Access subject require the user token action to be enabled.")]
        Outlook_Web_Subject_RequiresUserToken = 49410,

        [Translation("User tokens in Outlook Web Access content require the user token action to be enabled.")]
        Outlook_Web_Content_RequiresUserToken = 49411,

        [Translation("User tokens in the additional Outlook Web Access attachment files require the user token action to be enabled.")]
        Outlook_Web_AdditionalAttachment_RequiresUserToken = 49412,

        [Translation("At least one of the additional Outlook Web Access attachment files is unavailable.")]
        Outlook_Web_InvalidAttachmentFiles = 49413,

        [Translation("You don't have the permissions to read and write emails.\nPlease check your Microsoft account permissions.")]
        Outlook_Web_MissingReadWritePermissions = 49414,

        [Translation("You don't have the permissions to send emails directly.\nPlease check your Microsoft account permissions.")]
        Outlook_Web_MissingSendPermission = 49415,


        [Translation("The upload to SharePoint failed.")]
        Sharepoint_Upload_Failed = 49502,
        [Translation("You don't have selected an account. Please select a valid account to configure the SharePoint action.")]
        Sharepoint_Missing_Account = 49503,
        [Translation("You don't have selected a site. Please select a valid site to configure the SharePoint action.")]
        Sharepoint_Missing_Site = 49504,
        [Translation("You don't have selected a drive. Please select a valid drive to configure the SharePoint action.")]
        Sharepoint_Missing_Drive = 49505,
        [Translation("You don't have the permissions to upload to a SharePoint drive.\nPlease check your Microsoft account permissions.")]
        Sharepoint_MissingPermissions = 49506,

        [Translation("User tokens in the SharePoint shared folder path require the user token action to be enabled.")]
        Sharepoint_SharedFolder_RequiresUserTokenAction = 49207,

        [Translation("The provided SharePoint shared folder path is not a valid path.")]
        Sharepoint_InvalidSharedFolder = 49208,

        [Translation("General error during Outlook Web Access action.")]
        Outlook_Web_General_Error = 49999,

        [Translation("User tokens in metadata title require the user token action to be enabled.")]
        Metadata_Title_RequiresUserToken = 50000,

        [Translation("User tokens in metadata author require the user token action to be enabled.")]
        Metadata_Author_RequiresUserToken = 50001,

        [Translation("User tokens in metadata subject require the user token action to be enabled.")]
        Metadata_Subject_RequiresUserToken = 50002,

        [Translation("User tokens in metadata keywords require the user token action to be enabled.")]
        Metadata_Keywords_RequiresUserToken = 50003,

        [Translation("The output document has no pages. The conversion is aborted.")]
        UserToken_NoPagesInOutputDocument = 60000,
    }
}
