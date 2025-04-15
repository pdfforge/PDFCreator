using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.UI.Presentation.WorkflowQuery;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Utilities.Messages;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class ErrorViewModel : TranslatableViewModelBase<ErrorViewTranslation>, IWorkflowErrorViewModel
    {
        private TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
        private PasswordOverlayTranslation _passwordOverlayTranslation;
        private readonly ITranslationFactory _translationFactory;
        private InteractiveWorkflowTranslation _translation;

        public Error Error { get; set; }
        public DelegateCommand OkCommand { get; }

        public ErrorViewModel(ITranslationUpdater translationUpdater, ITranslationFactory translationFactory)
            : base(translationUpdater)
        {
            _translationFactory = translationFactory;
            OkCommand = new DelegateCommand(OkExecute);
            translationUpdater.RegisterAndSetTranslation(tf => _passwordOverlayTranslation = tf.UpdateOrCreateTranslation(_passwordOverlayTranslation));
            translationUpdater.RegisterAndSetTranslation(factory => _translation = factory.UpdateOrCreateTranslation(_translation));
        }

        private Error FormatError(ActionResult error, bool asWarning)
        {
            var title = asWarning ? _translation.Warning : _translation.Error;
            var icon = asWarning ? MessageIcon.Warning : MessageIcon.Error;

            var errorCodeInterpreter = new ErrorCodeInterpreter(_translationFactory);
            var errorOccurredText = asWarning ? _translation.ErrorsSkipped : _translation.AnErrorOccured;
            var message = errorCodeInterpreter.GetErrorText(error, false);

            return new Error(icon, title, errorOccurredText, message);
        }

        public async Task ExecuteWorkflowStep(Job job, ActionResult error, bool asWarning)
        {
            Error = FormatError(error, asWarning);
            RaisePropertyChanged(nameof(Error));

            _taskCompletionSource = new TaskCompletionSource<bool>();
            _ = await _taskCompletionSource.Task;
        }

        private void OkExecute(object _)
        {
            _taskCompletionSource.SetResult(true);
        }
    }

    public class Error
    {
        public Error(MessageIcon icon, string title, string preface, string text)
        {
            Icon = icon;
            Title = title;
            Preface = preface;
            Text = text;
        }

        public MessageIcon Icon { get; set; }

        public string Title { get; set; }
        public string Preface { get; set; }
        public string Text { get; set; }
    }
}
