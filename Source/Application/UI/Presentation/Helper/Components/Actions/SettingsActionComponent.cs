using pdfforge.PDFCreator.Conversion.Actions.Components;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Linq.Expressions;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Components.Actions
{
    public class SettingsActionComponent : SettingsComponent
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private Action<ConversionProfile, IProfileSetting> _setter;

        public ConversionProfile CurrentSetting => _currentSettingsProvider.SelectedProfile;
        private Func<ConversionProfile, IProfileSetting> _getSetting;

        public IProfileSetting Setting => _getSetting(_currentSettingsProvider.SelectedProfile);

        public IProfileSetting GetProfileSettingByConversionProfile(ConversionProfile profile)
        {
            return _getSetting(profile);
        }

        public SettingsActionComponent(ICurrentSettingsProvider currentSettingsProvider, Type settingsType) : base(settingsType)
        {
            _currentSettingsProvider = currentSettingsProvider;
        }

        public void Init<TSettings>(Expression<Func<ConversionProfile, TSettings>> getterExpression) where TSettings : IProfileSetting
        {
            var generator = new CustomMethodGenerator();
            _getSetting = generator.ConvertFromGenericToSpecificGetter<ConversionProfile, IProfileSetting, TSettings>(getterExpression);
            _setter = generator.GenerateSetterFromGetter<ConversionProfile, IProfileSetting, TSettings>(getterExpression);
        }

        public void SetSettings(IProfileSetting setting)
        {
            _setter.Invoke(_currentSettingsProvider.SelectedProfile, setting);
        }

        public bool Enabled
        {
            get => Setting != null ? Setting.Enabled : false;
            set
            {
                if (Setting != null)
                    Setting.Enabled = value;
            }
        }
    }
}
