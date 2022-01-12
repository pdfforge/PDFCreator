using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class MessageViewModel : OverlayViewModelBase<MessageInteraction, MessageViewTranslation>
    {
        private readonly ISoundPlayer _soundPlayer;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly IClipboardService _clipboardService;

        public MessageViewModel(ITranslationUpdater translationUpdater, ISoundPlayer soundPlayer,
            ErrorCodeInterpreter errorCodeInterpreter, IClipboardService clipboardService) : base(translationUpdater)
        {
            _soundPlayer = soundPlayer;
            _errorCodeInterpreter = errorCodeInterpreter;
            _clipboardService = clipboardService;

            LeftButtonCommand = new DelegateCommand(ButtonLeftExecute);
            MiddleButtonCommand = new DelegateCommand(MiddleButtonExecute, MiddleButtonCanExecute);
            RightButtonCommand = new DelegateCommand(RightButtonExecute, RightButtonCanExecute);
        }

        private Func<MessageViewTranslation, string> GetIdentifier { get; set; } = translation => translation.Profile;

        public void Init(Func<MessageViewTranslation, string> getIdentifier)
        {
            GetIdentifier = getIdentifier;
        }

        public IList<ErrorWithRegion> ErrorList { get; private set; }
        public Visibility ErrorListVisibility { get; private set; }
        public Visibility SecondTextVisibility { get; private set; }

        public DelegateCommand LeftButtonCommand { get; }
        public DelegateCommand MiddleButtonCommand { get; }
        public DelegateCommand RightButtonCommand { get; }

        public int IconSize { get; set; } = 32;

        public string LeftButtonContent { get; set; }
        public string MiddleButtonContent { get; set; }
        public string RightButtonContent { get; set; }

        private void RightButtonExecute(object obj)
        {
            switch (Interaction.Buttons)
            {
                case MessageOptions.OKCancel:
                case MessageOptions.RetryCancel:
                case MessageOptions.MoreInfoCancel:
                case MessageOptions.YesNoCancel:
                case MessageOptions.YesCancel:
                    Interaction.Response = MessageResponse.Cancel;
                    break;

                case MessageOptions.YesNo:
                    Interaction.Response = MessageResponse.No;
                    break;
            }
            FinishInteraction();
        }

        private void ButtonLeftExecute(object obj)
        {
            switch (Interaction.Buttons)
            {
                case MessageOptions.OK:
                case MessageOptions.OKCancel:
                    Interaction.Response = MessageResponse.OK;
                    break;

                case MessageOptions.MoreInfoCancel:
                    Interaction.Response = MessageResponse.MoreInfo;
                    break;

                case MessageOptions.RetryCancel:
                    Interaction.Response = MessageResponse.Retry;
                    break;

                case MessageOptions.YesNo:
                case MessageOptions.YesNoCancel:
                case MessageOptions.YesCancel:
                    Interaction.Response = MessageResponse.Yes;
                    break;
            }
            FinishInteraction();
        }

        private void MiddleButtonExecute(object obj)
        {
            switch (Interaction.Buttons)
            {
                case MessageOptions.YesNoCancel:
                    Interaction.Response = MessageResponse.No;
                    break;
            }
            FinishInteraction();
        }

        private bool MiddleButtonCanExecute(object obj)
        {
            return Interaction?.Buttons == MessageOptions.YesNoCancel;
        }

        private bool RightButtonCanExecute(object obj)
        {
            return Interaction?.Buttons != MessageOptions.OK;
        }

        protected override void HandleInteractionObjectChanged()
        {
            SetButtonContent(Interaction.Buttons);

            RightButtonCommand.RaiseCanExecuteChanged();
            MiddleButtonCommand.RaiseCanExecuteChanged();

            SetIcon();

            ApplyActionResultOverview();

            ApplySecondText();
        }

        public override string Title => Interaction.Title.ToUpperInvariant();

        private void SetIcon()
        {
            switch (Interaction.Icon)
            {
                case MessageIcon.Error:
                    _soundPlayer.Play(SystemSounds.Hand);
                    break;

                case MessageIcon.Exclamation:
                    _soundPlayer.Play(SystemSounds.Exclamation);
                    break;

                case MessageIcon.Info:
                    _soundPlayer.Play(SystemSounds.Asterisk);
                    break;

                case MessageIcon.Question:
                    _soundPlayer.Play(SystemSounds.Question);
                    break;

                case MessageIcon.Warning:
                    _soundPlayer.Play(SystemSounds.Exclamation);
                    break;

                case MessageIcon.PDFCreator:
                    IconSize = 45;
                    break;

                case MessageIcon.PDFForge:
                    IconSize = 45;
                    break;
            }

            RaisePropertyChanged(nameof(IconSize));
        }

        private void SetButtonContent(MessageOptions option)
        {
            switch (option)
            {
                case MessageOptions.MoreInfoCancel:
                    LeftButtonContent = Translation.MoreInfo;
                    RightButtonContent = Translation.Cancel;
                    break;

                case MessageOptions.OK:
                    LeftButtonContent = Translation.Ok;
                    break;

                case MessageOptions.OKCancel:
                    LeftButtonContent = Translation.Ok;
                    RightButtonContent = Translation.Cancel;
                    break;

                case MessageOptions.RetryCancel:
                    LeftButtonContent = Translation.Retry;
                    RightButtonContent = Translation.Cancel;
                    break;

                case MessageOptions.YesNo:
                    LeftButtonContent = Translation.Yes;
                    RightButtonContent = Translation.No;
                    break;

                case MessageOptions.YesNoCancel:
                    LeftButtonContent = Translation.Yes;
                    RightButtonContent = Translation.Cancel;
                    MiddleButtonContent = Translation.No;
                    break;

                case MessageOptions.YesCancel:
                    LeftButtonContent = Translation.Yes;
                    RightButtonContent = Translation.Cancel;
                    break;
            }

            RaisePropertyChanged(nameof(RightButtonContent));
            RaisePropertyChanged(nameof(MiddleButtonContent));
            RaisePropertyChanged(nameof(LeftButtonContent));
        }

        private void ApplyActionResultOverview()
        {
            if (Interaction.ActionResultDict == null || Interaction.ActionResultDict)
            {
                ErrorListVisibility = Visibility.Collapsed;
                RaisePropertyChanged(nameof(ErrorListVisibility));
                return;
            }

            ErrorListVisibility = Visibility.Visible;
            RaisePropertyChanged(nameof(ErrorListVisibility));

            ErrorList = new List<ErrorWithRegion>();

            foreach (var profileNameActionResult in Interaction.ActionResultDict)
            {
                foreach (var actionResult in profileNameActionResult.Value)
                {
                    var region = GetIdentifier(Translation) + ": " + profileNameActionResult.Key;
                    var error = _errorCodeInterpreter.GetErrorText(actionResult, false);
                    ErrorList.Add(new ErrorWithRegion(region, error));
                }
            }

            RaisePropertyChanged(nameof(ErrorList));

            var view = (CollectionView)CollectionViewSource.GetDefaultView(ErrorList);
            var groupDescription = new PropertyGroupDescription(nameof(ErrorWithRegion.Region));
            view.GroupDescriptions.Add(groupDescription);
        }

        private void ApplySecondText()
        {
            if (string.IsNullOrEmpty(Interaction.SecondText))
            {
                SecondTextVisibility = Visibility.Collapsed;
                RaisePropertyChanged(nameof(SecondTextVisibility));
                return;
            }

            SecondTextVisibility = Visibility.Visible;
            RaisePropertyChanged(nameof(SecondTextVisibility));
        }

        public void CopyToClipboard_CommandBinding(object sender, ExecutedRoutedEventArgs e)
        {
            var text = new StringBuilder();

            text.AppendLine(Interaction.Text);

            if (ErrorList != null)
            {
                var previousProfile = "";

                foreach (var profileError in ErrorList)
                {
                    if (previousProfile != profileError.Region)
                    {
                        text.AppendLine(profileError.Region);
                        previousProfile = profileError.Region;
                    }

                    text.AppendLine("- " + profileError.Error);
                }
            }

            if (!string.IsNullOrEmpty(Interaction.SecondText))
                text.AppendLine(Interaction.SecondText);

            _clipboardService.SetDataObject(text.ToString());
        }
    }

    public class ErrorWithRegion
    {
        public ErrorWithRegion(string region, string error)
        {
            Region = region;
            Error = error;
        }

        public string Error { get; set; }
        public string Region { get; set; }
    }
}
