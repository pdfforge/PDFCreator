using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System;

namespace Presentation.UnitTest.Commands.EvaluateSettingsCommands
{
    [TestFixture]
    public class EvaluateTabSwitchRelevantSettingsAndNotifyUserCommandTest
    {
        private EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand _beforeEvaluateTabSwitchCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ITabSwitchSettingsCheck _tabSwitchSettingsCheck;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private WaitableCommandTester<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand> _commandTester;

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
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();

            _beforeEvaluateTabSwitchCommand = new EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand(
                _interactionRequest, new DesignTimeTranslationUpdater(), _tabSwitchSettingsCheck, _currentSettingsProvider);

            _commandTester = new WaitableCommandTester<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>(_beforeEvaluateTabSwitchCommand);
        }

        [Test]
        public void CanExecute_IsAlwaysTrue()
        {
            Assert.IsTrue(_beforeEvaluateTabSwitchCommand.CanExecute(null));
        }

        [Test]
        public void Execute_SettingsCheckResultNoChangesNoErrors_NoUserInteraction_IsDoneCalledWithSuccess()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultNoChangesNoErrors);

            _beforeEvaluateTabSwitchCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void Execute_SettingsCheckResultWithChangesNoErrors_NoUserInteraction_IsDoneCalledWithSuccess()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesNoErrors);

            _beforeEvaluateTabSwitchCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.Settings, interaction.Title, "Interaction Title");
            Assert.AreEqual(_translation.UnsavedChanges
                            + Environment.NewLine
                            + _translation.WantToSave
                            + Environment.NewLine
                            + _translation.ChooseNoToRevert, interaction.Text, "Interaction Text");
            Assert.AreEqual(MessageOptions.YesNoCancel, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Question, interaction.Icon, "Interaction Icon");
        }

        [Test]
        public void Execute_SettingsCheckResultNoChangesWithErrors_NoUserInteraction_IsDoneCalledWithSuccess()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultNoChangesWithErrors);

            _beforeEvaluateTabSwitchCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void Execute_SettingsCheckResultWithChangesWithErrors_CorrectUserInteraction()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);

            _beforeEvaluateTabSwitchCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.Settings, interaction.Title, "Interaction Title");
            Assert.AreEqual(_translation.InvalidSettingsWithUnsavedChanges, interaction.Text, "Interaction Text");
            Assert.AreEqual(MessageOptions.YesNoCancel, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Interaction Icon");
            Assert.AreEqual(InvalidActionResultDict, interaction.ActionResultDict, "Interaction ActionResultDict");
            Assert.AreEqual(_translation.WantToSaveAnyway + Environment.NewLine + _translation.ChooseNoToRevert, interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void Execute_VerifyUserResponse_Yes_CallsIsDoneWithSuccess_CurrentSettingsDoesNotCallReset()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Yes;
            });

            _beforeEvaluateTabSwitchCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _currentSettingsProvider.DidNotReceive().Reset();
        }

        [Test]
        public void Execute_VerifyUserResponse_No_CallsIsDoneWithSkip_CurrentSettingsCallsReset()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.No;
            });

            _beforeEvaluateTabSwitchCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Skip, _commandTester.LastResponseStatus);
            _currentSettingsProvider.Received(1).Reset();
        }

        [Test]
        public void Execute_VerifyUserResponse_Cancel_CallsIsDoneWithCancel_CurrentSettingsDoesNotCallReset()
        {
            _tabSwitchSettingsCheck.CheckAffectedSettings().Returns(_resultWithChangesWithErrors);
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Cancel;
            });

            _beforeEvaluateTabSwitchCommand.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Cancel, _commandTester.LastResponseStatus);
            _currentSettingsProvider.DidNotReceive().Reset();
        }
    }
}
