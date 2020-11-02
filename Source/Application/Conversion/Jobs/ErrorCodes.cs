using Translatable;

// ReSharper disable InconsistentNaming

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    [Translatable]
    public enum ErrorCode
    {
        [Translation("PDF Architect could not open output file.")]
        Viewer_ArchitectCouldNotOpenOutput = 10100,

        [Translation("System could not open output file.")]
        Viewer_CouldNotOpenOutputFile = 10101,

        [Translation("No compatible e-mail client installed.")]
        MailClient_NoCompatibleEmailClientInstalled = 11101,

        [Translation("At least one of the attachment files for the mail client action is unavailable.")]
        MailClient_InvalidAttachmentFiles = 11102,

        [Translation("Unknown error in e-mail client action.")]
        MailClient_GenericError = 11999,

        [Translation("No certification file is specified.")]
        ProfileCheck_NoCertificationFile = 12100,

        [Translation("Certificate file does not exist.")]
        CertificateFile_CertificateFileDoesNotExist = 12101,

        [Translation("Automatic saving without certificate password.")]
        Signature_AutoSaveWithoutCertificatePassword = 12102,

        [Translation("Secured Time Server without user name.")]
        Signature_SecuredTimeServerWithoutUsername = 12103,

        [Translation("Secured Time Server without password.")]
        Signature_SecuredTimeServerWithoutPassword = 12104,

        [Translation("The certificate password is wrong.")]
        Signature_WrongCertificatePassword = 12200,

        [Translation("The certificate has no private key.")]
        Signature_NoPrivateKey = 12201,

        [Translation("Not enough space for signature.")]
        Signature_NotEnoughSpaceForSignature = 12202,

        [Translation("Not enough space for signature.")]
        Signature_ProfileCheck_NotEnoughSpaceForSignature = 12203,

        [Translation("Launched signing without certification password.")]
        Signature_LaunchedSigningWithoutPassword = 12204,

        [Translation("Error while signing. Can not connect to time server.")]
        Signature_NoTimeServerConnection = 12205,

        [Translation("Error while signing. The certificate is invalid or has expired.")]
        Signature_Invalid = 12206,

        [Translation("The specified time server account for signing is not configured.")]
        Signature_NoTimeServerAccount = 12207,

        [Translation("Unable to find the certification file.")]
        Signature_FileNotFound = 12208,

        [Translation("Error while signing the document.")]
        Signature_GenericError = 12999,

        [Translation("The default printer is invalid.")]
        Printing_InvalidDefaultPrinter = 13100,

        [Translation("The selected printer is invalid.")]
        Printing_InvalidSelectedPrinter = 13101,

        [Translation("Error while printing the file.")]
        Printing_GenericError = 13999,

        [Translation("No script file is specified.")]
        Script_NoScriptFileSpecified = 14100,

        [Translation("Script file does not exist.")]
        Script_FileDoesNotExist = 14101,

        [Translation("Script file contains illegal characters.")]
        Script_IllegalCharacters = 14102,

        [Translation("ScriptFile path is invalid absolute path.")]
        Script_InvalidRootedPath = 14103,

        [Translation("ScriptFile path is too long.")]
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

        [Translation("No password provided for e-mail over SMTP.")]
        Smtp_NoPasswordSpecified = 15111,

        [Translation("E-mail over SMTP could not be delivered to one or more recipients.")]
        Smtp_EmailNotDelivered = 15106,

        [Translation("Could not authorize to SMTP server.")]
        Smtp_AuthenticationDenied = 15107,

        [Translation("User cancelled retyping SMTP e-mail password.")]
        Smtp_UserCancelled = 15108,

        [Translation("Invalid SMTP e-mail recipients.")]
        Smtp_InvalidRecipients = 15109,

        [Translation("At least one of the attachment files for the SMTP action is unavailable.")]
        Smtp_InvalidAttachmentFiles = 15110,

        [Translation("Error while sending e-mail over SMTP.")]
        Smtp_GenericError = 15999,

        [Translation("No background file is specified.")]
        Background_NoFileSpecified = 17100,

        [Translation("Background file does not exist.")]
        Background_FileDoesNotExist = 17101,

        [Translation("Background file is no PDF file.")]
        Background_NoPdf = 17102,

        [Translation("Unable to add the background. The documents' PDF format version is not compatible.")]
        Background_ConformanceError = 17103,

        [Translation("Unable to add the background. The background PDF may not be password protected.")]
        Background_BadPasswordError = 17104,

        [Translation("Error while adding background to the document.")]
        Background_GenericError = 17999,

        [Translation("FTP server is not specified.")]
        Ftp_NoServer = 18100,

        [Translation("FTP server user name is not specified.")]
        Ftp_NoUser = 18101,

        [Translation("FTP server password is not specified.")]
        Ftp_NoPassword = 18103,

        [Translation("Could not login to FTP server.")]
        Ftp_LoginError = 18104,

        [Translation("Failure in directory on FTP server.")]
        Ftp_DirectoryError = 18105,

        [Translation("Could not read from FTP directory to ensure unique filenames.")]
        Ftp_DirectoryReadError = 18106,

        [Translation("Could not upload file to FTP.")]
        Ftp_UploadError = 18107,

        [Translation("Could not login to FTP server. Please check your Internet connection.")]
        Ftp_ConnectionError = 18108,

        [Translation("Automatic saving without FTP server password.")]
        Ftp_AutoSaveWithoutPassword = 18109,

        [Translation("User cancelled retyping FTP password.")]
        Ftp_UserCancelled = 18110,

        [Translation("The specified FTP account is not configured.")]
        Ftp_NoAccount = 18111,

        [Translation("Error while uploading file to FTP server.")]
        Ftp_GenericError = 18999,

        [Translation("The specified Dropbox account is not configured.")]
        Dropbox_AccountNotSpecified = 19001,

        [Translation("The specified Dropbox access token is not configured.")]
        Dropbox_AccessTokenNotSpecified = 19002,

        [Translation("These characters are not allowed as Dropbox directory name: < > : ? * \" |")]
        Dropbox_InvalidDirectoryName = 19003,

        [Translation("Automatic saving without filename template.")]
        AutoSave_NoFilenameTemplate = 21101,

        [Translation("Filename template contains illegal characters.")]
        FilenameTemplate_IllegalCharacters = 21102,

        [Translation("No cover file is specified.")]
        Cover_NoFileSpecified = 22100,

        [Translation("One of the cover files does not exist.")]
        Cover_FileDoesNotExist = 22101,

        [Translation("One of the cover files is no PDF file.")]
        Cover_NoPdf = 22102,

        [Translation("One of the cover paths is an invalid absolute path.")]
        Cover_InvalidRootedPath = 22103,

        [Translation("One of the cover paths is too long.")]
        Cover_PathTooLong = 22104,

        [Translation("One of the cover paths contains illegal characters.")]
        Cover_IllegalCharacters = 22105,

        [Translation("Unable to add one of the cover files. The documents' PDF format version is not compatible.")]
        Cover_ConformanceError = 22106,

        [Translation("Error while adding cover to the document.")]
        Cover_GenericError = 22999,

        [Translation("No attachment file is specified.")]
        Attachment_NoFileSpecified = 23100,

        [Translation("One of the attachment files does not exist.")]
        Attachment_FileDoesNotExist = 23101,

        [Translation("One of the attachment files is no PDF file.")]
        Attachment_NoPdf = 23102,

        [Translation("Unable to add one of the attachment files. The documents' PDF format version is not compatible.")]
        Attachment_ConformanceError = 23103,

        [Translation("One of the attachment paths is an invalid absolute path.")]
        Attachment_InvalidRootedPath = 23104,

        [Translation("One of the attachment paths is too long.")]
        Attachment_PathTooLong = 23105,

        [Translation("One of the attachment paths contains illegal characters.")]
        Attachment_IllegalCharacters = 23106,

        [Translation("Error while adding attachment to the document.")]
        Attachment_GenericError = 23999,

        [Translation("No stamp text is specified.")]
        Stamp_NoText = 24100,

        [Translation("Stamp font can't be found.")]
        Stamp_FontNotFound = 24101,

        [Translation("Error while stamping the document. Make sure that the selected font contains all glyphs used in the stamping text.")]
        Stamp_GenericError = 24999,

        [Translation("Automatic saving without owner password.")]
        AutoSave_NoOwnerPassword = 25100,

        [Translation("Automatic saving without user password.")]
        AutoSave_NoUserPassword = 25101,

        [Translation("Error while encrypting the document.")]
        Encryption_Error = 25200,

        [Translation("Launched encryption without owner password.")]
        Encryption_NoOwnerPassword = 25201,

        [Translation("Launched encryption without user password.")]
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

        [Translation("File path too long")]
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

        [Translation("Missing URL for HTTP upload.")]
        HTTP_NoUrl = 32101,

        [Translation("Missing user name for HTTP authentication.")]
        HTTP_NoUserNameForAuth = 32102,

        [Translation("Automatic saving without password for HTTP authentication.")]
        HTTP_NoPasswordForAuthWithAutoSave = 32103,

        [Translation("The specified HTTP account is not configured.")]
        HTTP_NoAccount = 32104,

        [Translation("User cancelled retyping HTTP autentication password.")]
        HTTP_UserCancelled = 32105,

        [Translation("HTTP URL must start with 'http://' or 'https://'.")]
        HTTP_MustStartWithHttp = 32106,

        [Translation("Error while uploading to HTTP server. 401 interaction is not authorized.")]
        HTTP_UnAuthorized_Request_Error = 32107,

        [Translation("Error while uploading to HTTP server.")]
        HTTP_Generic_Error = 32999,

        [Translation("Error while trying to log-in.")]
        PasswordAction_Login_Error = 33001,

        [Translation("Automatic saving without target directory.")]
        TargetDirectory_NotSetForAutoSave = 34001,

        [Translation("Target directory is invalid absolute path.")]
        TargetDirectory_InvalidRootedPath = 34002,

        [Translation("Target directory is too long.")]
        TargetDirectory_TooLong = 34003,

        [Translation("Target directory contains illegal characters.")]
        TargetDirectory_IllegalCharacters = 34004,

        [Translation("Background file path is invalid rooted path.")]
        Background_InvalidRootedPath = 36001,

        [Translation("Background path is too long.")]
        Background_PathTooLong = 36002,

        [Translation("Background path contains illegal characters.")]
        Background_IllegalCharacters = 36003,

        [Translation("Certificate path is invalid absolute path.")]
        CertificateFile_InvalidRootedPath = 38001,

        [Translation("Certificate path is too long.")]
        CertificateFile_TooLong = 38002,

        [Translation("Certificate path contains illegal characters.")]
        CertificateFile_IllegalCharacters = 38003,

        [Translation("Ftp directory path is invalid ftp path.")]
        FtpDirectory_InvalidFtpPath = 40001,

        [Translation("Ftp key file path is invalid path.")]
        FtpKeyFilePath_InvalidKeyFilePath = 40002,

        [Translation("Ftp key file path is invalid rooted path.")]
        FtpKeyFilePath_InvalidRootedPath = 40003,

        [Translation("Ftp key file path is too long.")]
        FtpKeyFilePath_PathTooLong = 40004,

        [Translation("Ftp key file path contains illegal characters.")]
        FtpKeyFilePath_IllegalCharacters = 40005,

        [Translation("Ftp key file does not exist.")]
        FtpKeyFilePath_FileDoesNotExist = 40006,

        [Translation("Custom viewer was not found.")]
        DefaultViewer_Not_Found = 41000,

        [Translation("Path for PDF viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Pdf = 41001,

        [Translation("File for PDF viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Pdf = 41002,

        [Translation("Path for JPEG viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Jpeg = 41003,

        [Translation("File for JPEG viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Jpeg = 41004,

        [Translation("Path for PNG viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Png = 41005,

        [Translation("File for PNG viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Png = 41006,

        [Translation("Path for TIFF viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Tif = 41007,

        [Translation("File for TIFF viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Tif = 41008,

        [Translation("Path for Text viewer is empty.")]
        DefaultViewer_PathIsEmpty_for_Txt = 41009,

        [Translation("File for Text viewer does not exist.")]
        DefaultViewer_FileDoesNotExist_For_Txt = 41010,

        [Translation("The path is not valid. Please enter a valid absolute path.")]
        FilePath_InvalidRootedPath = 42000,

        [Translation("The file path is not valid or empty. Please enter a valid path. \nThe file path must not contain any of the following characters: \n" + @" \ / : * ? \" + "< >")]
        FilePath_InvalidCharacters = 42001,

        //todo: better translation
        //Replace {0} with fix value for _pathUtil.MAX_PATH or find a way to set it dynamically
        //[Translation("The path to the file is longer than the maximum of {0} characters allowed, please choose a shorter file path.")]
        [Translation("File path too long")]
        FilePath_TooLong = 42002,

        [Translation("No CS-script file is specified.")]
        CustomScript_NoScriptFileSpecified = 43001,

        [Translation("CS-script file does not exist in the program directory 'CS-Scripts' folder.")]
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

        [Translation("Error during forward to further profile action.")]
        ForwardToFurtherProfile_GeneralError = 46000,

        [Translation("The forward to further profile action forwards to itself.")]
        ForwardToFurtherProfile_ForwardToItself = 46001,

        [Translation("The forward to further profile action is linked to an unknown profile.")]
        ForwardToFurtherProfile_UnknownProfile = 46002,

        [Translation("The forward to further profile action causes a circular dependency between the profiles.")]
        ForwardToFurtherProfile_CircularDependency = 46003,

        [Translation("No watermark file is specified.")]
        Watermark_NoFileSpecified = 47001,

        [Translation("Watermark file is no PDF file.")]
        Watermark_NoPdf = 47002,

        [Translation("Watermark file path is invalid rooted path.")]
        Watermark_InvalidRootedPath = 47003,

        [Translation("Watermark file path is too long.")]
        Watermark_PathTooLong = 47004,

        [Translation("Watermark path contains illegal characters.")]
        Watermark_IllegalCharacters = 47005,

        [Translation("Watermark file does not exist.")]
        Watermark_FileDoesNotExist = 47006,

        [Translation("Unable to add the watermark. The documents' PDF format version is not compatible.")]
        Watermark_ConformanceError = 47007,

        [Translation("Unable to add the watermark. The watermark PDF may not be password protected.")]
        Watermark_BadPassword = 47008,

        [Translation("Error while adding watermark to the document.")]
        Watermark_GenericError = 47999,
    }
}
