using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings
{
    public class TitleReplacementEditUserControlViewModel : OverlayViewModelBase<TitleReplacementEditInteraction, TitleReplacementsTranslation>
    {
        //private string _errorText;
        private bool _isInvalid;
        private bool _isRemoveAll;
        private bool _isRemoveAtBeginning;
        private bool _isRemoveAtEnd;
        private bool _isReplaceByRegex;
        private string _replaceWithInput = "";
        private string _searchForInput = "";


        public TitleReplacementEditUserControlViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator):base(translationUpdater)
        {
            IsRemoveAll = true;
            OkCommand = new DelegateCommand(ExecuteOkCommand);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
            ShowUserGuideCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.AppTitle);
        }

        public ICommand ShowUserGuideCommand { get; set; }


        public bool IsInvalid
        {
            get { return _isInvalid; }
            set
            {
                _isInvalid = value;
                RaisePropertyChanged(nameof(IsInvalid));
            }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public String SearchForInput
        {
            get { return _searchForInput; }
            set
            {
                _searchForInput = value;
                ValidateInput();
                RaisePropertyChanged(nameof(SearchForInput));
            }
        }

        public string ReplaceWithInput
        {
            get { return _replaceWithInput; }
            set
            {
                _replaceWithInput = value;
                ValidateInput();
                RaisePropertyChanged(nameof(ReplaceWithInput));
            }
        }

        public bool IsRemoveAll
        {
            get { return _isRemoveAll; }
            set
            {
                _isRemoveAll = value;
                RaisePropertyChanged(nameof(IsRemoveAll));
            }
        }

        public bool IsRemoveAtBeginning
        {
            get { return _isRemoveAtBeginning; }
            set
            {
                _isRemoveAtBeginning = value;
                RaisePropertyChanged(nameof(IsRemoveAtBeginning));
            }
        }

        public bool IsRemoveAtEnd
        {
            get { return _isRemoveAtEnd; }
            set
            {
                _isRemoveAtEnd = value;
                RaisePropertyChanged(nameof(IsRemoveAtEnd));
            }
        }

        public bool IsReplaceByRegex
        {
            get { return _isReplaceByRegex; }
            set
            {
                _isReplaceByRegex = value;
                if (value)
                {
                    ValidateInput();
                }
                else
                {
                    ClearReplacementField();
                }
                RaisePropertyChanged(nameof(IsReplaceByRegex));
            }
        }

        public override string Title => Translation.EditTextReplacementTitle;
        
        private void ExecuteCancelCommand(object o)
        {
            FinishInteraction();
        }

        private void ExecuteOkCommand(object o)
        {
            Interaction.Success = true;
            Interaction.Replacement.Replace = ReplaceWithInput;
            Interaction.Replacement.Search = SearchForInput;
            Interaction.Replacement.ReplacementType = GetReplacementType();
            FinishInteraction();
        }

        private ReplacementType GetReplacementType()
        {
            var replacementType = ReplacementType.Replace;
            if (IsRemoveAtBeginning)
            {
                replacementType = ReplacementType.Start;
            }
            else if (IsRemoveAtEnd)
            {
                replacementType = ReplacementType.End;
            }
            else if (IsReplaceByRegex)
            {
                replacementType = ReplacementType.RegEx;
            }
            return replacementType;
        }

        private void ValidateInput()
        {
            var isValidState = true;
            if (IsReplaceByRegex)
            {
                try
                {
                    Regex.Replace("E", SearchForInput, ReplaceWithInput);
                }
                catch (Exception)
                {
                    isValidState = false;
                }
            }
            IsInvalid = !isValidState;
        }

        private void ClearReplacementField()
        {
            ReplaceWithInput = "";
        }

        protected override void HandleInteractionObjectChanged()
        {
            IsRemoveAll = false;
            IsRemoveAtBeginning = false;
            IsReplaceByRegex = false;
            IsRemoveAtEnd = false;

            base.HandleInteractionObjectChanged();
            SearchForInput = Interaction.Replacement.Search;
            ReplaceWithInput = Interaction.Replacement.Replace;


            switch (Interaction.Replacement.ReplacementType)
            {
                case ReplacementType.Replace:
                    IsRemoveAll = true;
                    break;
                case ReplacementType.Start:
                    IsRemoveAtBeginning = true;
                    break;
                case ReplacementType.End:
                    IsRemoveAtEnd = true;
                    break;
                case ReplacementType.RegEx:
                    IsReplaceByRegex = true;
                    break;
            }
        }
    }

    public class DesignTimeTitleReplacementEditUserControlViewModel : TitleReplacementEditUserControlViewModel
    {
        public DesignTimeTitleReplacementEditUserControlViewModel() : base(new TranslationUpdater(new TranslationFactory(null), new ThreadManager()), new DesignTimeCommandLocator())
        {
        }
    }
}
