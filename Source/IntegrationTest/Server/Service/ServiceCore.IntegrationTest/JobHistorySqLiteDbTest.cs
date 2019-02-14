using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreatorServer.Service.ServiceCore.Helper;
using pdfforge.PDFCreatorServer.Service.ServiceCore.JobHistory;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ServiceCore.IntegrationTest
{
    [TestFixture]
    public class JobHistorySqLiteDbTest
    {
        private IList<JobHistoryFile> _historyFiles;
        private JobHistoryJob _jobHistoryJob;
        private JobHistoryJob _jobOldHistoryJob;
        private JobHistorySqLiteRepository _sqLiteRepository;
        private OsHelper _osHelper;
        private AssemblyHelper _assemblyHelper;
        private IDBPathProvider _dbPathProvider;
        private string TmpTestFolder { get; set; }

        [SetUp]
        public void SetUp()
        {
            _assemblyHelper = new AssemblyHelper(Assembly.GetExecutingAssembly());
            FindAssemblyDirectory();
            CreateTestHistoryJobs();
            InitTempFolder();

            _dbPathProvider = Substitute.For<IDBPathProvider>();
            _dbPathProvider.SqLiteDBFolder.Returns(TmpTestFolder);
            _dbPathProvider.SqLiteDBPath.Returns(Path.Combine(TmpTestFolder, "MonitoringSQLiteDb.sqlite"));
        }

        private void CreateSqLiteDatabaseWithWrongTablename()
        {
            var query = "DROP TABLE IF EXISTS[MonitoringTable]; " +
                        "CREATE TABLE IF NOT EXISTS [WrongTable] " +
                        "(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "'Author' TEXT);";

            ExecuteQuery(query);
            _sqLiteRepository = new JobHistorySqLiteRepository(_assemblyHelper, _osHelper, _dbPathProvider);
        }

        private void CreateSqLiteDatabaseWithWrongSchema()
        {
            var query = "DROP TABLE IF EXISTS[MonitoringTable]; " +
                        "CREATE TABLE IF NOT EXISTS [MonitoringTable] " +
                        "(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "'Author' TEXT, " +
                        "'Date' TEXT);";

            ExecuteQuery(query);
            _sqLiteRepository = new JobHistorySqLiteRepository(_assemblyHelper, _osHelper, _dbPathProvider);
        }

        private void CreateSqLiteDatabase()
        {
            var query = "CREATE TABLE IF NOT EXISTS [MonitoringTable] " +
                        "(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        "'Author' TEXT, " +
                        "'Keywords' TEXT, " +
                        "'PrintJobAuthor' TEXT, " +
                        "'PrintJobName' TEXT, " +
                        "'ComputerName' TEXT, " +
                        "'Producer' TEXT, " +
                        "'Subject' TEXT, " +
                        "'Title' TEXT, " +
                        "'ElapsedTime' NUMERIC, " +
                        "'Format' TEXT, " +
                        "'Pages' INTEGER, " +
                        "'SizeInByte' INTEGER, " +
                        "'Result' TEXT, " +
                        "'Date' TEXT);";

            ExecuteQuery(query);
            _sqLiteRepository = new JobHistorySqLiteRepository(_assemblyHelper, _osHelper, _dbPathProvider);
        }

        private void ExecuteQuery(string query)
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder
            {
                DataSource = _dbPathProvider.SqLiteDBPath,

                JournalMode = SQLiteJournalModeEnum.Wal
            };

            using (var connection = new SQLiteConnection(connectionStringBuilder.ToString()))
            {
                using (var command = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void FindAssemblyDirectory()
        {
            var libPath = _assemblyHelper.GetAssemblyDirectory() + "\\lib\\";
            _osHelper = new OsHelper();
            libPath += _osHelper.Is64BitProcess ? "x64" : "x86";
            _osHelper.AddDllDirectorySearchPath(libPath);
        }

        private void CreateTestHistoryJobs()
        {
            _historyFiles = Substitute.For<IList<JobHistoryFile>>();
            var elapsedTime = TimeSpan.FromTicks(DateTime.Now.Millisecond);
            var metadata = new Metadata();

            _jobHistoryJob = new JobHistoryJob(_historyFiles, OutputFormat.Pdf, DateTime.Now, metadata, "ClientComputer1", 1, elapsedTime, WorkflowResult.Finished);
            _jobOldHistoryJob = new JobHistoryJob(_historyFiles, OutputFormat.Pdf, DateTime.Now.AddDays(-40), metadata, "ClientComputer1", 1, elapsedTime, WorkflowResult.Finished);
        }

        private void InitTempFolder()
        {
            TmpTestFolder = TempFileHelper.CreateTempFolder("PdfCreatorTest\\" + "MonitoringDBTest");
        }

        [Test]
        public void Load_LoadsRecords_NewEntryInDatabase()
        {
            CreateSqLiteDatabase();

            _sqLiteRepository.Save(_jobHistoryJob);
            var test = _sqLiteRepository.Load(10, 1);

            Assert.AreEqual(1, test.Count);
        }

        [Test]
        public void Save_WritesToDatabase_NewEntryInDatabase()
        {
            CreateSqLiteDatabase();
            _sqLiteRepository.Save(_jobHistoryJob);

            var allRecords = _sqLiteRepository.Load(10, 1);
            var dateTime = DateTime.Now.ToString("dd.MM.yyyy");
            var creationDate = allRecords.Last().CreationTime.ToString("dd.MM.yyyy");

            Assert.AreEqual(creationDate, dateTime);
            Assert.AreEqual(1, allRecords.Count);
        }

        [Test]
        public void Search_SearchInDatabase_ReturnsOneRecord()
        {
            CreateSqLiteDatabase();
            _jobHistoryJob.Metadata.Title = "MehrSeitenDruck";
            _sqLiteRepository.Save(_jobHistoryJob);
            var allRecords = _sqLiteRepository.Search("Title", "MehrSeitenDruck", 10);

            Assert.AreEqual(1, allRecords.Count);
        }

        [Test]
        public void SearchFromToDate_SearchFromToInDatabase_ReturnsOneRecord()
        {
            CreateSqLiteDatabase();
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime toDate = DateTime.Now;

            _sqLiteRepository.Save(_jobHistoryJob);
            var allRecords = _sqLiteRepository.SearchDateFromTo(fromDate, toDate, 10);

            Assert.AreEqual(1, allRecords.Count);
        }

        [Test]
        public void RemoveOldEntries_RemovesOldRecordsFromDatabase_ReturnsCleanedDatabase()
        {
            CreateSqLiteDatabase();
            TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0); // days, hours, minutes, seconds
            _sqLiteRepository.Save(_jobOldHistoryJob); // write old job into database
            _sqLiteRepository.Save(_jobHistoryJob); // write new entry to database
            _sqLiteRepository.RemoveOldEntries(timeSpan); // remove all entries older than one day and the last one

            var allRecords = _sqLiteRepository.Load(10, 1);

            Assert.AreEqual(1, allRecords.Count);
        }

        [Test]
        public void DeleteHistory_DeletesAllEntries_NoEntryInDatabase()
        {
            CreateSqLiteDatabase();
            _sqLiteRepository.ClearHistory();

            var test = _sqLiteRepository.Load(10, 1);

            Assert.AreEqual(0, test.Count);
        }

        [Test]
        public void DatabaseHasWrongSchema_DropTableandCreateNew_SaveToDbIsPossible()
        {
            CreateSqLiteDatabaseWithWrongSchema();

            _sqLiteRepository.Save(_jobHistoryJob);
            var test = _sqLiteRepository.Load(10, 1);

            Assert.AreEqual(1, test.Count);
        }

        [Test]
        public void DatabaseHasWrongTablename_CreateNewMonitoringTable_SaveToDbIsPossible()
        {
            CreateSqLiteDatabaseWithWrongTablename();

            _sqLiteRepository.Save(_jobHistoryJob);
            var test = _sqLiteRepository.Load(10, 1);

            Assert.AreEqual(1, test.Count);
        }

        [TearDown]
        public void TeadDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            TempFileHelper.CleanUp();
        }
    }
}
