using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings
{
    public class DefaultViewerViewModel : TranslatableViewModelBase<DefaultViewerTranslation>, ITabViewModel
    {
        private readonly ICurrentSettings<ObservableCollection<DefaultViewer>> _defaultViewerProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IOpenFileInteractionHelper _fileInteractionHelper;
        public IGpoSettings GpoSettings { get; }

        private ObservableCollection<DefaultViewer> _defaultViewers => _settingsProvider?.Settings.DefaultViewerList;

        public DefaultViewerViewModel(
            ITranslationUpdater translationUpdater,
            ISettingsProvider settingsProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            IGpoSettings gpoSettings, 
            IOpenFileInteractionHelper fileInteractionHelper)
            : base(translationUpdater)
        {
            _settingsProvider = settingsProvider;
            _currentSettingsProvider = currentSettingsProvider;

           
            _fileInteractionHelper = fileInteractionHelper;
            GpoSettings = gpoSettings;

            if (_defaultViewers != null)
                UpdateDefaultViewer();

            FindPathCommand = new DelegateCommand(ExecuteFindPath);

        }

        public ICommand FindPathCommand { get; set; }

        private void UpdateDefaultViewer()
        {
            RaisePropertyChanged(nameof(DefaultViewers));
        }

        public void ExecuteFindPath(object data)
        {
            var model = (DefaultViewer)data;
            var filter = Translation.ExecutableFiles
                         + @" (*.exe, *.bat, *.cmd)|*.exe;*.bat;*.cmd|"
                         + Translation.AllFiles
                         + @"(*.*)|*.*";

            var interactionResult = _fileInteractionHelper.StartOpenFileInteraction("", "", filter);
            interactionResult.MatchSome(s =>
            {
                model.Path = s;
                RaisePropertyChanged(nameof(DefaultViewers));
            });
        }

        public ObservableCollection<DefaultViewer> DefaultViewers
        {
            get { return _defaultViewers; }
        }

        public string Title { get; set; } = "Viewer";
        public IconList Icon { get; set; } = IconList.DefaultViewerSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => GpoSettings.DisableApplicationSettings;
        public bool HasNotSupportedFeatures => false;

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            Title = Translation.Title;
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged();
        }

        public void MountView()
        {
            _currentSettingsProvider.SettingsChanged += OnSettingsChanged;
            UpdateDefaultViewer();
        }

        private void OnSettingsChanged(object sender, EventArgs args)
        {
            UpdateDefaultViewer();
        }

        public void UnmountView()
        {
            _currentSettingsProvider.SettingsChanged -= OnSettingsChanged;
        }
    }

    public class DesignTimeDefaultViewerViewModel : DefaultViewerViewModel
    {
        private static readonly ICurrentSettingsProvider CurrentSettingsProvider = new DesignTimeCurrentSettingsProvider();

        public DesignTimeDefaultViewerViewModel() : base(new DesignTimeTranslationUpdater(), null, CurrentSettingsProvider, null, null)
        {
        }
    }
}