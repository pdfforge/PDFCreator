using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class DefaultPrinterSettingsViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        private ITranslationUpdater _translationUpdater;

        private DefaultPrinterSettingsViewModel BuildViewModel()
        {
            return new DefaultPrinterSettingsViewModel(_translationUpdater, new CurrentSettingsProvider(new DefaultSettingsProvider()), new GpoSettingsDefaults(), new DesignTimeCurrentSettings<CreatorAppSettings>());
        }

        [Test]
        public void RequestAskSwitchPrinter_Get2AskSwitchPrinter()
        {
            var viewModel = BuildViewModel();

            var switchPrinterValues = viewModel.AskSwitchPrinterValues;

            Assert.IsTrue(switchPrinterValues.ElementAt(0).Value);
            Assert.IsFalse(switchPrinterValues.ElementAt(1).Value);

            Assert.AreEqual(switchPrinterValues.ElementAt(0).Name, viewModel.Translation.Ask);
            Assert.AreEqual(switchPrinterValues.ElementAt(1).Name, viewModel.Translation.Yes);
        }
    }
}
