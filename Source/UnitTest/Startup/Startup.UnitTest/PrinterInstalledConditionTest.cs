using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Collections.Generic;
using Translatable;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class PrinterInstalledConditionTest
    {
        [Test]
        public void WhenNoRepairRequired_Successful()
        {
            var repairPrinterAssitant = Substitute.For<IRepairPrinterAssistant>();
            repairPrinterAssitant.IsRepairRequired().Returns(false);

            var settingsLoader = Substitute.For<ISettingsLoader>();

            var printerInstalledCondition = new PrinterInstalledCondition(repairPrinterAssitant, settingsLoader, new TranslationFactory());

            var result = printerInstalledCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void WhenNoRepairFails_CheckFails()
        {
            var repairPrinterAssitant = Substitute.For<IRepairPrinterAssistant>();
            repairPrinterAssitant.IsRepairRequired().Returns(true);
            repairPrinterAssitant.TryRepairPrinter(Arg.Any<IEnumerable<string>>()).Returns(false);

            var settingsLoader = Substitute.For<ISettingsLoader>();
            settingsLoader.LoadPdfCreatorSettings().Returns(new PdfCreatorSettings());

            var printerInstalledCondition = new PrinterInstalledCondition(repairPrinterAssitant, settingsLoader, new TranslationFactory());

            var result = printerInstalledCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.PrintersBroken, result.ExitCode);
        }

        [Test]
        public void WhenNoRepairSucceeds_Successful()
        {
            var repairPrinterAssitant = Substitute.For<IRepairPrinterAssistant>();
            repairPrinterAssitant.IsRepairRequired().Returns(true);
            repairPrinterAssitant.TryRepairPrinter(Arg.Any<IEnumerable<string>>()).Returns(true);

            var settingsLoader = Substitute.For<ISettingsLoader>();
            settingsLoader.LoadPdfCreatorSettings().Returns(new PdfCreatorSettings());

            var printerInstalledCondition = new PrinterInstalledCondition(repairPrinterAssitant, settingsLoader, new TranslationFactory());

            var result = printerInstalledCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void DuringRepair_AllMappedPrintersArePassedToRepairAssistant()
        {
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("Printer1", ""));
            settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping("Printer2", ""));

            var settingsLoader = Substitute.For<ISettingsLoader>();
            settingsLoader.LoadPdfCreatorSettings().Returns(settings);

            IEnumerable<string> printersToRepair = null;

            var repairPrinterAssitant = Substitute.For<IRepairPrinterAssistant>();
            repairPrinterAssitant.IsRepairRequired().Returns(true);
            repairPrinterAssitant.TryRepairPrinter(Arg.Any<IEnumerable<string>>()).Returns(x =>
            {
                printersToRepair = x.ArgAt<IEnumerable<string>>(0);
                return true;
            });

            var printerInstalledCondition = new PrinterInstalledCondition(repairPrinterAssitant, settingsLoader, new TranslationFactory());

            printerInstalledCondition.Check();

            CollectionAssert.AreEquivalent(new[] { "Printer1", "Printer2" }, printersToRepair);
        }
    }
}
