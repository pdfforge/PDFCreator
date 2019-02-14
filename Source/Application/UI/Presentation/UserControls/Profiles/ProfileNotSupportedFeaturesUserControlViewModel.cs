using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public abstract class ProfileNotSupportedFeaturesUserControlViewModel<TTranslation> : ProfileUserControlViewModel<TTranslation>, IHasNotSupportedFeatures where TTranslation : ITranslatable, new()
    {
        public abstract bool HasNotSupportedFeatures { get; }

        protected ProfileNotSupportedFeaturesUserControlViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfileProvider, IDispatcher dispatcher)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            CurrentProfileChanged += OnCurrentProfileChanged;
            CurrentProfile.SetRaiseConditionsForNotSupportedFeatureSections(OnProfileChanged);
        }

        private void OnCurrentProfileChanged(object sender, EventArgs args)
        {
            OnProfileChanged(sender, args);
            CurrentProfile.SetRaiseConditionsForNotSupportedFeatureSections(OnProfileChanged);
        }

        private void OnProfileChanged(object sender, EventArgs args)
        {
            RaisePropertyChanged(nameof(HasNotSupportedFeatures));
        }
    }
}
