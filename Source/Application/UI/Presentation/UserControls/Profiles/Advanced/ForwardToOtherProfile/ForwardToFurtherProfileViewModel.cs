using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.ObjectModel;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.ForwardToOtherProfile
{
    public class ForwardToFurtherProfileViewModel : ProfileUserControlViewModel<ForwardToFurtherProfileTranslation>, IMountable
    {
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        public bool IsBusinessEdition { get; private set; }

        public ForwardToFurtherProfileViewModel(ITranslationUpdater translationUpdater,
                                              ISelectedProfileProvider selectedProfileProvider,
                                              IDispatcher dispatcher,
                                              ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
                                              EditionHelper editionHelper)
            : base(translationUpdater, selectedProfileProvider, dispatcher)
        {
            IsBusinessEdition = !editionHelper.IsFreeEdition;
            _profilesProvider = profilesProvider;
        }

        public ObservableCollection<ConversionProfileWrapper> ProfilesWrapper { get; private set; }

        private ConversionProfileWrapper _forwardProfile;

        public ConversionProfileWrapper ForwardProfileWrapper
        {
            get { return _forwardProfile; }
            set
            {
                if (value != null)
                {
                    CurrentProfile.ForwardToFurtherProfile.ProfileGuid = value.ConversionProfile.Guid;
                    _forwardProfile = value;
                }
            }
        }

        private void InitCombobox()
        {
            ProfilesWrapper = _profilesProvider?.Settings.Where(p => p.Guid != CurrentProfile.Guid).Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();
            _forwardProfile = ProfilesWrapper.FirstOrDefault(x => x.ConversionProfile.Guid == CurrentProfile.ForwardToFurtherProfile.ProfileGuid);

            // Important: SelectedProfile must be raised before Profiles.
            // Otherwise, the UI will update the binding source and overwrite the selected profile.
            RaisePropertyChanged(nameof(ForwardProfileWrapper));
            RaisePropertyChanged(nameof(ProfilesWrapper));
        }

        public new void MountView()
        {
            base.MountView();
            InitCombobox();
        }
    }
}
