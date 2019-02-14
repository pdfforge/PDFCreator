using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.Workflow.Steps
{
    [TestFixture]
    public class InteractiveProfileCheckerTest
    {
        private InteractiveProfileChecker _interactiveProfileChecker;
        private IProfileChecker _profileChecker;
        private UnitTestInteractionRequest _interactionRequest;
        private IInteractionInvoker _interactionInvoker;
        private readonly InteractiveProfileCheckerTranslation _translation = new InteractiveProfileCheckerTranslation();
        private Job _job;
        private const string ProfileName = "Profile Name";

        [SetUp]
        public void SetUp()
        {
            _profileChecker = Substitute.For<IProfileChecker>();
            var translationFactory = new TranslationFactory();
            _interactionRequest = new UnitTestInteractionRequest();
            _interactionInvoker = Substitute.For<IInteractionInvoker>();

            _interactiveProfileChecker = new InteractiveProfileChecker(_profileChecker, _interactionRequest, _interactionInvoker, translationFactory);

            _job = new Job(null, new ConversionProfile { Name = ProfileName }, null);
        }

        private void ValidateMessageInteraction(MessageInteraction interaction, ActionResult faultyResult)
        {
            Assert.AreEqual(_translation.Error, interaction.Title, "Interaction Title");
            Assert.AreEqual(_translation.InvalidSettings, interaction.Text, "Interaction Text");
            Assert.AreEqual(ProfileName, interaction.ActionResultDict.Keys.First(), "Interaction ActionResultDict Key");
            Assert.AreEqual(faultyResult, interaction.ActionResultDict[ProfileName], "Interaction ActionResultDict Result");
            Assert.AreEqual(MessageOptions.OK, interaction.Buttons, "interaction buttons");
            Assert.AreEqual(MessageIcon.Exclamation, interaction.Icon, "interaction icon");
            Assert.IsFalse(interaction.ShowErrorRegions, "interaction ShowErrorRegions");
        }

        [Test]
        public void CheckWithErrorResultInOverlay_InvalidProfile_ReturnFalse_NotifyUserInOverlay()
        {
            var faultyResult = new ActionResult(ErrorCode.Ftp_NoAccount); //Random error code, to make the profile invalid
            _profileChecker.CheckJob(_job).Returns(faultyResult);

            var result = _interactiveProfileChecker.CheckWithErrorResultInOverlay(_job);

            Assert.IsFalse(result, "Return value");
            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            ValidateMessageInteraction(interaction, faultyResult);
            _interactionInvoker.DidNotReceive().Invoke(Arg.Any<MessageInteraction>()); //InteractionInvoker must not be called!
        }

        [Test]
        public void CheckWithErrorResultInOverlay_ValidProfile_ReturnTrue_DoNotRaiseMessageInteraction()
        {
            _profileChecker.CheckJob(_job).Returns(new ActionResult()); //valid profile

            var result = _interactiveProfileChecker.CheckWithErrorResultInOverlay(_job);

            Assert.IsTrue(result, "Return value");
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void CheckWithErrorResultInWindow_InvalidProfile_ReturnsFalse_NotifyUserInWindow()
        {
            var faultyResult = new ActionResult(ErrorCode.Ftp_NoAccount); //Random error code, to make the profile invalid
            _profileChecker.CheckJob(_job).Returns(faultyResult);
            MessageInteraction interaction = null;
            _interactionInvoker.Invoke(Arg.Do<MessageInteraction>(i => interaction = i));

            var result = _interactiveProfileChecker.CheckWithErrorResultInWindow(_job);

            Assert.IsFalse(result, "Return value");
            ValidateMessageInteraction(interaction, faultyResult);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>(); //InteractionRequest must not be called!
        }

        [Test]
        public void CheckWithErrorResultInWindow_ValidProfile_ReturnsTrue_DoNotRaiseMessageInteraction()
        {
            _profileChecker.CheckJob(_job).Returns(new ActionResult()); //valid profile

            var result = _interactiveProfileChecker.CheckWithErrorResultInWindow(_job);

            Assert.IsTrue(result, "Return value");
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }
    }
}
