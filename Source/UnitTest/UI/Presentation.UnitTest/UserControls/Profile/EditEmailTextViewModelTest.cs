using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Collections.Generic;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class EditEmailTextViewModelTest
    {
        private EditEmailTextViewModel _viewModel;
        private EditEmailTextInteraction _interaction;
        private SmtpTranslation _translation;
        private const string TestSignature = "Test Signature";
        private ITokenHelper _tokenHelper;
        private TokenReplacer _tokenReplacer;

        [SetUp]
        public void SetUp()
        {
            _interaction = new EditEmailTextInteraction("Subject", "Content", false, false);

            var translationUpdater = Substitute.For<ITranslationUpdater>();
            _translation = new SmtpTranslation();

            var mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            mailSignatureHelper.ComposeMailSignature().Returns(TestSignature);

            _tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            _viewModel = new EditEmailTextViewModel(translationUpdater, mailSignatureHelper, _tokenHelper, new TokenViewModelFactory(null, _tokenHelper));
            _viewModel.SetInteraction(_interaction);
        }

        [Test]
        public void SubjectTokenViewModel_SetWritesToInteractionSubject()
        {
            _viewModel.SubjectTokenViewModel.Text = "Subject from TokenViewModel";

            Assert.AreEqual("Subject from TokenViewModel", _viewModel.Interaction.Subject);
        }

        [Test]
        public void SubjectTokenViewModel_GetReadsFormInterctionSubject()
        {
            _viewModel.Interaction.Subject = "Subject from Interaction";

            Assert.AreEqual("Subject from Interaction", _viewModel.SubjectTokenViewModel.Text);
        }

        [Test]
        public void SubjectTokenViewModel_TokensAreTokenListForEmail()
        {
            var tokenList = _tokenHelper.GetTokenListForEmail();

            foreach (var tokenWithCommand in _viewModel.SubjectTokenViewModel.Tokens)
                tokenList.Remove(tokenWithCommand.Name);

            Assert.IsEmpty(tokenList);
        }

        [Test]
        public void SubjectTokenViewModel_ReplaceTokensFunction_TextIsNull_PreviewIsEmptyString()
        {
            _viewModel.SubjectTokenViewModel.Text = null;

            Assert.AreEqual("", _viewModel.SubjectTokenViewModel.Preview);
        }

        [Test]
        public void SubjectTokenViewModel_ReplaceTokensFunction_TextIsToken_PokenIsReplacedInPreview()
        {
            var tokenName = _tokenHelper.GetTokenListForEmail()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.SubjectTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue, _viewModel.SubjectTokenViewModel.Preview);
        }

        [Test]
        public void ContentTokenViewModel_SetWritesToInteractionContent()
        {
            _viewModel.ContentTokenViewModel.Text = "Content from TokenViewModel";

            Assert.AreEqual("Content from TokenViewModel", _viewModel.Interaction.Content);
        }

        [Test]
        public void ContentTokenViewModel_GetReadsFormInterctionContent()
        {
            _viewModel.Interaction.Content = "Content from Interaction";

            Assert.AreEqual("Content from Interaction", _viewModel.ContentTokenViewModel.Text);
        }

        [Test]
        public void ContentTokenViewModel_TokensAreTokenListForEmail()
        {
            var tokenList = _tokenHelper.GetTokenListForEmail();

            foreach (var tokenWithCommand in _viewModel.ContentTokenViewModel.Tokens)
                tokenList.Remove(tokenWithCommand.Name);

            Assert.IsEmpty(tokenList);
        }

        [Test]
        public void ContentTokenViewModel_ReplaceTokensFunction_TextIsNull_PreviewIsEmptyString()
        {
            _viewModel.ContentTokenViewModel.Text = null;

            Assert.AreEqual("", _viewModel.ContentTokenViewModel.Preview);
        }

        [Test]
        public void ContentTokenViewModel_ReplaceTokensFunction_TextIsTokenAddSignatureDisabled_TokenIsReplacedInPreviewWithoutSignature()
        {
            var tokenName = _tokenHelper.GetTokenListForEmail()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.AddSignature = false;
            _viewModel.ContentTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue, _viewModel.ContentTokenViewModel.Preview);
        }

        [Test]
        public void ContentTokenViewModel_ReplaceTokensFunction_TextIsTokenAddSignatureEnabled_TokenIsReplacedInPreviewWithSignature()
        {
            var tokenName = _tokenHelper.GetTokenListForEmail()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.AddSignature = true;
            _viewModel.ContentTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue + TestSignature, _viewModel.ContentTokenViewModel.Preview);
        }

        [Test]
        public void OkCommand_Execute_InteractionSucessIsTrue_FinishEventGetsCalled()
        {
            var wasCalled = false;
            _viewModel.FinishInteraction += () => wasCalled = true;

            _viewModel.OkCommand.Execute(null);

            Assert.IsTrue(_interaction.Success, "Success");
            Assert.IsTrue(wasCalled, "Finish interaction");
        }

        [Test]
        public void CancelCommand_Execute_InteractionSucessIsTrue_FinishEventGetsCalled()
        {
            var wasCalled = false;
            _viewModel.FinishInteraction += () => wasCalled = true;

            _viewModel.CancelCommand.Execute(null);

            Assert.IsFalse(_interaction.Success, "Success");
            Assert.IsTrue(wasCalled, "Finish interaction");
        }

        [Test]
        public void AddSignature_SetsValueInInteraction_RaisesPropertyChanged()
        {
            _interaction.AddSignature = false;

            var propertyList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyList.Add(args.PropertyName);
            _viewModel.ContentTokenViewModel.PropertyChanged += (sender, args) => propertyList.Add(args.PropertyName);

            _viewModel.AddSignature = true;

            Assert.IsTrue(_interaction.AddSignature, "Value in Interaction");
            Assert.Contains(nameof(_viewModel.AddSignature), propertyList);
            Assert.Contains(nameof(_viewModel.ContentTokenViewModel.Text), propertyList);
        }

        [Test]
        public void HandleInteractionObjectchanged_CallsRaisePropertyChanged()
        {
            var propertyList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyList.Add(args.PropertyName);
            _viewModel.ContentTokenViewModel.PropertyChanged += (sender, args) => propertyList.Add(args.PropertyName);

            _viewModel.SetInteraction(_interaction);

            Assert.Contains(nameof(_viewModel.ContentTokenViewModel), propertyList);
            Assert.Contains(nameof(_viewModel.SubjectTokenViewModel), propertyList);
            Assert.Contains(nameof(_viewModel.AddSignature), propertyList);
            Assert.Contains(nameof(_viewModel.Interaction), propertyList);
            Assert.Contains(nameof(_viewModel.ContentTokenViewModel.Text), propertyList);
        }
    }
}
