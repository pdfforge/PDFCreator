using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.Tokens;
using Ploeh.AutoFixture;
using Translatable;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class MailClientControlViewModelTest
    {
        private MailClientControlViewModel _viewModel;
        private ConversionProfile _profile;
        private UnitTestInteractionRequest _interactionRequest;
        private IClientTestEmail _mailClientTest;
        private TokenHelper _tokenHelper;
        private TokenReplacer _tokenReplacer;

        private readonly IFixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _mailClientTest = Substitute.For<IClientTestEmail>();

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _profile = new ConversionProfile();
            var currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            currentSettingsProvider.SelectedProfile.Returns(_profile);

            _tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            _viewModel = new MailClientControlViewModel(_interactionRequest, _mailClientTest, translationUpdater, currentSettingsProvider, new TokenViewModelFactory(currentSettingsProvider, new TokenHelper(new DesignTimeTranslationUpdater())), null);
        }

        [Test]
        public void RecipientsTokenViewModel_UsesRecipientsFromProfile()
        {
            var expectedString = _fixture.Create<string>();
            _profile.EmailClientSettings.Recipients = expectedString;
            Assert.AreEqual(expectedString, _viewModel.RecipientsTokenViewModel.Text);
        }

        [Test]
        public void RecipientsTokenViewModel_UpdatesRecipientsInProfile()
        {
            var expectedString = _fixture.Create<string>();
            _viewModel.RecipientsTokenViewModel.Text = expectedString;
            Assert.AreEqual(expectedString, _profile.EmailClientSettings.Recipients);
        }

        [Test]
        public void RecipientsTokenViewModel_TokensAreTokenListForEmail()
        {
            var tokenList = _tokenHelper.GetTokenListForEmailRecipients();

            foreach (var tokenWithCommand in _viewModel.RecipientsTokenViewModel.Tokens)
                tokenList.Remove(tokenWithCommand.Name);

            Assert.IsEmpty(tokenList);
        }

        [Test]
        public void RecipientsTokenViewModel_Preview_IsTextWithReplacedTokens()
        {
            var tokenName = _tokenHelper.GetTokenListForEmailRecipients()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.RecipientsTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue, _viewModel.RecipientsTokenViewModel.Preview);
        }
    }
}
