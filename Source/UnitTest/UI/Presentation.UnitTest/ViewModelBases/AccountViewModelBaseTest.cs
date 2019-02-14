using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Translatable;

namespace Presentation.UnitTest.ViewModelBases
{
    internal class TestClassForAccountViewModelBaseTest : AccountViewModelBase<TestAccountInteraction, TestAccountTranslation>
    {
        public bool SaveExecuteWasCalled = false;
        public bool SaveCanExecuteWasCalled = false;

        public TestClassForAccountViewModelBaseTest(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        protected override void SaveExecute()
        {
            SaveExecuteWasCalled = true;
        }

        protected override bool SaveCanExecute()
        {
            SaveCanExecuteWasCalled = true;
            return false;
        }
    }

    internal class TestAccountInteraction : AccountInteractionBase
    { }

    internal class TestAccountTranslation : ITranslatable
    { }

    [TestFixture]
    public class AccountViewModelBaseTest
    {
        private TestClassForAccountViewModelBaseTest _accountViewModel;
        private AccountInteractionBase _interaction;
        private ITranslationUpdater _translationUpdater;

        [SetUp]
        public void SetUp()
        {
            _interaction = new TestAccountInteraction();
            _interaction.Success = true;
            _interaction.Title = "Test Interaction Title";

            _translationUpdater = Substitute.For<ITranslationUpdater>();
            _accountViewModel = new TestClassForAccountViewModelBaseTest(_translationUpdater);
            _accountViewModel.SetInteraction(_interaction);
        }

        [Test]
        public void TitleFromInteraction_IsSetInViewModel()
        {
            Assert.AreEqual(_interaction.Title, _accountViewModel.Title);
        }

        [Test]
        public void CancelCommand_Execute_CallsFinishInteractionAndInteractionSuccessIsFalse()
        {
            var wasCalled = false;
            _accountViewModel.FinishInteraction = () => wasCalled = true;

            _accountViewModel.CancelCommand.Execute(null);

            Assert.IsFalse(_interaction.Success, "Interaction success");
            Assert.IsTrue(wasCalled, "FinishInteraction");
        }

        [Test]
        public void SetNoPasswordChecked_ValueIsFalse_SetPasswordActionWasNotCalled()
        {
            var wasCalled = false;

            _accountViewModel.AskForPasswordLater = false;

            Assert.IsFalse(wasCalled, "SetPasswordAction");
        }

        [Test]
        public void SetNoPasswordChecked_RaisesSaveCanExecuteChanged()
        {
            var wasCalled = false;
            _accountViewModel.SaveCommand.CanExecuteChanged += (sender, args) => wasCalled = true;

            _accountViewModel.AskForPasswordLater = false;

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void SetPasswordChecked_CallsItsPropertyChanged()
        {
            var wasCalled = false;
            _accountViewModel.PropertyChanged += (sender, args) => wasCalled = args.PropertyName == nameof(_accountViewModel.AskForPasswordLater);

            _accountViewModel.AskForPasswordLater = false;

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void AskForPasswordLater_AllowConversionInterruptsEnabled_ValueIsSetInProperty()
        {
            _accountViewModel.AllowConversionInterrupts = true;

            _accountViewModel.AskForPasswordLater = true;
            Assert.IsTrue(_accountViewModel.AskForPasswordLater);

            _accountViewModel.AskForPasswordLater = false;
            Assert.IsFalse(_accountViewModel.AskForPasswordLater);
        }

        [Test]
        public void AskForPasswordLater_AllowConversionInterruptsEnabled_PropertyIsAlwaysFalse()
        {
            _accountViewModel.AllowConversionInterrupts = false;

            _accountViewModel.AskForPasswordLater = true;
            Assert.IsFalse(_accountViewModel.AskForPasswordLater);

            _accountViewModel.AskForPasswordLater = false;
            Assert.IsFalse(_accountViewModel.AskForPasswordLater);
        }

        [Test]
        public void ConstructorSetsExecuteForSaveCommand()
        {
            _accountViewModel.SaveCommand.Execute(null);
            Assert.IsTrue(_accountViewModel.SaveExecuteWasCalled);
        }

        [Test]
        public void ConstructorSetsCanExecuteForSaveCommand()
        {
            var x = _accountViewModel.SaveCommand.IsExecutable;
            Assert.IsTrue(_accountViewModel.SaveCanExecuteWasCalled);
        }
    }
}
