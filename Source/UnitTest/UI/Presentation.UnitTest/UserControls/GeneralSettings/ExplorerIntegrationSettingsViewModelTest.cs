using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Windows;
using pdfforge.PDFCreator.UI.Presentation;
using Translatable;

namespace Presentation.UnitTest.UserControls.GeneralSettings
{
    [TestFixture]
    public class ExplorerIntegrationSettingsViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _osHelper = Substitute.For<IOsHelper>();
            _uacAssistent = Substitute.For<IUacAssistant>();
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        private ITranslationUpdater _translationUpdater;
        private IOsHelper _osHelper;
        private IUacAssistant _uacAssistent;

        private ExplorerIntegrationSettingsViewModel BuildViewModel()
        {
            return new ExplorerIntegrationSettingsViewModel(_uacAssistent, _osHelper, _translationUpdater, new CurrentSettingsProvider(new DefaultSettingsProvider()), new GpoSettingsDefaults());
        }

        [TestCase(true, Visibility.Collapsed)]
        [TestCase(false, Visibility.Visible)]
        public void SetAdminInOsHelper_RequireUACVisibilty_GetVisibility(bool userAdminState, Visibility visibility)
        {
            var viewModel = BuildViewModel();

            _osHelper.UserIsAdministrator().Returns(userAdminState);
            var viewModelRequiresUacVisibility = viewModel.RequiresUacVisibility;
            Assert.AreEqual(viewModelRequiresUacVisibility, visibility);
        }

        [Test]
        public void RunConstructur_RequestProperties_FilledUpProperties()
        {
            var viewModel = BuildViewModel();

            Assert.IsFalse(viewModel.IsAddedToExplorer);
            Assert.IsFalse(viewModel.IsRemovedFromExplorer);
            Assert.NotNull(viewModel.AddExplorerIntegrationCommand);
            Assert.NotNull(viewModel.RemoveExplorerIntegrationCommand);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ExecuteAddToExplorerContextMenu_IsAddedToExplorerInSpecifiedStates(bool uacAssistentState)
        {
            var viewModel = BuildViewModel();
            _uacAssistent.AddExplorerIntegration().Returns(uacAssistentState);
            viewModel.AddExplorerIntegrationCommand.Execute(null);
            Assert.AreEqual(uacAssistentState, viewModel.IsAddedToExplorer);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ExecuteRemoveFromExplorerContextMenu_IsRemovedFromExplorerInSpecifiedStates(bool uacAssistentState)
        {
            var viewModel = BuildViewModel();
            _uacAssistent.RemoveExplorerIntegration().Returns(uacAssistentState);
            viewModel.RemoveExplorerIntegrationCommand.Execute(null);
            Assert.AreEqual(uacAssistentState, viewModel.IsRemovedFromExplorer);
        }
    }
}
