using NSubstitute;
using NUnit.Framework;
using pdfforge.CustomScriptAction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace CustomScriptAction.UnitTest
{
    [TestFixture]
    public class PostConversionScriptActionTest
    {
        private PostConversionScriptAction _customScriptAction;
        private ICustomScriptHandler _customScriptHandler;
        private Job _job;
        private ConversionProfile _profile;
        private ActionResult _actionResult;

        [SetUp]
        public void SetUp()
        {
            _customScriptHandler = Substitute.For<ICustomScriptHandler>();
            _customScriptAction = new PostConversionScriptAction(_customScriptHandler);

            _profile = new ConversionProfile();
            _job = new Job(new JobInfo(), _profile, new Accounts());
            _actionResult = new ActionResult();
        }

        [Test]
        public void Enabled_ReturnsCustomScriptActionEnabledFromProfile()
        {
            _profile.CustomScript.Enabled = true;
            Assert.IsTrue(_customScriptAction.IsEnabled(_profile));

            _profile.CustomScript.Enabled = false;
            Assert.IsFalse(_customScriptAction.IsEnabled(_profile));
        }

        [Test]
        public void ProcessJob_CallsCustomScriptHandlerExecutePreConversion()
        {
            _customScriptHandler.ExecutePostConversion(_job).Returns(_actionResult);

            var result = _customScriptAction.ProcessJob(_job);

            _customScriptHandler.Received(1).ExecutePostConversion(_job);
            Assert.AreSame(_actionResult, result);
        }
    }
}
