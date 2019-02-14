using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.Generic;
using Translatable;

namespace Presentation.UnitTest.UserControls.AccountViewModels
{
    [TestFixture]
    public class SmtpAccountViewModelTest
    {
        private SmtpAccountViewModel _viewModel;
        private SmtpAccount _smtpAccount;
        private SmtpAccountInteraction _smtpAccountInteraction;

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(null), new ThreadManager());

            _smtpAccount = new SmtpAccount
            {
                AccountId = "ID",
                Address = "Address",
                UserName = "UserName",
                Server = "Server",
                Password = "Password",
                Port = 0,
                Ssl = true
            };

            _smtpAccountInteraction = new SmtpAccountInteraction(_smtpAccount, "SmtpAccountTestTitle");

            _viewModel = new SmtpAccountViewModel(translationUpdater);
        }

        [Test]
        public void SaveCanExecute_ServerAndUsernameAreSet_AskForPasswordLater_ReturnsTrue()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            _viewModel.Address = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.Server = "NotEmpty";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "Does not matter";
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_ServerAndUsernameAreSet_DontAskForPasswordLaterWithEmptyPassword_ReturnsFalse()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            _viewModel.Address = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.Server = "NotEmpty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_ServerAndUsernameAreSet_DontAskForPasswordLaterWithPassword_ReturnsTrue()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            _viewModel.Address = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.Server = "NotEmpty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SetPassword_TriggersSaveCommandRaiseCanExecuteChanged()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            var wasCalled = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasCalled = true;

            _viewModel.Password = "Some value";

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void SetAddress_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Address = "New Address";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SetServer_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Server = "New Server";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SetUsername_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Username = "New Username";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void BeforeSetInteraction_ViewModelPropertiesDoNotThrowExecption()
        {
            Assert.IsNull(_viewModel.Address);
            Assert.IsNull(_viewModel.Username);
            Assert.IsNull(_viewModel.Server);
            Assert.IsNull(_viewModel.Password);
            Assert.IsFalse(_viewModel.Ssl);
            Assert.AreEqual(0, _viewModel.Port);
        }

        [Test]
        public void SaveCanExecuteChange_AddressIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);

            _viewModel.Address = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChange_ServerIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);

            _viewModel.Server = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChange_UsernameIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);

            _viewModel.Username = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveExecute_CallsFinishInteractionAndSuccessIsTrue()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            var wasCalled = false;
            _viewModel.FinishInteraction += () => wasCalled = true;

            _viewModel.SaveCommand.Execute(null);

            Assert.IsTrue(wasCalled, "FinishInteraction");
            Assert.IsTrue(_smtpAccountInteraction.Success, "Success");
        }

        [Test]
        public void SaveExecute_InteractionAccountHasCurrentValueWhileIDRemains()
        {
            var oldID = _smtpAccount.AccountId;

            _viewModel.SetInteraction(_smtpAccountInteraction);
            _viewModel.FinishInteraction = () => { };

            _viewModel.Address = "Changed Address";
            _viewModel.Username = "Changed Username";
            _viewModel.Server = "Changed Server";
            _viewModel.Password = "Changed Password";
            _viewModel.Port = 2;
            _viewModel.Ssl = false;

            _viewModel.SaveCommand.Execute(null);

            Assert.AreEqual(oldID, _smtpAccountInteraction.SmtpAccount.AccountId, "AccountId Remains Unchanged");
            Assert.AreEqual("Changed Address", _smtpAccountInteraction.SmtpAccount.Address);
            Assert.AreEqual("Changed Username", _smtpAccountInteraction.SmtpAccount.UserName);
            Assert.AreEqual("Changed Server", _smtpAccountInteraction.SmtpAccount.Server);
            Assert.AreEqual("Changed Password", _smtpAccountInteraction.SmtpAccount.Password);
            Assert.AreEqual(2, _smtpAccountInteraction.SmtpAccount.Port, "Port");
            Assert.IsFalse(_smtpAccountInteraction.SmtpAccount.Ssl, "Ssl");
        }

        [Test]
        public void SetInteraction_RaisesPropertyChangedForEveryProperty()
        {
            var changedPropertyList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => changedPropertyList.Add(args.PropertyName);

            _viewModel.SetInteraction(_smtpAccountInteraction);

            Assert.Contains(nameof(_viewModel.Address), changedPropertyList, "Address");
            Assert.Contains(nameof(_viewModel.Username), changedPropertyList, "Username");
            Assert.Contains(nameof(_viewModel.Server), changedPropertyList, "Server");
            Assert.Contains(nameof(_viewModel.Password), changedPropertyList, "Password");
            Assert.Contains(nameof(_viewModel.Port), changedPropertyList, "Port");
            Assert.Contains(nameof(_viewModel.Ssl), changedPropertyList, "Ssl");
        }

        [Test]
        public void SetInteraction_TriggersSaveCommandCanExecuteChanged()
        {
            var wasTriggered = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasTriggered = true;
            _viewModel.SetInteraction(_smtpAccountInteraction);

            Assert.IsTrue(wasTriggered);
        }

        [Test]
        public void SetInteraction_AllowConversionInterruptsEnabled_AskForPasswordLaterIsTrueIfPasswordIsEmpty()
        {
            _viewModel.AllowConversionInterrupts = true;

            _smtpAccountInteraction.SmtpAccount.Password = "Not empty";
            _viewModel.SetInteraction(_smtpAccountInteraction);

            Assert.IsFalse(_viewModel.AskForPasswordLater, "AskForPasswordLater should be false for set password");

            _smtpAccountInteraction.SmtpAccount.Password = "";
            _viewModel.SetInteraction(_smtpAccountInteraction);
            Assert.IsTrue(_viewModel.AskForPasswordLater, "AskForPasswordLater should be true for empty password");
        }

        [Test]
        public void SetInteraction_AllowConversionInterruptsDisabled_AskForPasswordLaterIsAlwaysFalse()
        {
            _viewModel.AllowConversionInterrupts = false;

            _smtpAccountInteraction.SmtpAccount.Password = "Not empty";
            _viewModel.SetInteraction(_smtpAccountInteraction);

            Assert.IsFalse(_viewModel.AskForPasswordLater);

            _smtpAccountInteraction.SmtpAccount.Password = "";
            _viewModel.SetInteraction(_smtpAccountInteraction);
            Assert.IsFalse(_viewModel.AskForPasswordLater);
        }

        [Test]
        public void SetAdressForEmptyUsername_UsernameIsEmpty_UsernameIsAdressAndRaisesRaisedPorpertyChanged()
        {
            var changedPropertyList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => changedPropertyList.Add(args.PropertyName);

            _viewModel.SetInteraction(_smtpAccountInteraction);
            var expectedUsername = "From Address";
            _viewModel.Address = expectedUsername;
            _viewModel.Username = "";

            _viewModel.SetAdressForEmptyUsername();

            Assert.AreEqual(expectedUsername, _viewModel.Username);
            Assert.Contains(nameof(_viewModel.Username), changedPropertyList, "Username raise poperty changed");
        }

        [Test]
        public void SetAdressForEmptyUsername_UsernameIsNotEmpty_UsernameRemains()
        {
            _viewModel.SetInteraction(_smtpAccountInteraction);
            var expectedUsername = "not empty";
            _viewModel.Address = "Some adress";
            _viewModel.Username = expectedUsername;

            _viewModel.SetAdressForEmptyUsername();

            Assert.AreEqual(expectedUsername, _viewModel.Username);
        }
    }
}
