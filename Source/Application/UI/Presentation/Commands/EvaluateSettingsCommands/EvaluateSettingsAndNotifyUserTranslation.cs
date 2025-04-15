using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands
{
    public class EvaluateSettingsAndNotifyUserTranslation : ITranslatable
    {
        public string Settings { get; private set; } = "Settings";
        public string UnsavedChanges { get; private set; } = "You have unsaved changes.";
        public string HowToProceed { get; private set; } = "How do you want to proceed?";
        public string InvalidSettings { get; private set; } = "You have invalid settings.";
        public string InvalidSettingsWithUnsavedChanges { get; private set; } = "You have invalid settings with unsaved changes.";
        public string WantToSave { get; private set; } = "Do you want to save?";
        public string WantToSaveAnyway { get; private set; } = "Do you want to save anyway?";
        public string WantToProceedAnyway { get; private set; } = "Do you want to proceed anyway?";
        public string SavingRequired { get; private set; } = "To proceed you need to save your current settings.";
        public string DefaultViewer { get; private set; } = "Default Viewer";
        public string Error { get; private set; } = "Error";

        public string SettingsAreLoadingTitle { get; private set; } = "Settings are Loading";

        public string ApplicationLoadingSettings { get; private set; } = "The application is still in the process of loading settings.";
        public string CloseAnyway { get; private set; } = "Do you want to close the application anyway?";

    }
}
