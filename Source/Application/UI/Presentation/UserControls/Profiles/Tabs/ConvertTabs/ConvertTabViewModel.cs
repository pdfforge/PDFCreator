using System;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertTabViewModel : ProfileNotSupportedFeaturesUserControlViewModel<ConvertTabTranslation>, ITabViewModel
    {
        public ConvertTabViewModel(ITranslationUpdater updater, ISelectedProfileProvider selectedProfile, IDispatcher dispatcher) : base(updater, selectedProfile, dispatcher)
        {
            SetOutputFormatCommand = new DelegateCommand<OutputFormat>(SetOutputFormatExecute);
        }

        // Required for Icons Styles
        public OutputFormat OutputFormat => CurrentProfile?.OutputFormat ?? OutputFormat.Pdf;

        public IEnumerable<OutputFormat> OutputFormats => System.Enum.GetValues(typeof(OutputFormat)) as OutputFormat[];

        public string Title => Translation.ConvertTab;
        public IconList Icon => IconList.ConversionSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => false;
        public override bool HasNotSupportedFeatures => CurrentProfile.HasNotSupportedConvert();

        public DelegateCommand<OutputFormat> SetOutputFormatCommand { get; }

        private void SetOutputFormatExecute(OutputFormat parameter)
        {
            CurrentProfile.OutputFormat = parameter;
            RaisePropertyChanged(nameof(OutputFormat));
        }
        private void OnCurrentProfileChanged(object sender, EventArgs args)
        {
            RaisePropertyChanged(nameof(OutputFormat));
        }

        public override void MountView()
        {
            base.MountView();
            CurrentProfileChanged += OnCurrentProfileChanged;
        }


        public override void UnmountView()
        {
            base.UnmountView();
            CurrentProfileChanged -= OnCurrentProfileChanged;
        }
    }
}
