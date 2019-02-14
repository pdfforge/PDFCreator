using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Printer;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    internal class PrinterTabViewModel
    {
        [SetUp]
        public void SetUp()
        {
            _translationHelper = new TranslationHelper(new DefaultSettingsProvider(), new AssemblyHelper(GetType().Assembly), new TranslationFactory(), null);
            _translationHelper.InitEmptyTranslator();

            _printerActionAssistant = Substitute.For<IPrinterActionsAssistant>();

            _profile1 = new ConversionProfile
            {
                Name = "Profile1",
                Guid = "Profile1Guid"
            };

            _profile2 = new ConversionProfile
            {
                Name = "Profile1",
                Guid = "Profile1Guid"
            };

            _applicationSettings = new ApplicationSettings();

            var mappings = new[]
            {
                new PrinterMapping(ExistingPrinter, _profile1.Guid)
                , new PrinterMapping(UnknownPrinter, _profile2.Guid)
            };

            _applicationSettings.PrinterMappings = new ObservableCollection<PrinterMapping>(mappings);

            _settings = new PdfCreatorSettings();
            _settings.ApplicationSettings = _applicationSettings;
            _settings.ConversionProfiles.Add(_profile1);
            _settings.ConversionProfiles.Add(_profile2);

            _printerProvider = Substitute.For<IPrinterProvider>();
        }

        private const string ExistingPrinter = "PDFCreator";
        private const string UnknownPrinter = "UnknownPrinter";
        private ApplicationSettings _applicationSettings;
        private ConversionProfile _profile1;
        private ConversionProfile _profile2;
        private ITranslationHelper _translationHelper;
        private IPrinterProvider _printerProvider;
        private IPrinterActionsAssistant _printerActionAssistant;
        private PdfCreatorSettings _settings;
        private ICurrentSettings<ObservableCollection<PrinterMapping>> _printerMappingsProvider;
        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;

        private PrinterViewModel BuildViewModel(IList<ConversionProfile> profiles = null)
        {
            _printerMappingsProvider = Substitute.For<ICurrentSettings<ObservableCollection<PrinterMapping>>>();

            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();
            _printerMappingsProvider.Settings.Returns(_ => _settings.ApplicationSettings.PrinterMappings);
            _profilesProvider.Settings.Returns(_ => _settings.ConversionProfiles);

            var settingsProvider = Substitute.For<ISettingsProvider>();
            settingsProvider.Settings.Returns(_ => _settings);

            var printerHelper = Substitute.For<IPrinterHelper>();
            printerHelper.GetApplicablePDFCreatorPrinter(Arg.Any<string>(), Arg.Any<string>()).Returns("PDFCreator");

            var list = new ObservableCollection<ConversionProfile>();

            if (profiles != null && profiles.Count > 0)
            {
                foreach (var conversionProfile in profiles)
                {
                    list.Add(conversionProfile);
                }
            }

            _profilesProvider.Settings.Returns(list);

            var viewModel = new PrinterViewModel(_printerProvider, settingsProvider, _printerMappingsProvider, _profilesProvider, _printerActionAssistant, null, new DesignTimeTranslationUpdater(), printerHelper, null);

            viewModel.MountView();

            return viewModel;
        }

        [Test]
        public void AddPrinterCommand_Execute_PrinterMappingContainsPrinterWithGivenName_DefaultProfileGuidIsAssigned()
        {
            var conversionProfiles = new[]
            {
                new ConversionProfile
                {
                    Guid = ProfileGuids.DEFAULT_PROFILE_GUID
                }
            };

            _settings = new PdfCreatorSettings();

            var newPrinterName = "New printer for testing";
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] { newPrinterName });
            var printerTabViewModel = BuildViewModel(conversionProfiles);

            _printerActionAssistant.AddPrinter().Returns<string>(x => newPrinterName);

            printerTabViewModel.AddPrinterCommand.Execute(null);

            Assert.AreEqual(1, printerTabViewModel.PrinterMappings.Count, "Added more or less than one printer");
            Assert.AreEqual(newPrinterName, printerTabViewModel.PrinterMappings[0].PrinterMapping.PrinterName,
                "Wrong printername");
            Assert.AreEqual(ProfileGuids.DEFAULT_PROFILE_GUID,
                printerTabViewModel.PrinterMappings[0].PrinterMapping.ProfileGuid,
                "Did not assign the default profile GUID.");
        }

        [Test]
        public void DeletePrinterCommand_Execute_DeletePrinterActionGetsCalled()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] { ExistingPrinter, "Something" });
            var printerTabViewModel = BuildViewModel();

            printerTabViewModel.DeletePrinterCommand.Execute(null);

            _printerActionAssistant.Received().DeletePrinter(ExistingPrinter, 2);
        }

        [Test]
        public void DeletePrinterCommand_WithNonexistingPrinter_NotExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] { ExistingPrinter, "Something" });
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToLast();

            Assert.IsFalse(printerTabViewModel.DeletePrinterCommand.CanExecute(null));
        }

        [Test]
        public void DeletePrinterCommand_WithValidPrinter_IsExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] { ExistingPrinter, "Something" });
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToFirst();

            Assert.IsTrue(printerTabViewModel.DeletePrinterCommand.CanExecute(null));
        }

        [Test]
        public void EmptyViewModel_SettingPdfCreatorPrinters_RaisesPdfCreatorPrintersPropertyChanged()
        {
            var eventStub = Substitute.For<IEventHandler<PropertyChangedEventArgs>>();
            var printerTabViewModel = BuildViewModel();
            printerTabViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(printerTabViewModel, "PdfCreatorPrinters");

            printerTabViewModel.PdfCreatorPrinters = new List<string>();

            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void PrimaryPrinter_SetPrimaryPrinter_PrimaryPrinterIsSetInEveryPrinterMappingWrapper()
        {
            var printerTabViewModel = BuildViewModel();
            var printerName = "some funny printer name (still laughing)";
            printerTabViewModel.PrimaryPrinter = printerName;

            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[0].PrimaryPrinter,
                "Wrong primary printer in first PrinterMapping");
            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[1].PrimaryPrinter,
                "Wrong primary printer in second PrinterMapping");
        }

        [Test]
        public void PrimaryPrinter_SetPrimaryPrinter_RaisesPrimaryPrinterPropertyChanged()
        {
            var printerTabViewModel = BuildViewModel();

            var propertyListener = new PropertyChangedListenerMock(printerTabViewModel, "PrimaryPrinter");

            printerTabViewModel.PrimaryPrinter = "something";

            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void RenamePrinterCommand_Execute_DeletePrinterActionGetsCalled()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] { ExistingPrinter, "Something" });
            var printerTabViewModel = BuildViewModel();

            printerTabViewModel.RenamePrinterCommand.Execute(null);

            _printerActionAssistant.Received().RenamePrinter(ExistingPrinter);
        }

        [Test]
        public void RenamePrinterCommand_WithNonexistingPrinter_NotExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] { ExistingPrinter, "Something" });
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToLast();

            Assert.IsFalse(printerTabViewModel.RenamePrinterCommand.CanExecute(null));
        }

        [Test]
        public void RenamePrinterCommand_WithValidPrinter_IsExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] { ExistingPrinter, "Something" });
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToFirst();

            Assert.IsTrue(printerTabViewModel.RenamePrinterCommand.CanExecute(null));
        }

        [Test]
        public void UpdatePrimaryPrinter_GivenNameIsPrimaryPrinterInEveryPrinterMappingWrapper()
        {
            var printerTabViewModel = BuildViewModel();
            const string printerName = "Some funny PrinterName for testing";

            printerTabViewModel.UpdatePrimaryPrinter(printerName);

            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[0].PrimaryPrinter,
                "Wrong primary printer in first PrinterMapping");
            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[1].PrimaryPrinter,
                "Wrong primary printer in second PrinterMapping");
        }

        [Test]
        public void SetPrimaryPrinter_ParameterIsNull_DoesNotThrowException()
        {
            var printerTabViewModel = BuildViewModel();

            Assert.DoesNotThrow(() => printerTabViewModel.SetPrimaryPrinterCommand.Execute(null));
        }

        [Test]
        public void SetPrimaryPrinter_WithPrinterMapping_SetsPrimaryPrinter()
        {
            var printerTabViewModel = BuildViewModel();
            var newMapping = _applicationSettings.PrinterMappings[1];

            printerTabViewModel.SetPrimaryPrinterCommand.Execute(new PrinterMappingWrapper(newMapping, _settings.ConversionProfiles));

            Assert.AreEqual(newMapping.PrinterName, printerTabViewModel.PrimaryPrinter);
        }
    }
}
