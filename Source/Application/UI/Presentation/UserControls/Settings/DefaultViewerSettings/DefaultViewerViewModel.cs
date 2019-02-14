using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
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
        private readonly IOpenFileInteractionHelper _fileInteractionHelper;
        public IGpoSettings GpoSettings { get; }

        private ObservableCollection<DefaultViewer> _defaultViewers => _defaultViewerProvider?.Settings;

        public DefaultViewerViewModel(
            ITranslationUpdater translationUpdater,
            ICurrentSettings<ObservableCollection<DefaultViewer>> defaultViewerProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            IGpoSettings gpoSettings, 
            IOpenFileInteractionHelper fileInteractionHelper)
            : base(translationUpdater)
        {
            _defaultViewerProvider = defaultViewerProvider;
            _fileInteractionHelper = fileInteractionHelper;
            GpoSettings = gpoSettings;

            if (_defaultViewers != null)
                UpdateDefaultViewer();

            currentSettingsProvider.SettingsChanged += (sender, args) => UpdateDefaultViewer();
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
    }

    public class DesignTimeDefaultViewerViewModel : DefaultViewerViewModel
    {
        private static readonly ICurrentSettingsProvider CurrentSettingsProvider = new DesignTimeCurrentSettingsProvider();

        public DesignTimeDefaultViewerViewModel() : base(new DesignTimeTranslationUpdater(), null, CurrentSettingsProvider, null, null)
        {
        }
    }
}