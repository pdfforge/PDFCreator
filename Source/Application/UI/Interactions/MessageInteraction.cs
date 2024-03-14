using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class MessageInteraction : IInteraction
    {
        public MessageInteraction(string text, string title, MessageOptions buttons, MessageIcon icon,
            ActionResultDict actionResultDict = null, string secondText = null, string thirdText = null)
        {
            Text = text;
            Title = title;
            Buttons = buttons;
            Icon = icon;
            ActionResultDict = actionResultDict;
            SecondText = secondText;
            ShowErrorRegions = true;
            ThirdText = thirdText;
        }

        public MessageInteraction(string text, string title, MessageOptions buttons, MessageIcon icon,
            string resultKey, ActionResult actionResult, string secondText = null, string thirdText = null)
            : this(text, title, buttons, icon)
        {
            ActionResultDict = new ActionResultDict { { resultKey, actionResult } };
            SecondText = secondText;
            ThirdText = thirdText;
        }

        public string Text { get; set; }
        public string Title { get; set; }
        public MessageOptions Buttons { get; set; }
        public MessageIcon Icon { get; set; }
        public ActionResultDict ActionResultDict { get; set; }
        public bool ShowErrorRegions { get; set; }
        public string SecondText { get; set; }

        public MessageResponse Response { get; set; } = MessageResponse.Cancel;
        public string ThirdText { get; set; }
    }
}
