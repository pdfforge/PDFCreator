using pdfforge.Obsidian.Interaction;
using System;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class InputInteraction : IInteraction
    {
        public InputInteraction(string title, string questionText, Func<string, InputValidation> isValidInput = null, bool showUacShield = false)
        {
            Title = title;
            QuestionText = questionText;
            IsValidInput = isValidInput;
            ShowUacShield = showUacShield;
        }

        public Func<string, InputValidation> IsValidInput { get; set; }

        public string Title { get; set; }
        public string InputText { get; set; }
        public string QuestionText { get; set; }

        public bool Success { get; set; }
        public bool ShowUacShield { get; set; }
    }
}
