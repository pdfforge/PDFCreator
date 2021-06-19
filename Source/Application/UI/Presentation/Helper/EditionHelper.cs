using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public enum Edition
    {
        Free,
        Professional,
        TerminalServer,
        Custom,
        Server
    }

    public class EditionHelper
    {
        public bool IsFreeEdition { get; }
        public bool IsServer { get; }
        public EncryptionLevel EncryptionLevel { get; }

        public EditionHelper(Edition edition, EncryptionLevel encryptionLevel = EncryptionLevel.Aes256Bit)
        {
            IsFreeEdition = edition == Edition.Free;
            IsServer = edition == Edition.Server;
            EncryptionLevel = encryptionLevel;
        }
    }
}
