using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using Ploeh.AutoFixture;
using System.ComponentModel;
using Translatable;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class MetadataTabViewModelUnitTest
    {
        private ConversionProfile _profile;
        private readonly IFixture _fixture = new Fixture();
        private TranslationFactory _translationFactory;
        private ICurrentSettingsProvider _currentSettingsProvider;

        [SetUp]
        public void Setup()
        {
            _profile = new ConversionProfile();
            _translationFactory = new TranslationFactory();
        }

        private MetadataViewModel BuildViewModel()
        {
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.SelectedProfile.Returns(_profile);

            var translationUpdater = new TranslationUpdater(_translationFactory, Substitute.For<IThreadManager>());
            var dispatcher = Substitute.For<IDispatcher>();

            return new MetadataViewModel(translationUpdater,
                new TokenHelper(new DesignTimeTranslationUpdater()),
                new TokenViewModelFactory(_currentSettingsProvider,
                new TokenHelper(new DesignTimeTranslationUpdater())),
                _currentSettingsProvider, dispatcher);
        }

        [Test]
        public void ViewModel_Is_TranslatableViewModelBase()
        {
            // This test validates that we do not have to test the behaviour implemented by TranslatableViewModelBase

            var vm = BuildViewModel();

            Assert.IsInstanceOf<TranslatableViewModelBase<MetadataTabTranslation>>(vm);
        }

        [Test]
        public void TitleToken_UsesTitleTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();
            _profile.TitleTemplate = expectedString;
            var vm = BuildViewModel();

            Assert.AreEqual(expectedString, vm.TitleTokenViewModel.Preview);
        }

        [Test]
        public void TitleToken_UpdatesTitleTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();

            var vm = BuildViewModel();
            vm.TitleTokenViewModel.Text = expectedString;

            Assert.AreEqual(expectedString, _profile.TitleTemplate);
        }

        [Test]
        public void AuthorToken_UsesAuthorTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();
            _profile.AuthorTemplate = expectedString;
            var vm = BuildViewModel();

            Assert.AreEqual(expectedString, vm.AuthorTokenViewModel.Preview);
        }

        [Test]
        public void AuthorToken_UpdateAuthorTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();

            var vm = BuildViewModel();
            vm.AuthorTokenViewModel.Text = expectedString;

            Assert.AreEqual(expectedString, _profile.AuthorTemplate);
        }

        [Test]
        public void SubjectToken_UsesSubjectTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();
            _profile.SubjectTemplate = expectedString;
            var vm = BuildViewModel();

            Assert.AreEqual(expectedString, vm.SubjectTokenViewModel.Preview);
        }

        [Test]
        public void SubjectToken_UpdatesSubjectTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();

            var vm = BuildViewModel();
            vm.SubjectTokenViewModel.Text = expectedString;

            Assert.AreEqual(expectedString, _profile.SubjectTemplate);
        }

        [Test]
        public void KeywordToken_UsesKeywordTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();
            _profile.KeywordTemplate = expectedString;
            var vm = BuildViewModel();

            Assert.AreEqual(expectedString, vm.KeywordsTokenViewModel.Preview);
        }

        [Test]
        public void KeywordToken_UpdatesKeywordTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();

            var vm = BuildViewModel();
            vm.KeywordsTokenViewModel.Text = expectedString;

            Assert.AreEqual(expectedString, _profile.KeywordTemplate);
        }

        [Test]
        public void ViewModel_WhenTranslationIsUpdated_UpdatesTokenViewModels()
        {
            var vm = BuildViewModel();

            var titleProperty = new PropertyChangedListenerMock(vm, nameof(vm.TitleTokenViewModel));
            var authorProperty = new PropertyChangedListenerMock(vm, nameof(vm.AuthorTokenViewModel));
            var subjectProperty = new PropertyChangedListenerMock(vm, nameof(vm.SubjectTokenViewModel));
            var keywordsProperty = new PropertyChangedListenerMock(vm, nameof(vm.KeywordsTokenViewModel));

            _translationFactory.TranslationSource = Substitute.For<ITranslationSource>();

            Assert.IsTrue(titleProperty.WasCalled);
            Assert.IsTrue(authorProperty.WasCalled);
            Assert.IsTrue(subjectProperty.WasCalled);
            Assert.IsTrue(keywordsProperty.WasCalled);
        }

        [Test]
        public void ViewModel_WhenSelectedProfileIsChanged_UpdatesTokenViewModels()
        {
            var vm = BuildViewModel();

            var titleProperty = false;
            vm.TitleTokenViewModel.TextChanged += (sender, args) => titleProperty = true;

            _currentSettingsProvider.SelectedProfileChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs(""));

            Assert.IsTrue(titleProperty);
        }
    }
}
