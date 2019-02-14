using NSubstitute;
using NSubstitute.ReturnsExtensions;
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
    public class ProfileRenameCommandTest
    {
        private ProfileRenameCommand _profileRenameCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private ProfileMangementTranslation _translation;
        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private string _currentName;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentName = "current profile name";
            var currentProfile = new ConversionProfile();
            currentProfile.Name = _currentName;
            _currentSettingsProvider.SelectedProfile = currentProfile;

            _translation = new ProfileMangementTranslation();

            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();

            _profileRenameCommand = new ProfileRenameCommand(_interactionRequest, _profilesProvider, _currentSettingsProvider, new DesignTimeTranslationUpdater(), new InvokeImmediatelyDispatcher());
        }

        [Test]
        public void CanExecute_CurrentProfileIsNull_ReturnsFalse()
        {
            _currentSettingsProvider.SelectedProfile.ReturnsNull();
            Assert.IsFalse(_profileRenameCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_CurrentProfileIsNotRenamable_ReturnsFalse()
        {
            _currentSettingsProvider.SelectedProfile.Properties.Renamable = false;
            Assert.IsFalse(_profileRenameCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_CurrentProfileIsRenamable_ReturnsTrue()
        {
            _currentSettingsProvider.SelectedProfile.Properties.Renamable = true;
            Assert.IsTrue(_profileRenameCommand.CanExecute(null));
        }

        [Test]
        public void Execute_RaisesInputInteractionWithCurrentNameAsInput()
        {
            _profileRenameCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<InputInteraction>();
            Assert.AreEqual(_translation.RenameProfile, interaction.Title, "Title");
            Assert.AreEqual(_translation.EnterNewProfileName, interaction.QuestionText, "QuestionText");
            Assert.AreEqual(_currentName, interaction.InputText, "InputText");
            Assert.IsNotNull(interaction.IsValidInput, "IsValidInput is not set");
        }

        [Test]
        public void Execute_UserCancelsInteraction_CurrentNameRemains()
        {
            _interactionRequest.RegisterInteractionHandler<InputInteraction>(i =>
            {
                i.Success = false; //User cancels
                i.InputText = "Unwanted new name";
            });

            _profileRenameCommand.Execute(null);

            Assert.AreEqual(_currentName, _currentSettingsProvider.SelectedProfile.Name);
        }

        [Test]
        public void Execute_UserAppliesInteraction_CurrentProfileHasNewName()
        {
            _interactionRequest.RegisterInteractionHandler<InputInteraction>(i =>
            {
                i.Success = true; //User cancels
                i.InputText = "NewProfileName";
            });

            _profileRenameCommand.Execute(null);

            Assert.AreEqual("NewProfileName", _currentSettingsProvider.SelectedProfile.Name);
        }
    }
}
