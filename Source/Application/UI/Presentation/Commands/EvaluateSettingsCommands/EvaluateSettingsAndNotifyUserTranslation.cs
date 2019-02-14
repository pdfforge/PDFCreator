using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands
{
    public class EvaluateSettingsAndNotifyUserTranslation : ITranslatable
    {
        public string Settings { get; private set; } = "Settings";
        public string UnsavedChanges { get; private set; } = "You have unsaved changes.";
        public string InvalidSettings { get; private set; } = "You have invalid settings.";
        public string InvalidSettingsWithUnsavedChanges { get; private set; } = "You have invalid settings with unsaved changes.";
        public string WantToSave { get; private set; } = "Do you want to save?";
        public string WantToSaveAnyway { get; private set; } = "Do you want to save anyway?";
        public string WantToProceedAnyway { get; private set; } = "Do you want to proceed anyway?";
        public string SavingRequired { get; private set; } = "To proceed you need to save your current settings.";
        public string ChooseNoToRevert { get; private set; } = "Choose 'No' to revert the changes.";
        public string DefaultViewer { get; private set; } = "Default Viewer";
        public string Error { get; private set; } = "Error";
        public string NoPrinterMapping { get; private set; } = "No queue is mapped to a printer.\nPlease add a new queue or select a printer for an existing queue.";
    }
}
