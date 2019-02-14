using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class AboutViewModel : TranslatableViewModelBase<AboutViewTranslation>
    {
        public ApplicationNameProvider ApplicationNameProvider { get; }

        public AboutViewModel(IVersionHelper versionHelper
            , ITranslationUpdater translationUpdater
            , ICommandLocator commandLocator, ApplicationNameProvider applicationNameProvider, EditionHelper editionHelper) : base(translationUpdater)
        {
            ApplicationNameProvider = applicationNameProvider;
            VersionText = versionHelper.FormatWithBuildNumber();

            ShowManualCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.General);
            ShowLicenseCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.License);

            PdfforgeWebsiteCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PdfforgeWebsiteUrl);
            IsLicensedVersion = !editionHelper?.ShowOnlyForPlusAndBusiness ?? true;
            PrioritySupportCommand = commandLocator.GetCommand<PrioritySupportUrlOpenCommand>();
        }

        public Boolean IsLicensedVersion { get; }

        public bool HideSocialMediaButtons { get; }
        public ICommand PrioritySupportCommand { get; }
        public string VersionText { get; }

        public ICommand ShowManualCommand { get; }
        public ICommand ShowLicenseCommand { get; }
        public ICommand PdfforgeWebsiteCommand { get; }
    }
}
