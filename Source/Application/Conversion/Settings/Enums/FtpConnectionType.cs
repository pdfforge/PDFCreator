using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum FtpConnectionType
    {
        [Translation("FTP - File Transfer Protocol")]
        Ftp,
        [Translation("SFTP - SSH File Transfer Protocol")]
        Sftp
    }
}
