using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using Translatable;

namespace Presentation.UnitTest.UserControls.GeneralSettings
{
    [TestFixture]
    public class TitleReplacementsViewModelUnitTest
    {
        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _commandLocator = Substitute.For<ICommandLocator>();

            _settingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _settings = new PdfCreatorSettings();

            _applicationSettings = new ApplicationSettings();
            _settings.ApplicationSettings = _applicationSettings;
            _titleReplacements = new ObservableCollection<TitleReplacement>();
            _titleReplacementsSettingsProvider = Substitute.For<ICurrentSettings<ObservableCollection<TitleReplacement>>>();
            _titleReplacementsSettingsProvider.Settings.Returns(_titleReplacements);
            _applicationSettings.TitleReplacement = _titleReplacements;
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        private ObservableCollection<TitleReplacement> _titleReplacements;
        private ITranslationUpdater _translationUpdater;
        private ICommandLocator _commandLocator;
        private ICurrentSettingsProvider _settingsProvider;
        private PdfCreatorSettings _settings;
        private ApplicationSettings _applicationSettings;
        private ICurrentSettings<ObservableCollection<TitleReplacement>> _titleReplacementsSettingsProvider;

        private TitleReplacementsViewModel BuildViewModel()
        {
            var viewModel = new TitleReplacementsViewModel(_translationUpdater, _titleReplacementsSettingsProvider, _settingsProvider, _commandLocator, null);
            return viewModel;
        }

        [TestCase(".txt", "yxvyxv", ReplacementType.End, "test.txt", "test")]
        [TestCase("Kartoffel", "yxcx", ReplacementType.Start, "KartoffelSalat", "Salat")]
        [TestCase("Ei", "xcyxc", ReplacementType.Replace, "KartEiofEifelSEialaEit", "KartoffelSalat")]
        public void SetupFilter_SetSearchText_ProperlyFiltered(string search, string replace, ReplacementType type, string sample, string result)
        {
            _titleReplacementsSettingsProvider.Settings.Add(new TitleReplacement(type, search, replace));
            var viewModel = BuildViewModel();
            viewModel.SampleText = sample;
            Assert.AreEqual(result, viewModel.ReplacedSampleText);
        }

        [Test]
        public void BuildViewModel_PropertiesAreSet()
        {
            var viewModel = BuildViewModel();
            Assert.AreEqual(viewModel.SampleText, "Microsoft Word - Sample Text.doc");
            Assert.AreEqual(viewModel.ReplacedSampleText, "Microsoft Word - Sample Text.doc");
            Assert.NotNull(viewModel.Translation);
        }

        [Test]
        public void DesignTimeViewModel()
        {
            var designTime = new DesignTimeTitleReplacementsViewModel();
            Assert.NotNull(designTime);
        }

        [Test]
        public void NoReplacements_SetSampleText_ReplacementTextIsEqualToSampleText()
        {
            var text = "Lore Ipsum";
            var viewModel = BuildViewModel();

            viewModel.SampleText = text;
            Assert.AreEqual(text, viewModel.ReplacedSampleText);
        }
    }
}
