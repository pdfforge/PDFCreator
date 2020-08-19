using Optional;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public class MountableTokenViewModel : TokenViewModel<ConversionProfile>
    {
        private readonly ISelectedProfileProvider _profileProvider;

        public MountableTokenViewModel(ISelectedProfileProvider profileProvider, Expression<Func<ConversionProfile, string>> selector, ConversionProfile initialValue, IList<string> tokens, Func<string, string> generatePreview, IList<Func<string, Option<string>>> buttonCommandFunction = null) : base(selector, initialValue, tokens, generatePreview, buttonCommandFunction)
        {
            _profileProvider = profileProvider;
        }

        public override void MountView()
        {
            base.MountView();

            _profileProvider.SettingsChanged += ProfileProviderOnSettingsChanged;
            _profileProvider.SelectedProfileChanged += OnProfileProviderOnSelectedProfileChanged;
            CurrentValue = _profileProvider.SelectedProfile;
        }

        private void OnProfileProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs args)
        {
            CurrentValue = _profileProvider.SelectedProfile;
        }

        private void ProfileProviderOnSettingsChanged(object sender, EventArgs e)
        {
            CurrentValue = _profileProvider.SelectedProfile;
        }

        public override void UnmountView()
        {
            base.UnmountView();

            _profileProvider.SettingsChanged -= ProfileProviderOnSettingsChanged;
            _profileProvider.SelectedProfileChanged -= OnProfileProviderOnSelectedProfileChanged;
        }
    }
}
