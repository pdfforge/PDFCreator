using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Collections.ObjectModel;
using System.Text;

namespace Presentation.UnitTest.Commands.ProfileCommands
{
    [TestFixture]
    public class ProfileRemoveCommandTest
    {
        private ProfileRemoveCommand _profileRemoveCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private ProfileMangementTranslation _translation;
        private PdfCreatorSettings _settings;
        private ConversionProfile _currentProfile;
        private ICurrentSettings<ObservableCollection<PrinterMapping>> _printerMappingsProvider;

        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentProfile = new ConversionProfile();
            _currentProfile.Name = "CurrentProfileName";
            _currentSettingsProvider.SelectedProfile = _currentProfile;
            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_currentProfile);
            _settings.ConversionProfiles.Add(new ConversionProfile());

            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();
            _profilesProvider.Settings.Returns(_settings.ConversionProfiles);

            _printerMappingsProvider = Substitute.For<ICurrentSettings<ObservableCollection<PrinterMapping>>>();
            _printerMappingsProvider.Settings.Returns(_ => _settings.ApplicationSettings.PrinterMappings);

            _translation = new ProfileMangementTranslation();

            _profileRemoveCommand = new ProfileRemoveCommand(_interactionRequest, _profilesProvider, _printerMappingsProvider, _currentSettingsProvider, new DesignTimeTranslationUpdater(), new InvokeImmediatelyDispatcher());
        }

        [Test]
        public void CanExecute_CurrentProfileIsNull_ReturnsFalse()
        {
            _currentSettingsProvider.SelectedProfile.ReturnsNull();
            Assert.IsFalse(_profileRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_CurrentProfileIsNotDeletable_ReturnsFalse()
        {
            _currentSettingsProvider.SelectedProfile.Properties.Deletable = false;
            Assert.IsFalse(_profileRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_CurrentProfileIsDeletable_ReturnsTrue()
        {
            _currentSettingsProvider.SelectedProfile.Properties.Deletable = true;
            Assert.IsTrue(_profileRemoveCommand.CanExecute(null));
        }

        [Test]
        public void Execute_RaisesMessageInteraction_CorrectTitleAndButtons()
        {
            _profileRemoveCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.RemoveProfile, interaction.Title);
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons);
        }

        [Test]
        public void Execute_UserCancelsInteraction_ProfileGetsNotRemoved()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _profileRemoveCommand.Execute(null);

            Assert.AreEqual(2, _profilesProvider.Settings.Count);
            Assert.Contains(_currentProfile, _profilesProvider.Settings);
        }

        [Test]
        public void Execute_UserAppliesInteraction_ProfileGetsRemoved()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _profileRemoveCommand.Execute(null);

            Assert.AreEqual(1, _profilesProvider.Settings.Count);
            Assert.IsFalse(_profilesProvider.Settings.Contains(_currentProfile));
        }

        [Test]
        public void Execute_ProfileIsNotMappedToPrinter_CorrectMessageTextAndIcon()
        {
            _settings.ApplicationSettings.PrinterMappings = new ObservableCollection<PrinterMapping>();

            _profileRemoveCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(MessageIcon.Question, interaction.Icon);
            var sb = new StringBuilder();
            sb.AppendLine(_currentProfile.Name);
            sb.AppendLine(_translation.RemoveProfileForSure);
            Assert.AreEqual(sb.ToString(), interaction.Text);
        }

        [Test]
        public void Execute_ProfileIsMappedToPrinters_MessageWithLinkedPrintersTextAndWarningIcon()
        {
            _settings.ApplicationSettings.PrinterMappings = new ObservableCollection<PrinterMapping>();
            var pm1 = new PrinterMapping("Printer1", _currentProfile.Guid);
            _settings.ApplicationSettings.PrinterMappings.Add(pm1);
            var pm2 = new PrinterMapping("Printer2", "Some other GUID");
            _settings.ApplicationSettings.PrinterMappings.Add(pm2);
            var pm3 = new PrinterMapping("Printer3", _currentProfile.Guid);
            _settings.ApplicationSettings.PrinterMappings.Add(pm3);

            _profileRemoveCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon);
            var sb = new StringBuilder();
            sb.AppendLine(_currentProfile.Name);
            sb.AppendLine(_translation.RemoveProfileForSure);
            sb.AppendLine();
            sb.AppendLine(_translation.GetProfileIsMappedToMessage(2));
            sb.AppendLine("Printer1");
            sb.AppendLine("Printer3");
            sb.AppendLine();
            sb.AppendLine(_translation.GetPrinterWillBeMappedToMessage(2));
            Assert.AreEqual(sb.ToString(), interaction.Text);
        }

        [Test]
        public void Execute_ProfileIsMappedToPrinters_UserAppliesInteraction_ProfileGetsRemovedPrintersGetMappedToDefaultGuid()
        {
            _settings.ApplicationSettings.PrinterMappings = new ObservableCollection<PrinterMapping>();
            var pm1 = new PrinterMapping("Printer1", _currentProfile.Guid);
            _settings.ApplicationSettings.PrinterMappings.Add(pm1);
            var pm2 = new PrinterMapping("Printer2", "Some other GUID");
            _settings.ApplicationSettings.PrinterMappings.Add(pm2);
            var pm3 = new PrinterMapping("Printer3", _currentProfile.Guid);
            _settings.ApplicationSettings.PrinterMappings.Add(pm3);

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _profileRemoveCommand.Execute(null);

            Assert.AreEqual(1, _profilesProvider.Settings.Count);
            Assert.IsFalse(_profilesProvider.Settings.Contains(_currentProfile));

            Assert.AreEqual(pm1.ProfileGuid, ProfileGuids.DEFAULT_PROFILE_GUID);
            Assert.AreEqual(pm3.ProfileGuid, ProfileGuids.DEFAULT_PROFILE_GUID);
        }
    }
}
