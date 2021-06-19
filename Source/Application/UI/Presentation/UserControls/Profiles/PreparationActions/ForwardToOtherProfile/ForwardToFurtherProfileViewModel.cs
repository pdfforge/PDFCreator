using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.ObjectModel;
using System.Linq;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.ForwardToOtherProfile
{
    public interface IForwardToFurtherProfileViewModel : IActionViewModel
    {
        bool IsBusinessEdition { get; }
        ObservableCollection<ConversionProfileWrapper> ProfilesWrapper { get; }

        ConversionProfileWrapper ForwardProfileWrapper { get; }
    }

    public class ForwardToFurtherProfileViewModel : ForwardToFurtherProfileViewModelBase<ForwardToFurtherProfileTranslation>
    {
        public ForwardToFurtherProfileViewModel(ITranslationUpdater translationUpdater,
            IDispatcher dispatcher,
            EditionHelper editionHelper, IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(translationUpdater, dispatcher, editionHelper, actionLocator, errorCodeInterpreter, currentSettingsProvider, defaultSettingsBuilder, actionOrderHelper)
        {
        }
    }

    public abstract class ForwardToFurtherProfileViewModelBase<TTranslation> : ActionViewModelBase<ForwardToFurtherProfileActionBase, TTranslation>, IForwardToFurtherProfileViewModel where TTranslation : IActionTranslation, new()
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        public bool IsBusinessEdition { get; }

        public ForwardToFurtherProfileViewModelBase(ITranslationUpdater translationUpdater,
                                              IDispatcher dispatcher,
                                              EditionHelper editionHelper,
                                              IActionLocator actionLocator,
                                              ErrorCodeInterpreter errorCodeInterpreter,
                                              ICurrentSettingsProvider currentSettingsProvider,
                                              IDefaultSettingsBuilder defaultSettingsBuilder,
                                              IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _currentSettingsProvider = currentSettingsProvider;
            IsBusinessEdition = !editionHelper.IsFreeEdition;
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
            ProfilesWrapper = _currentSettingsProvider.CheckSettings.Profiles.Where(p => p.Guid != CurrentProfile.Guid).Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();
            _forwardProfile = ProfilesWrapper.FirstOrDefault(x => x.ConversionProfile.Guid == CurrentProfile.ForwardToFurtherProfile.ProfileGuid);

            // Important: SelectedProfile must be raised before Profiles.
            // Otherwise, the UI will update the binding source and overwrite the selected profile.
            RaisePropertyChanged(nameof(ForwardProfileWrapper));
            RaisePropertyChanged(nameof(ProfilesWrapper));
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var forwardProfileGuid = CurrentProfile.ForwardToFurtherProfile.ProfileGuid;
                var profile = _currentSettingsProvider.CheckSettings.Profiles.FirstOrDefault(p => p.Guid == forwardProfileGuid);
                return profile != null ? profile.Name : string.Empty;
            }
        }

        public new void MountView()
        {
            base.MountView();
            InitCombobox();
        }
    }
}
