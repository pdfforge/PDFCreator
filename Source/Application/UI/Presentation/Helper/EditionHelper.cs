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
        public bool IsTerminalServer { get; }
        public bool IsCustom { get; }
        public EncryptionLevel EncryptionLevel { get; }
        public bool AlwaysUsePdfArchitect { get; }

        public EditionHelper(Edition edition, EncryptionLevel encryptionLevel = EncryptionLevel.Aes256Bit, bool alwaysUsePdfArchitect = true)
        {
            IsFreeEdition = edition == Edition.Free;
            IsServer = edition == Edition.Server;
            IsCustom = edition == Edition.Custom;
            IsTerminalServer = edition == Edition.TerminalServer;
            EncryptionLevel = encryptionLevel;
            AlwaysUsePdfArchitect = alwaysUsePdfArchitect;
        }
    }
}
