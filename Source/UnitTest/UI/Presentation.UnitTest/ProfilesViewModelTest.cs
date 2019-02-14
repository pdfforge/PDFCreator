using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace Presentation.UnitTest
{
    [TestFixture]
    public class ProfilesViewModelTest
    {
        private ProfilesViewModel _profilesViewModel;
        private ISelectedProfileProvider _selectedProfileProvider;
        private IGpoSettings _gpoSettings;

        private ITranslationUpdater _translationUpdater;
        private ProfileMangementTranslation _translation;
        private ConversionProfile _selectedProfile;

        [SetUp]
        public void SetUp()
        {
            _selectedProfileProvider = Substitute.For<ISelectedProfileProvider>();
            _selectedProfile = new ConversionProfile();
            _selectedProfileProvider.SelectedProfile.Returns(_selectedProfile);
            _translationUpdater = Substitute.For<ITranslationUpdater>();
            _gpoSettings = Substitute.For<IGpoSettings>();

            BuildProfilesViewModel();

            _translation = new ProfileMangementTranslation();
            _profilesViewModel.Translation = _translation;
        }

        private void BuildProfilesViewModel()
        {
            _profilesViewModel = new ProfilesViewModel(_selectedProfileProvider, _translationUpdater, new DesignTimeCommandLocator(), null, null, _gpoSettings);
        }

        [Test]
        public void TranslationUpdater_RegisterAndSetTranslation_GetsCalled()
        {
            _translationUpdater.Received().RegisterAndSetTranslation(_profilesViewModel);
        }

        [Test]
        public void Translation_SetTranslation_RaisePropertyChangedGetsCalled()
        {
            var wasCalled = false;
            _profilesViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_profilesViewModel.Translation)))
                    wasCalled = true;
            };

            _profilesViewModel.Translation = _translation;

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditProfileIsGpoDisabled_GpoSettingsIsNull_SelectedProfileIsNull_ReturnsFalse()
        {
            _gpoSettings = null;
            _selectedProfile = null;
            BuildProfilesViewModel();
            Assert.IsFalse(_profilesViewModel.EditProfileIsGpoDisabled);
        }

        [Test]
        public void EditProfileIsGpoDisabled_GpoDisableProfileManagementIsEnabled_SelectedProfileIsNotShared_ReturnsTrue()
        {
            _gpoSettings.DisableProfileManagement.Returns(true);
            _selectedProfile.Properties.IsShared = false;

            Assert.IsTrue(_profilesViewModel.EditProfileIsGpoDisabled);
        }

        [Test]
        public void EditProfileIsGpoDisabled_GpoDisableProfileManagementIsDisabled_SelectedProfileIsNotShared_ReturnsFalse()
        {
            _gpoSettings.DisableProfileManagement.Returns(false);
            _selectedProfile.Properties.IsShared = false;

            Assert.IsFalse(_profilesViewModel.EditProfileIsGpoDisabled);
        }

        [Test]
        public void EditProfileIsGpoDisabled_GpoDisableProfileManagementIsDisabled_SelectedProfileIsShared_ReturnsTrue()
        {
            _gpoSettings.DisableProfileManagement.Returns(false);
            _selectedProfile.Properties.IsShared = true;

            Assert.IsTrue(_profilesViewModel.EditProfileIsGpoDisabled);
        }

        [Test]
        public void RenameProfileButtonIsGpoEnabled_GpoSettingsIsNull_SelectedProfileIsNull_ReturnsFalse()
        {
            _gpoSettings = null;
            _selectedProfile = null;
            BuildProfilesViewModel();
            Assert.IsTrue(_profilesViewModel.RenameProfileButtonIsGpoEnabled);
        }

        [Test]
        public void RenameProfileButtonIsGpoEnabled_GpoDisableProfileManagementIsEnabled_SelectedProfileIsNotShared_ReturnsTrue()
        {
            _gpoSettings.DisableProfileManagement.Returns(true);
            _selectedProfile.Properties.IsShared = false;

            Assert.IsFalse(_profilesViewModel.RenameProfileButtonIsGpoEnabled);
        }

        [Test]
        public void RenameProfileButtonIsGpoEnabled_GpoDisableProfileManagementIsDisabled_SelectedProfileIsNotShared_ReturnsFalse()
        {
            _gpoSettings.DisableProfileManagement.Returns(false);
            _selectedProfile.Properties.IsShared = false;

            Assert.IsTrue(_profilesViewModel.RenameProfileButtonIsGpoEnabled);
        }

        [Test]
        public void RenameProfileButtonIsGpoEnabled_GpoDisableProfileManagementIsDisabled_SelectedProfileIsShared_ReturnsTrue()
        {
            _gpoSettings.DisableProfileManagement.Returns(false);
            _selectedProfile.Properties.IsShared = true;

            Assert.IsFalse(_profilesViewModel.RenameProfileButtonIsGpoEnabled);
        }

        [Test]
        public void AddProfileButtonIsGpoEnabled_GpoSettingsIsNull_ReturnsTrue()
        {
            _gpoSettings = null;
            BuildProfilesViewModel();
            Assert.IsTrue(_profilesViewModel.AddProfileButtonIsGpoEnabled);
        }

        [Test]
        public void AddProfileButtonIsGpoEnabled_GpoDisableProfileManagementIsEnabled_ReturnsFalse()
        {
            _gpoSettings.DisableProfileManagement.Returns(true);
            BuildProfilesViewModel();
            Assert.IsFalse(_profilesViewModel.AddProfileButtonIsGpoEnabled);
        }

        [Test]
        public void AddProfileButtonIsGpoEnabled_LoadSharedProfilesIsEnabled_AllowUserDefinedProfilesIsDisabled_ReturnsFalse()
        {
            _gpoSettings.LoadSharedProfiles.Returns(true);
            _gpoSettings.AllowUserDefinedProfiles.Returns(false);
            BuildProfilesViewModel();
            Assert.IsFalse(_profilesViewModel.AddProfileButtonIsGpoEnabled);
        }

        [Test]
        public void AddProfileButtonIsGpoEnabled_LoadSharedProfilesIsDisabled_ReturnsTrue()
        {
            _gpoSettings.LoadSharedProfiles.Returns(false);
            _gpoSettings.AllowUserDefinedProfiles.Returns(false);
            BuildProfilesViewModel();
            Assert.IsTrue(_profilesViewModel.AddProfileButtonIsGpoEnabled);
        }

        [Test]
        public void RemoveProfileButtonIsGpoEnabled_GpoSettingsIsNull_SelectedProfileIsNull_ReturnsFalse()
        {
            _gpoSettings = null;
            _selectedProfile = null;
            BuildProfilesViewModel();
            Assert.IsTrue(_profilesViewModel.RemoveProfileButtonIsGpoEnabled);
        }

        [Test]
        public void RemoveProfileButtonIsGpoEnabled_GpoDisableProfileManagementIsEnabled_SelectedProfileIsNotShared_ReturnsTrue()
        {
            _gpoSettings.DisableProfileManagement.Returns(true);
            _selectedProfile.Properties.IsShared = false;

            Assert.IsFalse(_profilesViewModel.RemoveProfileButtonIsGpoEnabled);
        }

        [Test]
        public void RemoveProfileButtonIsGpoEnabled_GpoDisableProfileManagementIsDisabled_SelectedProfileIsNotShared_ReturnsFalse()
        {
            _gpoSettings.DisableProfileManagement.Returns(false);
            _selectedProfile.Properties.IsShared = false;

            Assert.IsTrue(_profilesViewModel.RemoveProfileButtonIsGpoEnabled);
        }

        [Test]
        public void RemoveProfileButtonIsGpoEnabled_GpoDisableProfileManagementIsDisabled_SelectedProfileIsShared_ReturnsTrue()
        {
            _gpoSettings.DisableProfileManagement.Returns(false);
            _selectedProfile.Properties.IsShared = true;

            Assert.IsFalse(_profilesViewModel.RemoveProfileButtonIsGpoEnabled);
        }
    }
}
