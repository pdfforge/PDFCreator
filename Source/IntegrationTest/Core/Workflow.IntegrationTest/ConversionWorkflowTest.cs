using iTextSharp.text.pdf;
using NSubstitute;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.IntegrationTest.Core.Workflow
{
    [TestFixture]
    public class ConversionWorkflowTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            container.Options.AllowOverridingRegistrations = true;

            _workflowFactory = container.GetInstance<IWorkflowFactory>();
            _jobBuilder = container.GetInstance<IJobBuilder>();

            _interactiveProfile = new ConversionProfile();
            _interactiveProfile.Name = InteractivePrinterName;
            _interactiveProfile.Guid = InteractiveProfileGuid;
            _interactiveProfile.AutoSave.Enabled = false;
            _interactiveProfileMapping = new PrinterMapping(InteractivePrinterName, InteractiveProfileGuid);

            _autosaveProfile = new ConversionProfile();
            _autosaveProfile.Name = AutosaveProfileName;
            _autosaveProfile.Guid = AutosaveProfileGuid;
            _autosaveProfile.AutoSave.Enabled = true;
            _autosaveProfileMapping = new PrinterMapping(AutosavePrinterName, AutosaveProfileGuid);

            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("ConversionWorklowTest");
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Pdf);
            _settings = new PdfCreatorSettings();
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;
        private PdfCreatorSettings _settings;
        private ConversionProfile _interactiveProfile;
        private PrinterMapping _interactiveProfileMapping;
        private const string InteractivePrinterName = "InteractivePrinterName";
        private const string InteractiveProfileName = "InteractiveProfileName";
        private const string InteractiveProfileGuid = "InteractiveProfileGuid";

        private ConversionProfile _autosaveProfile;
        private PrinterMapping _autosaveProfileMapping;
        private IWorkflowFactory _workflowFactory;
        private IJobBuilder _jobBuilder;

        private const string AutosavePrinterName = "AutosavePrinterName";
        private const string AutosaveProfileName = "AutosaveProfileName";
        private const string AutosaveProfileGuid = "AutosaveProfileGuid";

        [Test]
        public void AuthorTemplateTest()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.JobInfo.Metadata.PrintJobAuthor = "Author from Job";

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.Enabled = true;
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;
            _settings.ConversionProfiles[0].AuthorTemplate = "<PrintJobAuthor> + some text";

            var job = _jobBuilder.BuildJobFromJobInfo(_th.JobInfo, _settings);
            var autoSaveWorkflow = _workflowFactory.CreateWorkflow(WorkflowModeEnum.Autosave);

            autoSaveWorkflow.RunWorkflow(job);

            using (var pdf = new PdfReader(job.OutputFiles[0]))
            {
                Assert.AreEqual("Author from Job + some text", pdf.Info["Author"], "Wrong author in PDF Metadata");
            }
        }

        [Test]
        public void CreateConversionWorkflowTest_NoMapping_TwoProfiles_NoneIsDefault_NoneIsLastUsedProfile_ReturnWorkflowWithFirstProfile()
        {
            _settings.CreatorAppSettings.LastUsedProfileGuid = "NoneOfTheProfileGuids";

            _settings.ConversionProfiles.Add(_autosaveProfile);
            _settings.ConversionProfiles.Add(_interactiveProfile);
            var job = _jobBuilder.BuildJobFromJobInfo(_th.JobInfo, _settings);
            Assert.AreEqual(_autosaveProfile, job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_NoMapping_TwoProfiles_OneIsDefault_NoneIsLastUsedProfile_ReturnWorkflowWithDefaultProfile()
        {
            _settings.CreatorAppSettings.LastUsedProfileGuid = "NoneOfTheProfileGuids";

            _settings.ConversionProfiles.Add(_autosaveProfile); //For this test the default profile must not be the first in list!
            _interactiveProfile.Guid = "DefaultGuid";
            _settings.ConversionProfiles.Add(_interactiveProfile);
            var job = _jobBuilder.BuildJobFromJobInfo(_th.JobInfo, _settings);
            Assert.AreEqual(_interactiveProfile, job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_PrinterIsNotListedInMapping_ReturnWorkflowWithDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(_autosaveProfileMapping);
            _settings.ApplicationSettings.PrinterMappings.Add(_interactiveProfileMapping);
            _settings.ConversionProfiles.Add(_autosaveProfile); //For this test the default profile must not be the first in list!
            _interactiveProfile.Guid = "DefaultGuid";
            _settings.ConversionProfiles.Add(_interactiveProfile);

            _th.JobInfo.SourceFiles[0].PrinterName = "PrinterNameNotInMapping";

            var job = _jobBuilder.BuildJobFromJobInfo(_th.JobInfo, _settings);
            Assert.AreEqual(_interactiveProfile, job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_PrinterMappingToInvalidProfile_ReturnWorkflowWithDefaultProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(_autosaveProfileMapping);
            _settings.ApplicationSettings.PrinterMappings.Add(_interactiveProfileMapping);
            const string somePrinterName = "somePrinterName";
            const string someProfileGuid = "someProfileNotInProfilesListGuid";
            var somePrinterMapping = new PrinterMapping(somePrinterName, someProfileGuid);
            _settings.ApplicationSettings.PrinterMappings.Add(somePrinterMapping);
            _settings.ConversionProfiles.Add(_autosaveProfile); //For this test the default profile must not be the first in list!
            _interactiveProfile.Guid = "DefaultGuid";
            _settings.ConversionProfiles.Add(_interactiveProfile);

            _th.JobInfo.SourceFiles[0].PrinterName = somePrinterName;

            var job = _jobBuilder.BuildJobFromJobInfo(_th.JobInfo, _settings);
            Assert.AreEqual(_interactiveProfile, job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void CreateConversionWorkflowTest_PrinterMappingToProfile_ReturnWorkflowWithMappedProfile()
        {
            _settings.ApplicationSettings.PrinterMappings.Add(_autosaveProfileMapping);
            _settings.ApplicationSettings.PrinterMappings.Add(_interactiveProfileMapping);
            _settings.ConversionProfiles.Add(_autosaveProfile);
            _settings.ConversionProfiles.Add(_interactiveProfile);

            _th.JobInfo.SourceFiles[0].PrinterName = _autosaveProfileMapping.PrinterName;
            var job = _jobBuilder.BuildJobFromJobInfo(_th.JobInfo, _settings);
            Assert.AreEqual(_autosaveProfile, job.Profile, "Wrong Profile in Job of workflow.");
        }

        [Test]
        public void TitleTemplateTest()
        {
            _th.GenerateGsJob(PSfiles.PDFCreatorTestpage, OutputFormat.Pdf);
            _th.JobInfo.Metadata.PrintJobName = "Title from Job";
            _th.JobInfo.Metadata.PrintJobAuthor = "Author from Job";

            _settings = new PdfCreatorSettings();
            _settings.ConversionProfiles.Add(_th.Job.Profile);
            _settings.ConversionProfiles[0].AutoSave.Enabled = true;
            _settings.ConversionProfiles[0].AutoSave.EnsureUniqueFilenames = false;
            _settings.ConversionProfiles[0].TargetDirectory = _th.TmpTestFolder;
            _settings.ConversionProfiles[0].FileNameTemplate = "AutoSaveTestOutput";
            _settings.ConversionProfiles[0].OutputFormat = OutputFormat.Pdf;
            _settings.ConversionProfiles[0].TitleTemplate = "<PrintJobName> by <PrintJobAuthor>";

            var profileChecker = Substitute.For<IProfileChecker>();
            profileChecker.CheckJob(Arg.Any<Job>()).Returns(new ActionResult());

            var job = _jobBuilder.BuildJobFromJobInfo(_th.JobInfo, _settings);
            var autoSaveWorkflow = _workflowFactory.CreateWorkflow(WorkflowModeEnum.Autosave);

            autoSaveWorkflow.RunWorkflow(job);

            using (var pdf = new PdfReader(job.OutputFiles[0]))
            {
                Assert.AreEqual("Title from Job by Author from Job", pdf.Info["Title"], "Wrong title in PDF Metadata");
            }
        }
    }
}
