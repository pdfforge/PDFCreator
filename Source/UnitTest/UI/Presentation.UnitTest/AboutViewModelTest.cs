using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using SystemInterface.IO;

namespace Presentation.UnitTest
{
    [TestFixture]
    public class AboutWindowViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _versionHelper = Substitute.For<IVersionHelper>();
            _versionHelper.FormatWithBuildNumber().Returns(VersionText);

            var fileWrap = Substitute.For<IFile>();
            fileWrap.Exists(Arg.Any<string>()).Returns(true);

            _translationUpdater = Substitute.For<ITranslationUpdater>();

            _commandLocator = Substitute.For<ICommandLocator>();

            InitAboutViewModel();
        }

        private AboutViewModel _aboutViewModel;
        private IVersionHelper _versionHelper;
        private const string VersionText = "v1.2.3";
        private ITranslationUpdater _translationUpdater;
        private ICommandLocator _commandLocator;

        private void InitAboutViewModel()
        {
            _aboutViewModel = new AboutViewModel(_versionHelper, _translationUpdater, _commandLocator, new DesignTimeApplicationNameProvider(), null);
        }

        [Test]
        public void Initialize_SetVersionTextFromVersionHelper()
        {
            Assert.AreEqual(VersionText, _aboutViewModel.VersionText);
        }

        [Test]
        public void Initialize_TranslationHelperAppliesTranslation()
        {
            _translationUpdater.Received().RegisterAndSetTranslation(Arg.Is(_aboutViewModel));
        }

        [Test]
        public void Initialize_CommandLocatorSetsShowManualCommandWithCorrectHelpTopic()
        {
            var userGuideGeneralCommand = Substitute.For<ICommand>();
            _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.General).Returns(userGuideGeneralCommand);
            InitAboutViewModel();
            Assert.AreSame(userGuideGeneralCommand, _aboutViewModel.ShowManualCommand);
        }

        [Test]
        public void Initialize_CommandLocatorSetsShowLicenseCommandWithCorrectHelpTopic()
        {
            var showLicenseCommand = Substitute.For<ICommand>();
            _commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.License).Returns(showLicenseCommand);
            InitAboutViewModel();
            Assert.AreSame(showLicenseCommand, _aboutViewModel.ShowLicenseCommand);
        }

        [Test]
        public void Initialize_CommandLocatorSetsPdfforgeWebsiteCommandWithCorrectUrls()
        {
            var pdfforgeWebsiteCommand = Substitute.For<ICommand>();
            _commandLocator.GetInitializedCommand<UrlOpenCommand, String>(Urls.PdfforgeWebsiteUrl).Returns(pdfforgeWebsiteCommand);
            InitAboutViewModel();
            Assert.AreSame(pdfforgeWebsiteCommand, _aboutViewModel.PdfforgeWebsiteCommand);
        }

        /* Remove when it's is really removed
        [Test]
        public void Initialize_CommandLocatorSetsFacebookCommandWithCorrectUrls()
        {
            var facebookCommand = Substitute.For<ICommand>();
            _commandLocator.GetInitializedCommand<UrlOpenCommand, String>(Urls.Facebook).Returns(facebookCommand);
            InitAboutViewModel();
            Assert.AreSame(facebookCommand, _aboutViewModel.FacebookCommand);
        }
        */

        [Test]
        public void Initialize_CommandLocatorSetsPrioritySupportCommand()
        {
            var openPriortiySupportUrlCommand = Substitute.For<ICommand>();
            _commandLocator.GetCommand<PrioritySupportUrlOpenCommand>().Returns(openPriortiySupportUrlCommand);
            InitAboutViewModel();
            Assert.AreSame(openPriortiySupportUrlCommand, _aboutViewModel.PrioritySupportCommand);
        }

        [Test]
        public void SetTranslation_RaisesRaisedPropertyChanged()
        {
            var changedProprtiesList = new List<string>();
            _aboutViewModel.PropertyChanged += (sender, args) => changedProprtiesList.Add(args.PropertyName);

            _aboutViewModel.Translation = new AboutViewTranslation();

            Assert.Contains(nameof(_aboutViewModel.Translation), changedProprtiesList);
        }
    }
}
