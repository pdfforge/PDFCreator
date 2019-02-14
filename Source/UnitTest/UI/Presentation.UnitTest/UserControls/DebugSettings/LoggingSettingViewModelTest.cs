using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using SystemInterface.IO;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class LoggingSettingViewModelTest
    {
        private IInteractionInvoker _invoker;
        private ITranslationUpdater _translationUpdater;
        private ISettingsProvider _settingsProvider;
        private IGpoSettings _gpoSettings;
        private IFile _fileWrap;
        private ICommandLocator _commandLocator;
        private PdfCreatorSettings _applicationSettings;

        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionInvoker>();
            _translationUpdater = Substitute.For<ITranslationUpdater>();
            _translationUpdater
                .When(x => x.RegisterAndSetTranslation(Arg.Any<ITranslatableViewModel<DebugSettingsTranslation>>()))
                .Do(x =>
                {
                    var viewModel = x.Arg<ITranslatableViewModel<DebugSettingsTranslation>>();
                    viewModel.Translation = new TranslationFactory().CreateTranslation<DebugSettingsTranslation>();
                });

            IStorage storage = Substitute.For<IStorage>();
            _applicationSettings = Substitute.For<PdfCreatorSettings>();
            _settingsProvider = Substitute.For<ISettingsProvider>();
            _settingsProvider.Settings.Returns(_applicationSettings);

            _gpoSettings = Substitute.For<IGpoSettings>();
            _fileWrap = Substitute.For<IFile>();
            _commandLocator = Substitute.For<ICommandLocator>();
        }

        private LoggingSettingViewModel BuildViewModel()
        {
            return new LoggingSettingViewModel(_invoker, _fileWrap, _translationUpdater, _gpoSettings, _commandLocator, new DesignTimeCurrentSettings<ApplicationSettings>());
        }

        [Test]
        public void Check_Properties()
        {
            var viewModel = BuildViewModel();

            Assert.NotNull(viewModel.ShowLogFileCommand);
            Assert.NotNull(viewModel.ClearLogFileCommand);
            Assert.NotNull(viewModel.Translation);

            var loggingLevels = Enum.GetValues(typeof(LoggingLevel)) as LoggingLevel[];

            Assert.AreEqual(loggingLevels, viewModel.LoggingValues);
        }

        [Test]
        public void LogFileExists_ClearLogFileExecute_WriteToLogFile()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(true);
            var viewModel = BuildViewModel();

            viewModel.ClearLogFileCommand.Execute(null);

            Received.InOrder(() =>
            {
                _fileWrap.Exists(Arg.Any<string>());
                _fileWrap.WriteAllText(Arg.Any<string>(), Arg.Any<string>());
            });
        }

        [Test]
        public void LogFileNotExist_ClearLogFileExecute_DontWriteLogFile()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(false);
            var viewModel = BuildViewModel();

            viewModel.ClearLogFileCommand.Execute(null);

            Received.InOrder(() => _fileWrap.Exists(Arg.Any<string>()));
        }

        [Test]
        public void LoggingFileExists_ShowLogFileExecute_ProcessStarterGetsCalled()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(true);
            var viewModel = BuildViewModel();

            viewModel.ShowLogFileCommand.Execute(null);

            Received.InOrder(() =>
            {
                _commandLocator.GetCommand<QuickActionOpenExplorerLocationCommand>();
                _fileWrap.Exists(Arg.Any<string>());
            });
        }

        [Test]
        public void LoggingFileExistsNot_ShowLogFileExecute_UserNoLogFileInteractionIsCalled()
        {
            _fileWrap.Exists(Arg.Any<string>()).Returns(false);
            var viewModel = BuildViewModel();

            _invoker
                .When(x => _invoker.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    var message = x.Arg<MessageInteraction>();
                    Assert.AreEqual(viewModel.Translation.NoLogFile, message.Title);
                    Assert.AreEqual(viewModel.Translation.NoLogFileAvailable, message.Text);
                    Assert.AreEqual(message.Buttons, MessageOptions.OK);
                    Assert.AreEqual(message.Icon, MessageIcon.Warning);
                });

            viewModel.ShowLogFileCommand.Execute(null);
        }
    }
}
