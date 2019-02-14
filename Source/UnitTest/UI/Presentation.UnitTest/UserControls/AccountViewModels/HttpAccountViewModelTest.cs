using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Collections.Generic;
using Translatable;

namespace Presentation.UnitTest.UserControls.AccountViewModels
{
    [TestFixture]
    public class HttpAccountViewModelTest
    {
        private HttpAccountViewModel _viewModel;
        private HttpAccount _httpAccount;
        private HttpAccountInteraction _httpAccountInteraction;

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(null), new ThreadManager());

            _httpAccount = new HttpAccount
            {
                AccountId = new Guid().ToString(),
                Url = "www.pdfforge.org",
                UserName = "UserName",
                Password = "Password",
                Timeout = 60,
                IsBasicAuthentication = true
            };

            _httpAccountInteraction = new HttpAccountInteraction(_httpAccount, "HttpAccountTestTitle");

            _viewModel = new HttpAccountViewModel(translationUpdater);
        }

        [Test]
        public void SaveCanExecute_SetTimeout()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "http://testsite.com";
            _viewModel.Timeout = 120;
            Assert.AreEqual(_httpAccount.Timeout, _viewModel.Timeout);
        }

        [Test]
        public void SaveCanExecute_AccountIsNull_TimeoutIsDefaultValue()
        {
            _httpAccount = null;
            Assert.AreEqual(60, _viewModel.Timeout);
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpAndUsernameIsSet_AskForPasswordLater_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "http://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "Does not matter";
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpsAndUsernameIsSet_AskForPasswordLater_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "https://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "Does not matter";
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlAndUsernameAreSet_DontAskForPasswordLaterWithEmptyPassword_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpAndUsernameIsSet_DontAskForPasswordLaterWithPassword_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "http://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpsAndUsernameIsSet_DontAskForPasswordLaterWithPassword_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "https://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsNotHttpOrHttpsAndUsernameIsSet_DontAskForPasswordLaterWithPassword_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpAndUsernameIsSet_AskForPasswordLaterWithEmptyPasswordAndBasicAuthenticationIsFalse_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "http://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "";
            _viewModel.HasBasicAuthentication = false;
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpAndUsernameIsEmpty_BasicAuthenticationChecked_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "http://testsite.com";
            _viewModel.Username = "";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "";
            _viewModel.HasBasicAuthentication = true;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpPasswordIsEmptyAndAskForPasswortLaterNotChecked_BasicAuthenticationChecked_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "http://testsite.com";
            _viewModel.Username = "Not Empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "";
            _viewModel.HasBasicAuthentication = true;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpsAndUsernameIsSet_AskForPasswordLaterWithEmptyPasswordAndBasicAuthenticationIsFalse_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "https://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "";
            _viewModel.HasBasicAuthentication = false;
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsNeitherHttpNorHttpsAndUsernameIsSet_AskForPasswordLaterWithEmptyPasswordAndBasicAuthenticationIsFalse_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "";
            _viewModel.HasBasicAuthentication = false;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_OnlyUrlIsSet_AskForPasswordLaterWithEmptyPasswordAndBasicAuthenticationIsTrue_ReturnFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "";
            _viewModel.HasBasicAuthentication = true;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlAndUsernameAreSet_DoNotAskForPasswordLaterWithEmptyPasswordAndBasicAuthenticationIsTrue_ReturnFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "";
            _viewModel.HasBasicAuthentication = true;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpAndUsernameIsSet_DoNotAskForPasswordLaterWithPasswordSetAndBasicAuthenticationIsTrue_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "http://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            _viewModel.HasBasicAuthentication = true;
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsHttpsAndUsernameIsSet_DoNotAskForPasswordLaterWithPasswordSetAndBasicAuthenticationIsTrue_ReturnsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "https://testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            _viewModel.HasBasicAuthentication = true;
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsNeitherHttpNorHttpsAndUsernameIsSet_DoNotAskForPasswordLaterWithPasswordSetAndBasicAuthenticationIsTrue_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            _viewModel.HasBasicAuthentication = true;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChange_UrlIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);

            _viewModel.Url = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChange_UrlIsNotHttpOrHttps_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);

            _viewModel.Url = "testsite.com";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChange_UsernameIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);

            _viewModel.Username = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveExecute_CallsFinishInteractionAndSuccessIsTrue()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            var wasCalled = false;
            _viewModel.FinishInteraction += () => wasCalled = true;

            _viewModel.SaveCommand.Execute(null);

            Assert.IsTrue(wasCalled, "FinishInteraction");
            Assert.IsTrue(_httpAccountInteraction.Success, "Success");
        }

        [Test]
        public void SaveExecute_InteractionAccountHasCurrentValueWhileIDRemains()
        {
            var oldID = _httpAccount.AccountId;

            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.FinishInteraction = () => { };

            _viewModel.Url = "Changed Url";
            _viewModel.Username = "Changed Username";
            _viewModel.Password = "Changed Password";

            _viewModel.SaveCommand.Execute(null);

            Assert.AreEqual(oldID, _httpAccountInteraction.HttpAccount.AccountId, "AccountId Remains Unchanged");
            Assert.AreEqual("Changed Url", _httpAccountInteraction.HttpAccount.Url);
            Assert.AreEqual("Changed Username", _httpAccountInteraction.HttpAccount.UserName);
            Assert.AreEqual("Changed Password", _httpAccountInteraction.HttpAccount.Password);
        }

        [Test]
        public void SaveCanExecute_UrlIsNeitherUriSchemeHttpNorUriSchemeHttps_ReturnsFasle()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "testsite.com";
            _viewModel.Username = "Not empty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsFtp_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "ftp://www.validurlbutnothttp.com";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsLdap_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "ldap://www.ldapurl.com";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsMailto_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "mailto:someone@example.com";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsFile_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "file://localhost.com";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsIrc_ReturnsFalse()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            _viewModel.Url = "irc://irc.dal.net";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SetPassword_TriggersSaveCommandRaiseCanExecuteChanged()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            var wasCalled = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasCalled = true;

            _viewModel.Password = "Some value";

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void SetUrl_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Url = "New Url";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SetUsername_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_httpAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Username = "New Username";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void BeforeSetInteraction_ViewModelPropertiesDoNotThrowExecption()
        {
            Assert.IsNull(_viewModel.Url);
            Assert.IsNull(_viewModel.Username);
            Assert.IsNull(_viewModel.Password);
            Assert.IsFalse(_viewModel.HasBasicAuthentication);
        }

        [Test]
        public void SetInteraction_RaisesPropertyChangedForEveryProperty()
        {
            var changedPropertyList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => changedPropertyList.Add(args.PropertyName);

            _viewModel.SetInteraction(_httpAccountInteraction);

            Assert.Contains(nameof(_viewModel.Url), changedPropertyList, "Url");
            Assert.Contains(nameof(_viewModel.Username), changedPropertyList, "Username");
            Assert.Contains(nameof(_viewModel.Password), changedPropertyList, "Password");
            Assert.IsTrue(_viewModel.HasBasicAuthentication, "Has Basic Authentication");
        }

        [Test]
        public void SetInteraction_TriggersSaveCommandCanExecuteChanged()
        {
            var wasTriggered = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasTriggered = true;
            _viewModel.SetInteraction(_httpAccountInteraction);

            Assert.IsTrue(wasTriggered);
        }

        [Test]
        public void SetInteraction_AllowConversionInterruptsEnabled_AskForPasswordLaterIsTrueIfPasswordIsEmpty()
        {
            _viewModel.AllowConversionInterrupts = true;

            _httpAccountInteraction.HttpAccount.Password = "Not empty";
            _viewModel.SetInteraction(_httpAccountInteraction);

            Assert.IsFalse(_viewModel.AskForPasswordLater, "AskForPasswordLater should be false for set password");

            _httpAccountInteraction.HttpAccount.Password = "";
            _viewModel.SetInteraction(_httpAccountInteraction);
            Assert.IsTrue(_viewModel.AskForPasswordLater, "AskForPasswordLater should be true for empty password");
        }

        [Test]
        public void SetInteraction_AllowConversionInterruptsDisabled_AskForPasswordLaterIsAlwaysFalse()
        {
            _viewModel.AllowConversionInterrupts = false;

            _httpAccountInteraction.HttpAccount.Password = "Not empty";
            _viewModel.SetInteraction(_httpAccountInteraction);

            Assert.IsFalse(_viewModel.AskForPasswordLater);

            _httpAccountInteraction.HttpAccount.Password = "";
            _viewModel.SetInteraction(_httpAccountInteraction);
            Assert.IsFalse(_viewModel.AskForPasswordLater);
        }
    }
}
