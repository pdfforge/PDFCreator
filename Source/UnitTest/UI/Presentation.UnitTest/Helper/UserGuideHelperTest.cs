using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.UserGuide;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace Presentation.UnitTest.Helper
{
    [TestFixture]
    public class UserGuideHelperTest
    {
        [SetUp]
        public void Setup()
        {
            _languages = new List<Language>();
            _languages.Add(new Language { CommonName = "English", Iso2 = "en" });
            _languages.Add(new Language { CommonName = "German", Iso2 = "de" });

            _fileWrap = Substitute.For<IFile>();
            _userGuideLauncher = Substitute.For<IUserGuideLauncher>();
            _settingsProvider = Substitute.For<ISettingsProvider>();
            _settingsProvider.Settings.Returns(new PdfCreatorSettings());
            _assemblyHelper = Substitute.For<IAssemblyHelper>();

            _languageProvider = Substitute.For<ILanguageProvider>();
            _languageProvider.GetAvailableLanguages().Returns(_languages);
            _languageProvider.FindBestLanguage("en").Returns(_languages[0]);

            _assemblyHelper.GetAssemblyDirectory().Returns(AssemblyPath);
        }

        private IFile _fileWrap;
        private IUserGuideLauncher _userGuideLauncher;
        private IAssemblyHelper _assemblyHelper;
        private ISettingsProvider _settingsProvider;
        private ILanguageProvider _languageProvider;
        private List<Language> _languages;

        private const string AssemblyPath = @"X:\MyPath";

        private UserGuideHelper BuildUserGuideHelper()
        {
            return new UserGuideHelper(_fileWrap, _assemblyHelper, _userGuideLauncher, _settingsProvider, _languageProvider);
        }

        [Test]
        public void SetLanguage_WithExistingLanguageAndEnglishExists_SetsLanguage()
        {
            var userGuideHelper = BuildUserGuideHelper();
            var englishPath = Path.Combine(AssemblyPath, "PDFCreator_english.chm");
            var expectedPath = Path.Combine(AssemblyPath, "PDFCreator_German.chm");
            _fileWrap.Exists(englishPath).Returns(true);
            _fileWrap.Exists(expectedPath).Returns(true);

            var germanLanguage = new Language() { CommonName = "German", Iso2 = "de" };
            _languages.Add(germanLanguage);
            _settingsProvider.GetApplicationLanguage().Returns("de");

            userGuideHelper.UpdateLanguage();

            Assert.AreEqual(1, _userGuideLauncher.ReceivedCalls().Count());
            _userGuideLauncher.Received().SetUserGuide(expectedPath);
        }

        [Test]
        public void SetLanguage_WithNonexistentLanguage_DoesNotUpdateLauncher()
        {
            var userGuideHelper = BuildUserGuideHelper();
            _settingsProvider.GetApplicationLanguage().Returns("xy");

            userGuideHelper.UpdateLanguage();

            _userGuideLauncher.DidNotReceiveWithAnyArgs().SetUserGuide(Arg.Any<string>());
        }

        [Test]
        public void SetLanguage_WithNonexistentLanguageAndEnglishExists_SetsEnglish()
        {
            var userGuideHelper = BuildUserGuideHelper();
            var englishPath = Path.Combine(AssemblyPath, "PDFCreator_english.chm");
            _fileWrap.Exists(englishPath).Returns(true);
            _settingsProvider.GetApplicationLanguage().Returns("xy");

            userGuideHelper.UpdateLanguage();

            Assert.AreEqual(1, _userGuideLauncher.ReceivedCalls().Count());
            _userGuideLauncher.Received().SetUserGuide(englishPath);
        }

        [Test]
        public void ShowHelp_WithAnnotatedTopic_CallsShowHelp()
        {
            var userGuideHelper = BuildUserGuideHelper();

            userGuideHelper.ShowHelp(HelpTopic.General);

            Assert.AreEqual(1, _userGuideLauncher.ReceivedCalls().Count());
            _userGuideLauncher.Received().ShowHelpTopic(HelpTopic.General);
        }
    }
}
