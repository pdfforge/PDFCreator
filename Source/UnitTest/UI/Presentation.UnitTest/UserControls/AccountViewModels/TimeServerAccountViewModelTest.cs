using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Collections.Generic;
using Translatable;

namespace Presentation.UnitTest.UserControls.AccountViewModels
{
    [TestFixture]
    public class TimeServerAccountViewModelTest
    {
        private TimeServerAccountViewModel _viewModel;
        private TimeServerAccount _timeServerAccount;
        private TimeServerAccountInteraction _timeServerAccountInteraction;

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(null), new ThreadManager());

            _timeServerAccount = new TimeServerAccount
            {
                AccountId = new Guid().ToString(),
                Url = "www.randometimeserver.org",
                UserName = "UserName",
                Password = "Password",
                IsSecured = true
            };

            _timeServerAccountInteraction = new TimeServerAccountInteraction(_timeServerAccount, "TimeServerAccountTestTitle");

            _viewModel = new TimeServerAccountViewModel(translationUpdater);
        }

        [Test]
        public void DesignTimeViewModel_CheckClass_ClassNotNull()
        {
            TimeServerAccountViewModel viewModel = new DesignTimeTimeServerAccountViewModel();
            Assert.NotNull(viewModel);
        }

        [Test]
        public void TimeServerAccountViewModel_CheckClass_ClassNotNull()
        {
            TimeServerAccountViewModel viewModel = new TimeServerAccountViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));

            Assert.NotNull(viewModel);
        }

        [Test]
        public void SaveCanExecute_UrlIsSet_UsernameAndPasswordIsEmpty_IsSecuredIsFalse_ReturnsFalse()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "";
            _viewModel.Password = "";
            _viewModel.IsSecured = false;
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlAndUsernameAndPasswordIsEmpty_IsSecuredIsFalse_ReturnsFalse()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            _viewModel.Url = "";
            _viewModel.Username = "";
            _viewModel.Password = "";
            _viewModel.IsSecured = false;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlIsSet_UsernameAndPasswordIsEmpty_IsSecuredIsTrue_ReturnsFalse()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "";
            _viewModel.Password = "";
            _viewModel.IsSecured = true;
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlAndUsernameAndPasswordAreSet_ReturnsTrue()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.Password = "Not empty";
            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecute_UrlAndUsernameAreSet_PasswordIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            _viewModel.Url = "Not empty";
            _viewModel.Username = "Not empty";
            _viewModel.Password = "";
            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SetPassword_TriggersSaveCommandRaiseCanExecuteChanged()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            var wasCalled = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasCalled = true;

            _viewModel.Password = "Some value";

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void SetUrl_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Url = "New Url";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SetUsername_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
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
            Assert.IsFalse(_viewModel.IsSecured);
        }

        [Test]
        public void SaveCanExecuteChange_UrlIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);

            _viewModel.Url = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChange_UsernameIsEmptyAndIsSecuredIsTrue_ReturnsFalse()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);

            _viewModel.Username = "";
            _viewModel.IsSecured = true;

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveExecute_CallsFinishInteractionAndSuccessIsTrue()
        {
            _viewModel.SetInteraction(_timeServerAccountInteraction);
            var wasCalled = false;
            _viewModel.FinishInteraction += () => wasCalled = true;

            _viewModel.SaveCommand.Execute(null);

            Assert.IsTrue(wasCalled, "FinishInteraction");
            Assert.IsTrue(_timeServerAccountInteraction.Success, "Success");
        }

        [Test]
        public void SaveExecute_InteractionAccountHasCurrentValueWhileIDRemains()
        {
            var oldID = _timeServerAccount.AccountId;

            _viewModel.SetInteraction(_timeServerAccountInteraction);
            _viewModel.FinishInteraction = () => { };

            _viewModel.Url = "Changed Url";
            _viewModel.Username = "Changed Username";
            _viewModel.Password = "Changed Password";

            _viewModel.SaveCommand.Execute(null);

            Assert.AreEqual(oldID, _timeServerAccountInteraction.TimeServerAccount.AccountId, "AccountId Remains Unchanged");
            Assert.AreEqual("Changed Url", _timeServerAccountInteraction.TimeServerAccount.Url);
            Assert.AreEqual("Changed Username", _timeServerAccountInteraction.TimeServerAccount.UserName);
            Assert.AreEqual("Changed Password", _timeServerAccountInteraction.TimeServerAccount.Password);
        }

        [Test]
        public void SetInteraction_RaisesPropertyChangedForEveryProperty()
        {
            var changedPropertyList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => changedPropertyList.Add(args.PropertyName);

            _viewModel.SetInteraction(_timeServerAccountInteraction);

            Assert.Contains(nameof(_viewModel.Url), changedPropertyList, "Url");
            Assert.Contains(nameof(_viewModel.Username), changedPropertyList, "Username");
            Assert.Contains(nameof(_viewModel.Password), changedPropertyList, "Password");
            Assert.IsTrue(_viewModel.IsSecured, "Time Server is Secured");
        }

        [Test]
        public void SetInteraction_TriggersSaveCommandCanExecuteChanged()
        {
            var wasTriggered = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasTriggered = true;
            _viewModel.SetInteraction(_timeServerAccountInteraction);

            Assert.IsTrue(wasTriggered);
        }
    }
}
