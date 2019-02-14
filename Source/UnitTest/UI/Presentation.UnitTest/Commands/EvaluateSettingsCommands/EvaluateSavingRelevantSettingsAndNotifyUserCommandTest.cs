using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;

namespace Presentation.UnitTest.Commands.EvaluateSettingsCommands
{
    [TestFixture]
    public class EvaluateSavingRelevantSettingsAndNotifyUserCommandTest
    {
        private EvaluateSavingRelevantSettingsAndNotifyUserCommand _evaluateSavingCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private WaitableCommandTester<EvaluateSavingRelevantSettingsAndNotifyUserCommand> _commandTester;
        private ITabSwitchSettingsCheck _tabSwitchSettingsCheck;
        private readonly EvaluateSettingsAndNotifyUserTranslation _translation = new EvaluateSettingsAndNotifyUserTranslation();

        private readonly SettingsCheckResult _resultNoChangesNoErrors = new SettingsCheckResult(new ActionResultDict(), false);
        private readonly SettingsCheckResult _resultWithChangesNoErrors = new SettingsCheckResult(new ActionResultDict(), true);
        private static readonly ActionResultDict InvalidActionResultDict = new ActionResultDict { { "Key", new ActionResult(0) } };
        private readonly SettingsCheckResult _resultNoChangesWithErrors = new SettingsCheckResult(InvalidActionResultDict, false);
        private readonly SettingsCheckResult _resultWithChangesWithErrors = new SettingsCheckResult(InvalidActionResultDict, true);

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _tabSwitchSettingsCheck = Substitute.For<ITabSwitchSettingsCheck>();

            _evaluateSavingCommand = new EvaluateSavingRelevantSettingsAndNotifyUserCommand(
                _interactionRequest, new DesignTimeTranslationUpdater(), _tabSwitchSettingsCheck);

            _commandTester = new WaitableCommandTester<EvaluateSavingRelevantSettingsAndNotifyUserCommand>(_evaluateSavingCommand);
        }

        [Test]
        public void CanExecute_IsAlwaysTrue()
        {
            Assert.IsTrue(_evaluateSavingCommand.CanExecute(null));
        }

        [Test]
        public void Execute_SettingsCheckResultNoChangesNoErrors_NoUserInteraction_IsDoneCalledWithSuccess()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultNoChangesNoErrors);

            _evaluateSavingCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void Execute_SettingsCheckResultWithChangesNoErrors_NoUserInteraction_IsDoneCalledWithSuccess()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesNoErrors);

            _evaluateSavingCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void Execute_SettingsCheckResultNoChangesWithErrors_CorrectUserInteraction()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultNoChangesWithErrors);

            _evaluateSavingCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.Settings, interaction.Title, "Interaction Title");
            Assert.AreEqual(_translation.InvalidSettings, interaction.Text, "Interaction Text");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Interaction Icon");
            Assert.AreEqual(InvalidActionResultDict, interaction.ActionResultDict, "Interaction ActionResultDict");
            Assert.AreEqual(_translation.WantToProceedAnyway, interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void Execute_SettingsCheckResultWithChangesWithErrors_CorrectUserInteraction()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);

            _evaluateSavingCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.Settings, interaction.Title, "Interaction Title");
            Assert.AreEqual(_translation.InvalidSettingsWithUnsavedChanges, interaction.Text, "Interaction Text");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Interaction Icon");
            Assert.AreEqual(InvalidActionResultDict, interaction.ActionResultDict, "Interaction ActionResultDict");
            Assert.AreEqual(_translation.WantToSaveAnyway, interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void Execute_VerifyUserResponse_Yes_CallsIsDoneWithSuccess()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Yes;
            });

            _evaluateSavingCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
        }

        [Test]
        public void Execute_VerifyUserResponse_No_CallsIsDoneWithCancel()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.No;
            });

            _evaluateSavingCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Cancel, _commandTester.LastResponseStatus);
        }

        [Test]
        public void Execute_VerifyUserResponse_Cancel_CallsIsDoneWithCancel()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Cancel;
            });

            _evaluateSavingCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Cancel, _commandTester.LastResponseStatus);
        }
    }
}
