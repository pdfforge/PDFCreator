using Optional;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailBase
{
    public abstract class MailBaseControlViewModel<T> : ProfileUserControlViewModel<T> where T : IMailBaseTabTranslation, ITranslatable, new()
    {
        public TokenViewModel<ConversionProfile> AdditionalAttachmentsTokenViewModel { get; protected set; }
        public DelegateCommand RemoveSelectedFromListCommand { get; set; }

        protected string AdditionalAttachmentsForTextBox { get; private set; }
        protected abstract IMailActionSettings MailActionSettings { get; }

        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        protected MailBaseControlViewModel(
            ITranslationUpdater translationUpdater,
            ISelectedProfileProvider selectedProfileProvider,
            IDispatcher dispatcher,
            IOpenFileInteractionHelper openFileInteractionHelper
        )
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;

            RemoveSelectedFromListCommand = new DelegateCommand(RemoveSelectedFromList);
        }

        protected List<string> AdditionalAttachments => MailActionSettings?.AdditionalAttachments ?? new List<string>();

        public Dictionary<string, string> AdditionalAttachmentsDictionary
        {
            get
            {
                var fileDict = new Dictionary<string, string>();
                AdditionalAttachments.ForEach(e =>
                {
                    if (!fileDict.ContainsKey(e))
                    {
                        fileDict.Add(
                            e,
                            e.Substring(e.LastIndexOf('\\') + 1));
                    }
                });

                return fileDict;
            }
        }

        protected void RemoveSelectedFromList(object obj)
        {
            MailActionSettings.AdditionalAttachments.Remove((string)obj);

            RaisePropertyChanged(nameof(AdditionalAttachmentsForTextBox)); //raus?
            RaisePropertyChanged(nameof(AdditionalAttachmentsDictionary));
            RaisePropertyChanged(nameof(AdditionalAttachments));

            AdditionalAttachmentsTokenViewModel.RaiseTextChanged();
        }

        protected Option<string> AddFilePathToAdditionalAttachments(string filePathWithToken)
        {
            if (string.IsNullOrEmpty(filePathWithToken))
                return Option.Some("");

            if (!AdditionalAttachmentsDictionary.ContainsKey(filePathWithToken))
                AdditionalAttachments.Add(filePathWithToken);

            AdditionalAttachmentsForTextBox = "";

            RaisePropertyChanged(nameof(AdditionalAttachmentsForTextBox));
            RaisePropertyChanged(nameof(AdditionalAttachmentsDictionary));
            RaisePropertyChanged(nameof(AdditionalAttachments));

            AdditionalAttachmentsTokenViewModel.RaiseTextChanged();

            return Option.Some(AdditionalAttachmentsForTextBox);
        }

        protected Option<string> SelectAttachmentFileAction(string s1)
        {
            var title = Translation.SelectAttachmentTitle;
            var filter = Translation.AllFiles + " " + @"(*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction("", title, filter);
            interactionResult.MatchSome(s =>
            {
                if (!AdditionalAttachmentsDictionary.ContainsKey(s))
                {
                    AdditionalAttachments.Add(s);
                }

                RaisePropertyChanged(nameof(AdditionalAttachmentsForTextBox));
                RaisePropertyChanged(nameof(AdditionalAttachmentsDictionary));
                RaisePropertyChanged(nameof(AdditionalAttachments));

                AdditionalAttachmentsTokenViewModel.RaiseTextChanged();
            });

            return Option.Some(AdditionalAttachmentsForTextBox ?? "");
        }
    }
}
