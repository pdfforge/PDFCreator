using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    internal class DropboxUserControlViewModelTest
    {
        private DropboxUserControlViewModel _viewModel;
        private ObservableCollection<DropboxAccount> _dropboxAccounts;
        private ICurrentSettings<Accounts> _accountsProvider;
        private ICommand _addCommand;

        [SetUp]
        public void Setup()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            var settingsProvider = Substitute.For<ICurrentSettingsProvider>();
            settingsProvider.SelectedProfile.Returns(new ConversionProfile());

            var settings = new PdfCreatorSettings();
            _dropboxAccounts = new ObservableCollection<DropboxAccount>();
            settings.ApplicationSettings.Accounts.DropboxAccounts = _dropboxAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var commandLocator = Substitute.For<ICommandLocator>();
            commandLocator.CreateMacroCommand().Returns(x => new MacroCommandBuilder(commandLocator));

            _addCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<DropboxAccountAddCommand>().Returns(_addCommand);

            _viewModel = new DropboxUserControlViewModel(translationUpdater, _accountsProvider, settingsProvider, commandLocator, new TokenViewModelFactory(settingsProvider, new TokenHelper(new DesignTimeTranslationUpdater())), null, new GpoSettingsDefaults());
        }

        [Test]
        public void Initialize_AddCommandGetsSetByCommandLocator()
        {
            Assert.AreSame(_addCommand, _viewModel.AddDropboxAccountCommand.GetCommand(0));
            Assert.IsNotNull(_viewModel.AddDropboxAccountCommand.GetCommand(1), "Missing UpdateView Command");
        }

        [Test]
        public void AddAccount_LatestAccountIsCurrentItemInView()
        {
            var collectionView = CollectionViewSource.GetDefaultView(_dropboxAccounts);

            //add new Account (what the AddCommand would do)
            var newAccount = new DropboxAccount();
            _dropboxAccounts.Add(newAccount);

            _viewModel.AddDropboxAccountCommand.Execute(null);

            Assert.AreSame(newAccount, collectionView.CurrentItem, "Newest Account is not selected Item");
        }

        [Test]
        public void SharedFolderTokenViewModel_SetActionWritesToCurrentProfilesDropboxSharedFolder()
        {
            _viewModel.SharedFolderTokenViewModel.Text = "Text from TokenViewModel";

            Assert.AreEqual("Text from TokenViewModel", _viewModel.CurrentProfile.DropboxSettings.SharedFolder);
        }

        [Test]
        public void SharedFolderTokenViewModel_GetFunctionReadsFromCurrentProfilesFtpDropboxSharedFolder()
        {
            _viewModel.CurrentProfile.DropboxSettings.SharedFolder = "SharedFolder from Profile";
            Assert.AreEqual("SharedFolder from Profile", _viewModel.SharedFolderTokenViewModel.Text);
        }

        [Test]
        public void DesignTimeViewModelIsNewable()
        {
            var dtvm = new DesignTimeDropboxUserControlViewModel();
            Assert.NotNull(dtvm);
        }
    }
}
