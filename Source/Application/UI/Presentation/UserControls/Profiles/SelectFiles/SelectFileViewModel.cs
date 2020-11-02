using Optional;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    public class SelectFileViewModel : OverlayViewModelBase<SelectFileInteraction, SelectFileTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        public TokenViewModel<SelectFileInteraction> AdditionalAttachmentsTokenViewModel { get; protected set; }

        public DelegateCommand AddEditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand CancelCommand { get; }

        public SelectFileViewModel(ITranslationUpdater translationUpdater,
            ITokenViewModelFactory tokenViewModelFactory,
            IOpenFileInteractionHelper openFileInteractionHelper)
            : base(translationUpdater)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _tokenViewModelFactory = tokenViewModelFactory;
            var builder = tokenViewModelFactory
                .Builder<SelectFileInteraction>()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForFilename()); // Tokens are overriden in HandleInteractionObjectChanged if Interaction.Tokens is set

            AdditionalAttachmentsTokenViewModel = builder
                .WithSelector(interaction => interaction.File)
                .WithButtonCommand(SelectFileAction)
                .Build();

            AddEditCommand = new DelegateCommand(ApplyExecute, o => !string.IsNullOrWhiteSpace(AdditionalAttachmentsTokenViewModel.Text));
            AdditionalAttachmentsTokenViewModel.TextChanged += (sender, args) => AddEditCommand.RaiseCanExecuteChanged();

            RemoveCommand = new DelegateCommand(RemoveExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
        }

        private void ApplyExecute(object obj)
        {
            Interaction.Result = SelectFileInteractionResult.Apply;
            FinishInteraction();
        }

        private void RemoveExecute(object o)
        {
            Interaction.Result = SelectFileInteractionResult.Remove;
            FinishInteraction();
        }

        private void CancelExecute(object o)
        {
            Interaction.Result = SelectFileInteractionResult.Cancel;
            FinishInteraction();
        }

        private Option<string> SelectFileAction(string currentAttachment)
        {
            var title = Translation.SelectFile;
            var filter = Interaction.Filter ?? Translation.AllFiles + " " + @"(*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(currentAttachment, title, filter);
            interactionResult.MatchSome(s => { AdditionalAttachmentsTokenViewModel.RaiseTextChanged(); });

            return interactionResult;
        }

        public Visibility RemoveButtonVisibility { get; private set; }

        public override string Title => Interaction.Title;

        protected override void HandleInteractionObjectChanged()
        {
            RemoveButtonVisibility = Interaction.ShowRemoveButton ? Visibility.Visible : Visibility.Collapsed;
            RaisePropertyChanged(nameof(RemoveButtonVisibility));

            RaisePropertyChanged(nameof(Title));
            AdditionalAttachmentsTokenViewModel.CurrentValue = Interaction;
            if (!(Interaction.Tokens is null)) // If null, tokens are left as the default set in the constructor
            {
                AdditionalAttachmentsTokenViewModel.SetTokens(Interaction.Tokens);
            }
        }
    }
}
