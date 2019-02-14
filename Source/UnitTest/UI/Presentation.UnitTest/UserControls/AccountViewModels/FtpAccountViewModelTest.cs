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
    public class FtpAccountViewModelTest
    {
        private FtpAccountViewModel _viewModel;
        private FtpAccount _ftpAccount;
        private FtpAccountInteraction _ftpAccountInteraction;

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(null), new ThreadManager());

            _ftpAccount = new FtpAccount();
            _ftpAccount.AccountId = "ID";
            _ftpAccount.UserName = "UserName";
            _ftpAccount.Server = "Server";
            _ftpAccount.Password = "Password";

            _ftpAccountInteraction = new FtpAccountInteraction(_ftpAccount, "FtpAccountTestTitle");

            _viewModel = new FtpAccountViewModel(translationUpdater);
        }

        [Test]
        public void SetServer_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Server = "New Server";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SetUserName_TriggersSaveCommandCanExecuteChanged()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.UserName = "New UserName";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void BeforeSetInteraction_ViewModelPropertiesDoNotThrowException()
        {
            Assert.IsNull(_viewModel.UserName);
            Assert.IsNull(_viewModel.Server);
            Assert.IsNull(_viewModel.Password);
        }

        [Test]
        public void SaveCanExecuteChanged_ServerAndUsernameAreSet_AskForPasswordLater_ReturnsTrue()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);

            _viewModel.UserName = "Not empty";
            _viewModel.Server = "NotEmpty";
            _viewModel.AskForPasswordLater = true;
            _viewModel.Password = "Does not matter";

            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChanged_ServerAndUsernameAreSet_DontAskForPasswordLaterWithEmptyPassword_ReturnsFalse()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);

            _viewModel.UserName = "Not empty";
            _viewModel.Server = "NotEmpty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChanged_ServerAndUsernameAreSet_DontAskForPasswordLaterWithPassword_ReturnsTrue()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);

            _viewModel.UserName = "Not empty";
            _viewModel.Server = "NotEmpty";
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "Not empty";

            Assert.IsTrue(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChanged_ServerIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);

            _viewModel.Server = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveCanExecuteChanged_UserNameIsEmpty_ReturnsFalse()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);

            _viewModel.UserName = "";

            Assert.IsFalse(_viewModel.SaveCommand.CanExecute(null));
        }

        [Test]
        public void SaveExecute_CallsFinsihInteractionAndSuccessIsTrue()
        {
            _viewModel.SetInteraction(_ftpAccountInteraction);
            var wasCalled = false;
            _viewModel.FinishInteraction += () => wasCalled = true;

            _viewModel.SaveCommand.Execute(null);

            Assert.IsTrue(wasCalled, "FinishInteraction");
            Assert.IsTrue(_ftpAccountInteraction.Success, "Success");
        }

        [Test]
        public void SaveExecute_InteractionAccountHasCurrentValuesWhileIDRemains()
        {
            var oldID = _ftpAccount.AccountId;

            _viewModel.SetInteraction(_ftpAccountInteraction);
            _viewModel.FinishInteraction = () => { };

            _viewModel.UserName = "Changed UserName";
            _viewModel.Server = "Changed Server";
            _viewModel.Password = "Changed Password";

            _viewModel.SaveCommand.Execute(null);

            Assert.AreEqual(oldID, _ftpAccountInteraction.FtpAccount.AccountId);
            Assert.AreEqual(_ftpAccountInteraction.FtpAccount.UserName, "Changed UserName");
            Assert.AreEqual(_ftpAccountInteraction.FtpAccount.Server, "Changed Server");
            Assert.AreEqual(_ftpAccountInteraction.FtpAccount.Password, "Changed Password");
        }

        [Test]
        public void SetInteraction_CallsRaisePropertyChangedForAllProperties()
        {
            var changedPropertyList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => changedPropertyList.Add(args.PropertyName);

            _viewModel.SetInteraction(_ftpAccountInteraction);

            Assert.Contains(nameof(_viewModel.UserName), changedPropertyList, "UserName");
            Assert.Contains(nameof(_viewModel.Server), changedPropertyList, "Server");
            Assert.Contains(nameof(_viewModel.Password), changedPropertyList, "Password");
            Assert.Contains(nameof(_viewModel.AskForPasswordLater), changedPropertyList, "AskForPasswordlater");
        }

        [Test]
        public void SetInteraction_AllowConversionInterruptsEnabled_AskForPasswordLaterIsTrueIfPasswordIsEmpty()
        {
            _viewModel.AllowConversionInterrupts = true;

            _ftpAccountInteraction.FtpAccount.Password = "Not empty";
            _viewModel.SetInteraction(_ftpAccountInteraction);

            Assert.IsFalse(_viewModel.AskForPasswordLater, "AskForPasswordLater should be false for set password");

            _ftpAccountInteraction.FtpAccount.Password = "";
            _viewModel.SetInteraction(_ftpAccountInteraction);
            Assert.IsTrue(_viewModel.AskForPasswordLater, "AskForPasswordLater should be true for empty password");
        }

        [Test]
        public void SetInteraction_AllowConversionInterruptsDisabled_AskForPasswordLaterIsAlwaysFalse()
        {
            _viewModel.AllowConversionInterrupts = false;

            _ftpAccountInteraction.FtpAccount.Password = "Not empty";
            _viewModel.SetInteraction(_ftpAccountInteraction);

            Assert.IsFalse(_viewModel.AskForPasswordLater);

            _ftpAccountInteraction.FtpAccount.Password = "";
            _viewModel.SetInteraction(_ftpAccountInteraction);
            Assert.IsFalse(_viewModel.AskForPasswordLater);
        }

        [Test]
        public void SetInteraction_TriggersSaveCommandCanExecuteChanged()
        {
            var wasRaised = false;
            _viewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.SetInteraction(_ftpAccountInteraction);

            Assert.IsTrue(wasRaised);
        }
    }
}
