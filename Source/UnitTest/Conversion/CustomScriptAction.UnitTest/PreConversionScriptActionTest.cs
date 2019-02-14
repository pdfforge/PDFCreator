using NSubstitute;
using NUnit.Framework;
using pdfforge.CustomScriptAction;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace CustomScriptAction.UnitTest
{
    [TestFixture]
    public class PreConversionScriptActionTest
    {
        private PreConversionScriptAction _customScriptAction;
        private ICustomScriptHandler _customScriptHandler;
        private ICustomScriptLoader _customScriptLoader;
        private Job _job;
        private ConversionProfile _profile;

        private string _scriptFilename;
        private ActionResult _actionResult;

        [SetUp]
        public void SetUp()
        {
            _scriptFilename = "Script.cs";
            _actionResult = new ActionResult();

            _customScriptHandler = Substitute.For<ICustomScriptHandler>();
            _customScriptLoader = Substitute.For<ICustomScriptLoader>();

            _customScriptAction = new PreConversionScriptAction(_customScriptHandler, _customScriptLoader);

            _profile = new ConversionProfile();
            _job = new Job(new JobInfo(), _profile, new Accounts());
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
        public void ProcessJob_CallsCustomScriptHandlerExecutePostConversion()
        {
            _customScriptHandler.ExecutePreConversion(_job).Returns(_actionResult);

            var result = _customScriptAction.ProcessJob(_job);

            _customScriptHandler.Received(1).ExecutePreConversion(_job);
            Assert.AreSame(_actionResult, result);
        }

        [Test]
        public void Check_ActionIsDisabled_ReturnsValidResult()
        {
            _profile.CustomScript.Enabled = false;

            var result = _customScriptAction.Check(_profile, null, CheckLevel.Profile);

            Assert.IsTrue(result);
        }

        [Test]
        public void Check_ActionIsEnabled_RetunrsResultFromCustomScriptLoaderLoadScriptWithValidation()
        {
            _profile.CustomScript.Enabled = true;
            _profile.CustomScript.ScriptFilename = _scriptFilename;
            _customScriptLoader.LoadScriptWithValidation(_scriptFilename).Returns(new LoadScriptResult(_actionResult, null, ""));

            var result = _customScriptAction.Check(_profile, null, CheckLevel.Profile);

            _customScriptLoader.Received(1).LoadScriptWithValidation(_scriptFilename);
            Assert.AreEqual(_actionResult, result);
        }
    }
}
