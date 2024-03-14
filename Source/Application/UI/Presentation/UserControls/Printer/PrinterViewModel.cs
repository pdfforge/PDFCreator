using System;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using NaturalSort.Extension;
using NLog;
using pdfforge.Obsidian.Trigger;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class PrinterViewModel : TranslatableViewModelBase<PrinterViewTranslation>, IMountable
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private ConversionProfileWrapper _defaultProfile;

        private readonly IPrinterAssistant _printerAssistant;
        private readonly IPrinterHelper _printerHelper;
        private readonly IGpoSettings _gpoSettings;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ISettingsProvider _settingsProvider;

        private ListCollectionView _printerMappingView;

        private readonly ICurrentSettings<ObservableCollection<PrinterMapping>> _printerMappingProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;

        public ObservableCollection<ConversionProfileWrapper> ConversionProfiles { get; private set; }
        public ObservableCollection<PrinterMappingWrapper> PrinterMappings { get; set; }

        public ICommand AddPrinterCommand { get; private set; }
        public DelegateCommand RenamePrinterCommand { get; }
        public DelegateCommand EditProfileCommand { get; }
        public DelegateCommand DeletePrinterCommand { get; }
        public DelegateCommand SetPrimaryPrinterCommand { get; }

        private readonly NaturalSortComparer _naturalSortComparer = new NaturalSortComparer(StringComparison.CurrentCulture);

        public PrinterViewModel(
            ISettingsProvider settingsProvider,
            ICurrentSettings<ObservableCollection<PrinterMapping>> printerMappingProvider,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            IPrinterAssistant printerAssistant,
            ITranslationUpdater translationUpdater,
            IPrinterHelper printerHelper,
            IGpoSettings gpoSettings,
            IInteractionRequest interactionRequest)
            : base(translationUpdater)
        {
            _printerHelper = printerHelper;
            _gpoSettings = gpoSettings;
            _interactionRequest = interactionRequest;
            _printerAssistant = printerAssistant;
            _settingsProvider = settingsProvider;
            _printerMappingProvider = printerMappingProvider;
            _profilesProvider = profilesProvider;

            AddPrinterCommand = new DelegateCommand(AddPrinterExecute);
            RenamePrinterCommand = new DelegateCommand(RenamePrinterExecute);
            EditProfileCommand = new DelegateCommand(EditProfileExecute);
            DeletePrinterCommand = new DelegateCommand(DeletePrinterCommandExecute);
            SetPrimaryPrinterCommand = new DelegateCommand(SetPrimaryPrinterExecute);
        }

        private async void EditProfileExecute(object obj)
        {
            if (obj is not PrinterMappingWrapper printerMappingWrapper)
                return;
            
            await EditProfile(printerMappingWrapper);
        }

        public async Task EditProfile(PrinterMappingWrapper printerMappingWrapper)
        {
            var interaction = new EditPrinterProfileUserInteraction(printerMappingWrapper, ConversionProfiles);
            
            await _interactionRequest.RaiseAsync(interaction);

            if (interaction.Success)
                printerMappingWrapper.Profile = interaction.ResultProfile;
        }

        public void MountView()
        {
            SetupConversionProfiles();

            var printerMappings = SetupPrinterMappings();

            PrinterMappings = printerMappings.ObservableCollection
                .OrderBy(x => x.PrinterName, StringComparison.OrdinalIgnoreCase.WithNaturalSort()).ToObservableCollection();
            RaisePropertyChanged(nameof(PrinterMappings));
            PrinterMappings.CollectionChanged += PrinterMappings_OnCollectionChanged;

            _printerMappingView = (ListCollectionView)CollectionViewSource.GetDefaultView(PrinterMappings);

            Comparison<PrinterMappingWrapper> printerMappingWrapperComparison = (pmX, pmY)
                => _naturalSortComparer.Compare(pmX.PrinterName, pmY.PrinterName);
            var printerMappingWrapperComparer = Comparer<PrinterMappingWrapper>.Create(printerMappingWrapperComparison);
            _printerMappingView.CustomSort = printerMappingWrapperComparer;

            if (string.IsNullOrEmpty(_settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter) ||
                PrinterMappings.All(o => o.PrinterName != _settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter))
            {
                _settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter = _printerHelper.GetApplicablePDFCreatorPrinter("PDFCreator",
                    "PDFCreator");
            }

            PrimaryPrinter = _settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter;
        }

        public void UnmountView()
        {
            PrinterMappings.CollectionChanged -= PrinterMappings_OnCollectionChanged;
        }

        private void SetupConversionProfiles()
        {
            var conversionProfiles =
                _profilesProvider.Settings.Select(x => new ConversionProfileWrapper(x))
                   .OrderBy(pm => pm.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort())
                   .ToObservableCollection();

            var dummyLastUsedProfile = new ConversionProfileWrapper
            (
                new ConversionProfile()
                {
                    Name = "<" + Translation.LastUsedProfileMapping + ">",
                    Guid = ProfileGuids.LAST_USED_PROFILE_GUID
                });

            conversionProfiles.Insert(0, dummyLastUsedProfile);

            ConversionProfiles = conversionProfiles;
            RaisePropertyChanged(nameof(ConversionProfiles));

            _defaultProfile = ConversionProfiles
                .FirstOrDefault(x => x.ConversionProfile.IsDefault);
        }

        private Helper.SynchronizedCollection<PrinterMappingWrapper> SetupPrinterMappings()
        {
            if (_printerMappingProvider?.Settings == null)
                return null;

            var mappingWrappers = new List<PrinterMappingWrapper>();

            foreach (var printerMapping in _printerMappingProvider.Settings)
            {
                var profileWrapper = ConversionProfiles.FirstOrDefault(p => p.ConversionProfile.Guid == printerMapping.ProfileGuid);
                if (profileWrapper == null)
                {
                    _logger.Debug("Could not find profile for " + printerMapping.PrinterName + ". Default Profile is set");
                    printerMapping.ProfileGuid = _defaultProfile.ConversionProfile.Guid;
                }
                var mappingWrapper = new PrinterMappingWrapper(printerMapping, ConversionProfiles);
                mappingWrappers.Add(mappingWrapper);
            }

            return new Helper.SynchronizedCollection<PrinterMappingWrapper>(mappingWrappers);
        }

        private void SetPrimaryPrinterExecute(object parameter)
        {
            var selectedPrinter = parameter as PrinterMappingWrapper;

            if (selectedPrinter == null)
                return;

            PrimaryPrinter = selectedPrinter.PrinterName;
        }

        public string PrimaryPrinter
        {
            get
            {
                return _settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter;
            }
            set
            {
                _settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter = value;
                foreach (var printerMappingWrapper in PrinterMappings)
                {
                    printerMappingWrapper.PrimaryPrinter = value;
                }
                RaisePropertyChanged(nameof(PrimaryPrinter));
            }
        }

        private void PrinterMappings_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _printerMappingProvider.Settings.Clear();

            foreach (var printerMappingWrapper in PrinterMappings)
            {
                _printerMappingProvider.Settings.Add(printerMappingWrapper.PrinterMapping);
                if (printerMappingWrapper.Profile == null)
                    printerMappingWrapper.Profile = _defaultProfile;
            }
        }

        private async void AddPrinterExecute(object o)
        {
            var printerName = await _printerAssistant.AddPrinter();

            if (string.IsNullOrWhiteSpace(printerName))
                return;

            if (PrinterMappings.Any(p => p.PrinterName == printerName))
                return;

            var newMapping = new PrinterMappingWrapper(new PrinterMapping(printerName, _defaultProfile.ConversionProfile.Guid), ConversionProfiles);
            PrinterMappings.Add(newMapping);
        }

        private async void RenamePrinterExecute(object obj)
        {
            if (_printerMappingView.CurrentItem is not PrinterMappingWrapper currentMapping)
                return;

            var newPrinterName = await _printerAssistant.RenamePrinter(currentMapping.PrinterName);

            if (string.IsNullOrWhiteSpace(newPrinterName))
                return;

            if(currentMapping.PrinterName == newPrinterName)
                return;

            var wasPrimaryPrinter = currentMapping.IsPrimaryPrinter;
            currentMapping.PrinterName = newPrinterName;
            if (wasPrimaryPrinter)
                PrimaryPrinter = newPrinterName;

            _printerMappingView.Refresh();
        }

        private async void DeletePrinterCommandExecute(object obj)
        {
            if (_printerMappingView.CurrentItem is not PrinterMappingWrapper currentMapping)
                return;

            if (!await _printerAssistant.DeletePrinter(currentMapping.PrinterName, PrinterMappings.Count))
                return;

            var wasPrimaryPrinter = currentMapping.IsPrimaryPrinter;
            PrinterMappings.Remove(currentMapping);
            if(wasPrimaryPrinter)
                PrimaryPrinter = _printerHelper.GetApplicablePDFCreatorPrinter(PrimaryPrinter);
        }

        public bool PrinterIsDisabled
        {
            get
            {
                if (_profilesProvider.Settings == null)
                    return false;

                return _gpoSettings != null && _gpoSettings.DisablePrinterTab;
            }
        }
    }
}