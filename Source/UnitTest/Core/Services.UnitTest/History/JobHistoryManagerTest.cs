using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.Services.UnitTest.History
{
    [TestFixture]
    public class JobHistoryManagerTest
    {
        private IJobHistoryManager _jobHistoryManager;
        private IJobHistoryStorage _jobHistoryStorage;
        private ITempFolderProvider _tempFolderProvider;
        private IFile _file;
        private IHashUtil _hashUtil;
        private Job _job;
        private IList<HistoricJob> _storedHistoryFullCapacity;
        private PdfCreatorSettings _settings;
        private int _capacity = 10;
        private const string TempFolder = "TempFolder";

        [SetUp]
        public void SetUp()
        {
            _job = new Job(new JobInfo(), new ConversionProfile(), new Accounts());
            _job.OutputFileTemplate = "MayNotStartWithTempfolder.pdf";
            _job.OutputFiles.Add("NotEmpty.pdf");
            _storedHistoryFullCapacity = new List<HistoricJob>();

            _jobHistoryStorage = Substitute.For<IJobHistoryStorage>();
            _storedHistoryFullCapacity = new List<HistoricJob>();
            var hjPdf = new HistoricFile(@"X:\Foldername\PeeDeeEff.pdf", "PeeDeeEff.pdf", @"X:\Foldername\", "ABC123");
            var historicFiles = new List<HistoricFile> { hjPdf };
            for (int i = 0; i < _capacity; i++)
                _storedHistoryFullCapacity.Add(new HistoricJob(historicFiles, OutputFormat.Pdf, DateTime.Now, new Metadata(), 23, false));
            _jobHistoryStorage.Load().Returns(_storedHistoryFullCapacity);

            var settingsProvider = Substitute.For<ISettingsProvider>();
            _settings = new PdfCreatorSettings();
            _settings.ApplicationSettings.JobHistory.Enabled = true;
            _settings.ApplicationSettings.JobHistory.Capacity = _capacity;
            settingsProvider.Settings.Returns(_settings);

            _tempFolderProvider = Substitute.For<ITempFolderProvider>();
            _tempFolderProvider.TempFolder.Returns(TempFolder);

            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);

            _hashUtil = Substitute.For<IHashUtil>();

            _jobHistoryManager = new JobHistoryManager(settingsProvider, _jobHistoryStorage, _tempFolderProvider, _file, _hashUtil);
        }

        [Test]
        public void Init_HistoryIsEmptyList()
        {
            Assert.IsNotNull(_jobHistoryManager.History);
            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Save_CallsJobHistoryStorageSave()
        {
            _jobHistoryManager.Save();

            _jobHistoryStorage.Received(1).Save(_jobHistoryManager.History);
        }

        [Test]
        public void Clear_HistoryGetsEmptyed_CallsJobHistoryStorageSaveWithEmptyHistory_CallsHistoryChanged()
        {
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Clear();

            Assert.IsEmpty(_jobHistoryManager.History);
            _jobHistoryStorage.Received().Save(Arg.Is<IList<HistoricJob>>(x => x.Count == 0));
            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public async Task Load_CapacityFitsSavedHistory_HistoryInHistoryManagerWasLoadedFromHistoryStorage()
        {
            _settings.ApplicationSettings.JobHistory.Capacity = _storedHistoryFullCapacity.Count;

            await _jobHistoryManager.Load();

            Assert.AreEqual(_storedHistoryFullCapacity, _jobHistoryManager.History);
        }

        [Test]
        public async Task Load_CapacityIsToSmallForSavedHistory_HistoryIsReducedToCapacity()
        {
            //reduce capacity
            _settings.ApplicationSettings.JobHistory.Capacity = _storedHistoryFullCapacity.Count - 2;

            await _jobHistoryManager.Load();
            var loadedHistory = _jobHistoryManager.History;

            //remove first files to check if the lastest files remain
            _storedHistoryFullCapacity.Remove(_storedHistoryFullCapacity.First());
            _storedHistoryFullCapacity.Remove(_storedHistoryFullCapacity.First());
            Assert.AreEqual(_storedHistoryFullCapacity, loadedHistory);
        }

        [Test]
        public async Task Load_RemovesDeletedJobsFromHistory()
        {
            var deletedFilePath = "deletedFilePath";
            _file.Exists(deletedFilePath).Returns(false);
            _storedHistoryFullCapacity.First().HistoricFiles.First().Path = deletedFilePath;
            var deletedHistoricJob1 = _storedHistoryFullCapacity.First();
            _storedHistoryFullCapacity.Last().HistoricFiles.First().Path = deletedFilePath;
            var deletedHistoricJob2 = _storedHistoryFullCapacity.First();

            await _jobHistoryManager.Load();

            Assert.IsFalse(_jobHistoryManager.History.Contains(deletedHistoricJob1), "Did not remove deleted file from history");
            Assert.IsFalse(_jobHistoryManager.History.Contains(deletedHistoricJob2), "Did not remove deleted file from history");
        }

        [Test]
        public async Task Load_CallsHistoryChanged()
        {
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            await _jobHistoryManager.Load();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public async Task Refresh_RemovesDeletedJobsFromHistory()
        {
            var deletedFilePath = "deletedFilePath";
            _file.Exists(deletedFilePath).Returns(false);
            _storedHistoryFullCapacity.First().HistoricFiles.First().Path = deletedFilePath;
            var deletedHistoricJob1 = _storedHistoryFullCapacity.First();
            _storedHistoryFullCapacity.Last().HistoricFiles.First().Path = deletedFilePath;
            var deletedHistoricJob2 = _storedHistoryFullCapacity.First();
            _jobHistoryManager.History = _storedHistoryFullCapacity;

            await _jobHistoryManager.Refresh();

            Assert.IsFalse(_jobHistoryManager.History.Contains(deletedHistoricJob1), "Did not remove deleted file from history");
            Assert.IsFalse(_jobHistoryManager.History.Contains(deletedHistoricJob2), "Did not remove deleted file from history");
        }

        [Test]
        public async Task Refresh_CallsHistoryChanged()
        {
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            await _jobHistoryManager.Refresh();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Remove_RemovesGivenJobFromHistory()
        {
            _jobHistoryManager.History = _storedHistoryFullCapacity;
            var removedjob = _storedHistoryFullCapacity.First();

            _jobHistoryManager.Remove(removedjob);

            Assert.IsFalse(_jobHistoryManager.History.Contains(removedjob), "Did not remove job from history");
        }

        [Test]
        public void Remove_CallsHistoryChanged()
        {
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Remove(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Add_HistoryDisabled_HistoryRemainsEmpty()
        {
            _settings.ApplicationSettings.JobHistory.Enabled = false;

            _jobHistoryManager.Add(_job);

            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Add_JobIsTemporary_HistoryRemainsEmpty()
        {
            _job.OutputFileTemplate = Path.Combine(TempFolder, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()), "file.pdf");

            _jobHistoryManager.Add(_job);

            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Add_JobHasNoOutpufiles_HistoryRemainsEmpty()
        {
            _job.OutputFiles.Clear();

            _jobHistoryManager.Add(_job);

            Assert.IsEmpty(_jobHistoryManager.History);
        }

        [Test]
        public void Add_JobIsAddable_HistoryContainsJob_CallsHistoryChanged()
        {
            _job.OutputFiles.Clear();
            var filepathToIdentifyJob = "NameToIdentifyJob.pdf";
            _job.OutputFiles.Add(filepathToIdentifyJob);
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Add(_job);

            Assert.AreEqual(1, _jobHistoryManager.History.Count, "1 Job should be added.");
            Assert.AreEqual(filepathToIdentifyJob, _jobHistoryManager.History.First().HistoricFiles.First().FileName, "New job is not first in history");
            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void Add_JobIsAddableHistoryHasFullCapacity_HistoryContainsJobCapacityIsNotExceeded_CallsHistoryChanged()
        {
            _jobHistoryManager.Load();//Adds jobs to history

            _job.OutputFiles.Clear();
            var filepathToIdentifyJob = "NameToIdentifyJob.pdf";
            _job.OutputFiles.Add(filepathToIdentifyJob);
            var wasCalled = false;
            _jobHistoryManager.HistoryChanged += (sender, args) => wasCalled = true;

            _jobHistoryManager.Add(_job);

            Assert.AreEqual(filepathToIdentifyJob, _jobHistoryManager.History.First().HistoricFiles.First().FileName, "New job is not first in history");
            Assert.AreEqual(_capacity, _jobHistoryManager.History.Count, "Capacity was exceeded");
            Assert.IsTrue(wasCalled, "Did not call HistoryChanged");
        }

        [Test]
        public void TransformToHistoricJob_DataGetsTransferedCorrectly()
        {
            var expectedFormat = OutputFormat.PdfX;
            _job.Profile.OutputFormat = expectedFormat;
            var expectedCreationTime = DateTime.Now;
            _job.JobInfo.PrintDateTime = expectedCreationTime;
            var expectedMetaData = new Metadata();
            expectedMetaData.Author = "Some Author";
            expectedMetaData.Title = "Some Title";
            expectedMetaData.Subject = "Some Subject";
            expectedMetaData.Keywords = "Some Keywords";
            _job.JobInfo.Metadata = expectedMetaData;

            var historicJob = _jobHistoryManager.TransformToHistoricJob(_job);

            Assert.AreEqual(expectedFormat, historicJob.Format, "Format");
            Assert.AreEqual(expectedCreationTime, historicJob.CreationTime, "CreationTime");
            Assert.AreEqual(expectedMetaData, historicJob.Metadata, "Metadata");
        }

        [Test]
        public void TransformToHistoricJob_SingleOutputFile_HistoricJobContainsFileInHistoricFilesWithHash()
        {
            var expectedFile = "ExpectedFile.pdf";
            _job.OutputFiles = new List<string> { expectedFile };
            var expectedHash = "ExpectedHash";
            _hashUtil.CalculateFileMd5(expectedFile).Returns(expectedHash);

            var historicJob = _jobHistoryManager.TransformToHistoricJob(_job);

            Assert.AreEqual(1, historicJob.HistoricFiles.Count, "Wrong number of HistoricFiles");
            Assert.AreEqual(expectedFile, historicJob.HistoricFiles.First().Path);
            Assert.AreEqual(expectedHash, historicJob.HistoricFiles.First().Hash);
        }

        [Test]
        public void TransformToHistoricJob_MultipleOutputFiles_HistoricJobContainsFilesInHistoricFilesWithHash()
        {
            var expectedFile1 = "ExpectedFile.pdf";
            var expectedFile2 = "ExpectedFile2.pdf";
            var expectedFile3 = "ExpectedFile3.pdf";
            _job.OutputFiles = _job.OutputFiles = new List<string> { expectedFile1, expectedFile2, expectedFile3 };
            var expectedHash1 = "ExpectedHash1";
            _hashUtil.CalculateFileMd5(expectedFile1).Returns(expectedHash1);
            var expectedHash2 = "ExpectedHash2";
            _hashUtil.CalculateFileMd5(expectedFile2).Returns(expectedHash2);
            var expectedHash3 = "ExpectedHash3";
            _hashUtil.CalculateFileMd5(expectedFile3).Returns(expectedHash3);

            var historicJob = _jobHistoryManager.TransformToHistoricJob(_job);

            Assert.AreEqual(3, historicJob.HistoricFiles.Count, "Wrong number of HistoricFiles");
            Assert.AreEqual(expectedFile1, historicJob.HistoricFiles.First().Path);
            Assert.AreEqual(expectedFile2, historicJob.HistoricFiles[1].Path);
            Assert.AreEqual(expectedFile3, historicJob.HistoricFiles.Last().Path);
            Assert.AreEqual(expectedHash1, historicJob.HistoricFiles.First().Hash);
            Assert.AreEqual(expectedHash2, historicJob.HistoricFiles[1].Hash);
            Assert.AreEqual(expectedHash3, historicJob.HistoricFiles.Last().Hash);
        }
    }
}
