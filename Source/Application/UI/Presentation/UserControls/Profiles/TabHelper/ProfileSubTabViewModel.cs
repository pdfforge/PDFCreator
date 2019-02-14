using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.UI.Presentation.Controls.Tab;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public class ProfileSubTabViewModel : BindableBase, ISubTabViewModel
    {
        private readonly ITranslationUpdater _translationUpdater;
        protected readonly ISelectedProfileProvider Profile;
        private ITranslatable _translation;
        private PrismNavigationValueObject _navigationObject;
        private Func<ConversionProfile, IProfileSetting> _setting;
        private Func<ConversionProfile, bool> _hasNotSupportedFeatures;

        public ITranslatable Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                RaisePropertyChanged(nameof(Translation));
                OnTranslationChanged();
            }
        }

        public ProfileSubTabViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider profile)
        {
            _translationUpdater = translationUpdater;
            Profile = profile;
            Profile.SelectedProfileChanged += OnSelectedProfileChanged;
            Profile.SelectedProfile.SetRaiseConditionsForNotSupportedFeatureSections(OnProfileChanged);
        }

        private void OnSelectedProfileChanged(object sender, PropertyChangedEventArgs args)
        {
            OnProfileChanged(sender, args);
            Profile.SelectedProfile.SetRaiseConditionsForNotSupportedFeatureSections(OnProfileChanged);
        }

        private void OnProfileChanged(object sender, PropertyChangedEventArgs args)
        {
            RaisePropertyChanged(nameof(IsChecked));
            RaisePropertyChanged(nameof(HasNotSupportedFeatures));
        }

        public void Init<TTranslation>(Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, PrismNavigationValueObject navigationObject, Func<ConversionProfile, bool> hasNotSupportedFeatures = null) where TTranslation : ITranslatable, new()
        {
            _setting = setting;

            _hasNotSupportedFeatures = hasNotSupportedFeatures ?? (p => false);
            _navigationObject = navigationObject;

            _translationUpdater.RegisterAndSetTranslation(tf =>
            {
                var translation = tf.UpdateOrCreateTranslation((TTranslation)_translation);
                _translation = translation;
                Title = titleId(translation);
            });
        }

        public void SetIsChecked(bool value)
        {
            var setting = _setting(Profile.SelectedProfile);
            setting.Enabled = value;
            _navigationObject.Activate.Invoke();
            RaisePropertyChanged(nameof(HasNotSupportedFeatures));
        }

        public bool GetIsChecked()
        {
            return _setting(Profile.SelectedProfile).Enabled;
        }

        public bool IsChecked
        {
            get { return GetIsChecked(); }
            set
            {
                SetIsChecked(value);
            }
        }

        public bool HasNotSupportedFeatures => _hasNotSupportedFeatures(Profile.SelectedProfile);

        protected void OnTranslationChanged()
        {
            RaisePropertyChanged(Title);
        }

        public string Title { get; set; }
    }
}
