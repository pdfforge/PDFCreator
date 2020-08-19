using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Regions;
using System;
using System.ComponentModel;
using pdfforge.PDFCreator.Core.Services;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class ProfileUserControlViewModel<TTranslation> : TranslatableViewModelBase<TTranslation>, IMountable, IRegionMemberLifetime where TTranslation : ITranslatable, new()
    {
        protected readonly IDispatcher _dispatcher;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        public ConversionProfile CurrentProfile => _selectedProfileProvider.SelectedProfile;

        public event EventHandler CurrentProfileChanged;

        public ProfileUserControlViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfileProvider, IDispatcher dispatcher) : base(translationUpdater)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _dispatcher = dispatcher;
        }

        private void OnCurrentSettingsChanged(object sender, EventArgs e)
        {
            OnCurrentProfileChanged(this, null);
        }

        protected virtual void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _dispatcher?.BeginInvoke(() =>
            {
                CurrentProfileChanged?.Invoke(this, EventArgs.Empty);
                RaisePropertyChanged(nameof(CurrentProfile));
            });
        }

        public virtual void MountView()
        {
            if (_selectedProfileProvider == null)
                return;
            _selectedProfileProvider.SelectedProfileChanged += OnCurrentProfileChanged;
            _selectedProfileProvider.SettingsChanged += OnCurrentSettingsChanged;
            OnCurrentProfileChanged(this, null);
        }

        public virtual void UnmountView()
        {
            if (_selectedProfileProvider == null)
                return;
            _selectedProfileProvider.SelectedProfileChanged -= OnCurrentProfileChanged;
            _selectedProfileProvider.SettingsChanged -= OnCurrentSettingsChanged;
        }

        public bool KeepAlive { get; } = true;
    }
}
