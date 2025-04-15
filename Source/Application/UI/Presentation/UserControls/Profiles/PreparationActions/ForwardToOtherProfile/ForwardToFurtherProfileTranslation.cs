﻿namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.ForwardToOtherProfile
{
    public class ForwardToFurtherProfileTranslation : ActionTranslationBase
    {
        public virtual string DisplayName { get; protected set; } = "Forward to profile";
        public virtual string SelectProfile { get; protected set; } = "Select profile:";
        public override string Title { get; set; } = "Forward to profile";
        public override string InfoText { get; set; } = "Trigger another conversion by forwarding the original source document to another profile, e.g. to convert to another output format.";
    }
}
