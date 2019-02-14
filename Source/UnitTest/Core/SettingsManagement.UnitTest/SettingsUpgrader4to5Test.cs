using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal class SettingsUpgrader4To5Test
    {
        [Test]
        public void DataWithVersion4_UpgradeRequiredToVersion4_ReturnsTrue()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "4");

            var upgrader = new CreatorSettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(5));
        }

        [Test]
        public void DataWithVersion4_UpgradeToVersion5_SetsVersionTo5()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(@"ApplicationProperties\SettingsVersion", "4");
            var upgrader = new CreatorSettingsUpgrader(data);

            upgrader.Upgrade(5);

            Assert.AreEqual("5", data.GetValue(@"ApplicationProperties\SettingsVersion"));
        }

        [SetUp]
        public void SetUp()
        {
            _dataV3 = Data.CreateDataStorage();
            _replacementCount = 0;

            _dataV3.SetValue(@"ApplicationProperties\SettingsVersion", "4");
        }

        private Data _dataV3;
        private int _replacementCount;

        private void AddOldReplacement(string search, string replace)
        {
            string section = $"ApplicationSettings\\TitleReplacement\\{_replacementCount}\\";
            _dataV3.SetValue($"{section}Search", search);
            _dataV3.SetValue($"{section}Replace", replace);
            _replacementCount++;
            _dataV3.SetValue(@"ApplicationSettings\TitleReplacement\numClasses", _replacementCount.ToString());
        }

        [TestCase("Microsoft Word - ")]
        [TestCase("Microsoft PowerPoint - ")]
        [TestCase("Microsoft Excel - ")]
        public void OldStartReplacements_AreConvertedToStartType(string expectedStartType)
        {
            AddOldReplacement(expectedStartType, "");
            var upgrader = new CreatorSettingsUpgrader(_dataV3.Clone());

            upgrader.Upgrade(5);
            var data = upgrader.Data;

            Assert.AreEqual("Start", data.GetValue("ApplicationSettings\\TitleReplacement\\0\\ReplacementType"));
        }

        [TestCase(".doc")]
        [TestCase(".docx")]
        [TestCase(".xls")]
        [TestCase(".xlsx")]
        [TestCase(".ppt")]
        [TestCase(".pptx")]
        [TestCase(".png")]
        [TestCase(".jpg")]
        [TestCase(".jpeg")]
        [TestCase(".txt - Editor")]
        [TestCase(" - Editor")]
        [TestCase(".txt")]
        [TestCase(".tif")]
        [TestCase(".tiff")]
        public void OldEndReplacements_AreConvertedToEndType(string expectedEndType)
        {
            AddOldReplacement(expectedEndType, "");
            var upgrader = new CreatorSettingsUpgrader(_dataV3.Clone());

            upgrader.Upgrade(5);
            var data = upgrader.Data;

            Assert.AreEqual("End", data.GetValue("ApplicationSettings\\TitleReplacement\\0\\ReplacementType"));
        }
    }
}
