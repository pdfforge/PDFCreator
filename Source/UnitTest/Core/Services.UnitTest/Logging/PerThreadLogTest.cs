using NLog;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Services.Logging;

namespace pdfforge.PDFCreator.UnitTest.Core.Services.UnitTest.Logging
{
    [TestFixture]
    public class PerThreadLogTest
    {
        [Test]
        public void NoLogs_OverallLevelIsOff()
        {
            var log = new PerThreadLog(1);

            Assert.AreEqual(LogLevel.Off, log.OverallSeverity);
        }

        [Test]
        public void ThreeLogsWithDifferentSeverity_OverallLevelIsHighestSeverity()
        {
            var log = new PerThreadLog(1);

            log.AddLog(LogLevel.Fatal, "1");
            log.AddLog(LogLevel.Warn, "1");
            log.AddLog(LogLevel.Trace, "1");

            Assert.AreEqual(LogLevel.Fatal, log.OverallSeverity);
        }
    }
}
