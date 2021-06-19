using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken
{
    public class UserTokenTranslation : ActionTranslationBase
    {
        public string UserTokenTitle { get; private set; } = "Extract User Tokens";
        public string UserTokenIntroduction { get; private set; } = "User tokens allow you to define your own place holders for settings and set the values directly in your document.";
        public string UserTokenInDocumentText { get; private set; } = "Place a pattern like this on a separate line in your document";
        private string UserTokenDocumentExample { get; set; } = "NameDefinedByUser:Example Value";
        public string UserTokenInSettingsText { get; private set; } = "Use the value in the Profile Settings (e.g. for title or e-mail recipient) via the following token pattern";
        public string UserTokenSettingsExample { get; private set; } = "<User:NameDefinedByUser:Default Value>";
        public string DefaultValueExplanation { get; private set; } = "The default value will be used in case the user token is not defined in your document.";
        public string SelectSeparator { get; private set; } = "Select Separator:";
        public string UserGuideButtonText { get; private set; } = "More on user tokens";
        public EnumTranslation<UserTokenSeperator>[] UserTokenSeperatorValues { get; set; } = EnumTranslation<UserTokenSeperator>.CreateDefaultEnumTranslation();

        public string GetUserTokenDocumentExample(UserTokenSeperator userTokenSeperator)
        {
            switch (userTokenSeperator)
            {
                case UserTokenSeperator.AngleBrackets:
                    return "<<<" + UserTokenDocumentExample + ">>>";

                case UserTokenSeperator.CurlyBrackets:
                    return "{{{" + UserTokenDocumentExample + "}}}";

                case UserTokenSeperator.RoundBrackets:
                    return "(((" + UserTokenDocumentExample + ")))";

                case UserTokenSeperator.SquareBrackets:
                default:
                    return "[[[" + UserTokenDocumentExample + "]]]";
            }
        }

        public override string Title { get; set; } = "User Token";
        public override string InfoText { get; set; } = "Extracts values from the source document and uses them anywhere they are supported by PDFCreator.";
    }
}
