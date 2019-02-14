using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Collections.ObjectModel;

namespace Presentation.UnitTest.Commands.ProfileCommands
{
    [TestFixture]
    public class ProfileAddCommandTest
    {
        private ProfileAddCommand _profileAddCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private ProfileMangementTranslation _translation;
        private PdfCreatorSettings _settings;
        private ConversionProfile _presentProfile;

        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _presentProfile = new ConversionProfile();
            _presentProfile.AuthorTemplate = "Example settings to check copied profile later";
            _currentSettingsProvider.SelectedProfile = _presentProfile;
            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_presentProfile);
            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();
            _profilesProvider.Settings.Returns(_settings.ConversionProfiles);

            _translation = new ProfileMangementTranslation();

            _profileAddCommand = new ProfileAddCommand(_interactionRequest, _profilesProvider, _currentSettingsProvider, new DesignTimeTranslationUpdater());
        }

        [Test]
        public void CanExecute_ReturnsTrue()
        {
            Assert.IsTrue(_profileAddCommand.CanExecute(null));
        }

        [Test]
        public void Execute_RaisesInputInteractionWithCorrectTitleQuestionTextAndInputText()
        {
            _profileAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<InputInteraction>();

            Assert.AreEqual(_translation.AddNewProfile, interaction.Title, "Title");
            Assert.AreEqual(_translation.EnterProfileName, interaction.QuestionText, "QuestionText");
            Assert.AreEqual(_translation.NewProfile, interaction.InputText, "InputText");
            Assert.IsNotNull(interaction.IsValidInput, "IsValidInput is not set");
        }

        [Test]
        public void Execute_UserCancelsInteraction_NoProfileGetsAdded()
        {
            _interactionRequest.RegisterInteractionHandler<InputInteraction>(i => i.Success = false);

            _profileAddCommand.Execute(null);

            Assert.AreEqual(1, _settings.ConversionProfiles.Count);
        }

        [Test]
        public void Execute_UserAppliesInteraction_NewProfileGetsAddedIsCurrentProfileAndHasCorretProperties()
        {
            _presentProfile.Properties.IsShared = true;
            _interactionRequest.RegisterInteractionHandler<InputInteraction>(i =>
            {
                i.Success = true;
                i.InputText = "NewProfileName";
            });

            _profileAddCommand.Execute(null);

            Assert.AreEqual(2, _settings.ConversionProfiles.Count, "No profiles was added");
            Assert.AreEqual("NewProfileName", _currentSettingsProvider.SelectedProfile.Name, "Current Profile is not new profile");
            var newProfile = _currentSettingsProvider.SelectedProfile;
            Assert.Contains(newProfile, _settings.ConversionProfiles, "New Profile was not added to Profiles");
            Assert.IsFalse(string.IsNullOrWhiteSpace(newProfile.Guid), "NewProfile GUID was not set");
            Assert.AreNotEqual(_presentProfile.Guid, newProfile.Guid, "NewProfile GUID is same as present profile");
            Assert.AreEqual(_presentProfile.AuthorTemplate, newProfile.AuthorTemplate, "NewProfile is no copy of present profile");
            Assert.IsFalse(newProfile.Properties.IsShared, "NewProfile must not have shared flag");
        }
    }
}
