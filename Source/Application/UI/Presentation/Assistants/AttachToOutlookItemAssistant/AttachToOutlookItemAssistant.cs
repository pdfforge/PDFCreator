using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System;
using System.Collections.Generic;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.AttachToOutlookItem;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IAttachToOutlookItemAssistant
    {
        IList<string> GetOutlookItemCaptions();
        void ExportToOutlookItem(string itemCaption, IList<string> attachmentFiles);
    }

    public class AttachToOutlookItemAssistant : IAttachToOutlookItemAssistant
    {
        private readonly IAttachToOutlookItem _attachToOutlookItem;
        private readonly IInteractionRequest _interactionRequest;
        private AttachToOutlookItemTranslation Translation { get; set; }

        public AttachToOutlookItemAssistant(ITranslationUpdater translationUpdater, IAttachToOutlookItem attachToOutlookItem, IInteractionRequest interactionRequest)
        {
            translationUpdater.RegisterAndSetTranslation(tf => Translation = tf.UpdateOrCreateTranslation(Translation));
            _attachToOutlookItem = attachToOutlookItem;
            _interactionRequest = interactionRequest;
        }

        public IList<string> GetOutlookItemCaptions()
        {
            return _attachToOutlookItem.GetOutlookItemCaptions();
        }

        public void ExportToOutlookItem(string itemCaption, IList<string> attachmentFiles)
        {
            var result = _attachToOutlookItem.ExportToOutlookItem(itemCaption, attachmentFiles);
            if (result == AttachToOutlookItemResult.Success)
                return;

            var title = Translation.AttachToOutlookItem;

            var text = "";
            switch (result)
            {
                case AttachToOutlookItemResult.NoOutlook:
                case AttachToOutlookItemResult.NoOpenItems:
                case AttachToOutlookItemResult.ItemCouldNotBeFound:
                    text = Translation.ItemCouldNotBeFound;
                    break;
                case AttachToOutlookItemResult.ErrorWhileAddingAttachment:
                    text = Translation.ErrorWhileAddingAttachment;
                    break;
            }
            
            text += Environment.NewLine + "\'" + itemCaption + "\'";
            var message = new MessageInteraction(text, title, MessageOptions.Ok, MessageIcon.Error);
            _interactionRequest.Raise(message);
        }
    }
}
