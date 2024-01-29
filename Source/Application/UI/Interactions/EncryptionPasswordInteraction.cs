using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class EncryptionPasswordInteraction : IInteraction
    {
        public EncryptionPasswordInteraction(bool skip, bool askOwnerPw, bool askUserPw)
        {
            Skip = skip;
            AskOwnerPassword = askOwnerPw;
            AskUserPassword = askUserPw;
        }

        public bool Skip { get; set; }
        public bool AskOwnerPassword { get; set; }
        public bool AskUserPassword { get; set; }
        public string OwnerPassword { get; set; }
        public string UserPassword { get; set; }
        public bool IsAutoSaveMode { get; set; }
        public PasswordResult Response { get; set; } = PasswordResult.Cancel;
    }
}
