using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.ProfileHasNotSupportedFeaturesExtension;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Controls.Tab;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public class ProfileSubTabViewModel : BindableBase, ISubTabViewModel
    {
        private readonly ITranslationUpdater _translationUpdater;
        protected readonly ISelectedProfileProvider Profile;
        private readonly ICommandLocator _commandLocator;
        private readonly IEnumerable<IActionFacade> _tokens;
        private ITranslatable _translation;
        private PrismNavigationValueObject _navigationObject;
        private ICommand _addActionCommand;
        private ICommand _removeActionCommand;
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

        public ProfileSubTabViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider profile, ICommandLocator commandLocator, IEnumerable<IActionFacade> tokens)
        {
            _translationUpdater = translationUpdater;
            Profile = profile;
            _commandLocator = commandLocator;
            _tokens = tokens;
        }

        private void OnSelectedProfileChanged(object sender, PropertyChangedEventArgs args)
        {
            Profile.SelectedProfile.UnMountRaiseConditionsForNotSupportedFeatureSections(OnProfileChanged);
            Profile.SelectedProfile.MountRaiseConditionsForNotSupportedFeatureSections(OnProfileChanged);
            OnProfileChanged(sender, args);
        }

        private void OnProfileChanged(object sender, PropertyChangedEventArgs args)
        {
            RaisePropertyChanged(nameof(IsChecked));
            RaisePropertyChanged(nameof(HasNotSupportedFeatures));
        }

        public void Init<TTranslation>(Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, PrismNavigationValueObject navigationObject, Func<ConversionProfile, bool> hasNotSupportedFeatures = null) where TTranslation : ITranslatable, new()
        {
            _setting = setting;

            _addActionCommand = _commandLocator?.GetCommand<AddActionCommand>();
            _removeActionCommand = _commandLocator?.GetCommand<RemoveActionCommand>();

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
            var token = _tokens.OfType<IPresenterActionFacade>()
                            .FirstOrDefault(x => x.SettingsType.Name == setting.GetType().Name);
            if (token == null)
                return;

            _navigationObject.Activate.Invoke();
            RaisePropertyChanged(nameof(HasNotSupportedFeatures));

            if (setting.Enabled == value)
                return;

            if (value)
                _addActionCommand.Execute(token);
            else
                _removeActionCommand.Execute(token);
        }

        public bool GetIsChecked()
        {
            var profileSetting = _setting(Profile.SelectedProfile);
            return profileSetting.Enabled;
        }

        public bool IsChecked
        {
            get => GetIsChecked();
            set => SetIsChecked(value);
        }

        public bool HasNotSupportedFeatures => _hasNotSupportedFeatures(Profile.SelectedProfile);

        protected void OnTranslationChanged()
        {
            RaisePropertyChanged(Title);
        }

        public string Title { get; set; }

        public void MountView()
        {
            Profile.SelectedProfileChanged += OnSelectedProfileChanged;
            SetIsChecked(_setting(Profile.SelectedProfile).Enabled);
            OnSelectedProfileChanged(this, null);
        }

        public void UnmountView()
        {
            Profile.SelectedProfile.UnMountRaiseConditionsForNotSupportedFeatureSections(OnProfileChanged);
            Profile.SelectedProfileChanged -= OnSelectedProfileChanged;
        }
    }
}
