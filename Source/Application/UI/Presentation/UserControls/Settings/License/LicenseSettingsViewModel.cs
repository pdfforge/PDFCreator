using System;
using System.Globalization;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using NLog;
using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public class LicenseSettingsOnlineViewModel : LicenseSettingsViewModel
    {
        public LicenseSettingsOnlineViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker,
            IInteractionRequest interactionRequest, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings, IEventAggregator eventAggregator)
            : base(processStarter, licenseChecker, null, interactionRequest, translationUpdater, gpoSettings, eventAggregator)
        //offlineActivator not required. Set to null. 
        {
        }

        public override bool ShowOfflineActivation => false;

        protected override Task OfflineActivationCommandExecute(object obj)
        {
            throw new InvalidOperationException("Method should not be called when CanExecute returns False");
        }

        protected override bool OfflineActivationCommandCanExecute(object o)
        {
            return false;
        }
    }

    public class LicenseSettingsOfflineViewModel : LicenseSettingsViewModel
    {
        public LicenseSettingsOfflineViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker, IOfflineActivator offlineActivator,
            IInteractionRequest interactionRequest, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings, IEventAggregator eventAggregator)
            : base(processStarter, licenseChecker, offlineActivator, interactionRequest, translationUpdater, gpoSettings,eventAggregator)
        {
        }

        public override bool ShowOnlineActivation => false;

        protected override Task OnlineActivationCommandExecute(object obj)
        {
            throw new InvalidOperationException();
        }

        protected override bool OnlineActivationCommandCanExecute(object o)
        {
            return false;
        }
    }

    public class LicenseSettingsViewModel : TranslatableViewModelBase<LicenseSettingsTranslation>, ITabViewModel
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly IGpoSettings _gpoSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ILicenseChecker _licenseChecker;
        private readonly LicenseKeySyntaxChecker _licenseKeySyntaxChecker = new LicenseKeySyntaxChecker();
        private readonly IOfflineActivator _offlineActivator;
        private readonly IProcessStarter _processStarter;

        private Option<Activation, LicenseError> _activation;
        
        public LicenseSettingsViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker, IOfflineActivator offlineActivator,
            IInteractionRequest interactionRequest, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings,
            IEventAggregator eventAggregator):base(translationUpdater)
        {
            ShareLicenseForAllUsersEnabled = true;
            _processStarter = processStarter;
            _licenseChecker = licenseChecker;
            _offlineActivator = offlineActivator;

            _interactionRequest = interactionRequest;
            _gpoSettings = gpoSettings;
            _eventAggregator = eventAggregator;

            OnlineActivationAsyncCommand = new AsyncCommand(OnlineActivationCommandExecute, OnlineActivationCommandCanExecute);
            OfflineActivationAsyncCommand = new AsyncCommand(OfflineActivationCommandExecute, OfflineActivationCommandCanExecute);
            ManageLicensesCommand = new DelegateCommand(ManageLicensesCommandExecute);

            _activation = licenseChecker.GetSavedActivation();
        }

        public bool ShareLicenseForAllUsersEnabled { private get; set; }

        public virtual bool ShowOnlineActivation => true;
        public virtual bool ShowOfflineActivation => true;

        public string Licensee => _activation.Match(a => a.Licensee, e => "");
        public string MachineId => _activation.Match(a => a.MachineId, e => "");
        public string LicenseStatusText => DetermineLicenseStatusText(_activation);
        public LicenseStatusForView LicenseStatusForView => DetermineLicenseStatus(_activation);

        public string LicenseExpiryDate
        {
            get
            {
                var activationDate = _activation.Map(a =>
                {
                    if (a.LicenseExpires.Year > 2035)
                        return Translation.Never;

                    if (a.LicenseExpires == DateTime.MinValue)
                        return "";
                    return a.LicenseExpires.ToShortDateString();
                });

                return activationDate.ValueOr("");
            }
        }

        public string ActivationValidTill
        {
            get
            {
                return _activation
                    .Filter(a => a.ActivatedTill > DateTime.MinValue, LicenseError.NoActivation)
                    .Match(
                        a => a.ActivatedTill.ToString(CultureInfo.InstalledUICulture),
                        e => "");
            }
        }

        public string LastActivationTime
        {
            get
            {
                var activation = _activation.Filter(a => a.TimeOfActivation > DateTime.MinValue, LicenseError.NoActivation);

                return activation.Match(
                    a => a.TimeOfActivation.ToString(CultureInfo.InstalledUICulture),
                    e => "");
            }
        }

        public AsyncCommand OnlineActivationAsyncCommand { get; }

        public AsyncCommand OfflineActivationAsyncCommand { get; }

        public ICommand ManageLicensesCommand { get; }

        public string LicenseKey
        {
            get
            {
                return _activation.Match(
                    a => _licenseKeySyntaxChecker.NormalizeLicenseKey(a.Key),
                    e => "");
            }
        }

        public string Title { get; set; } = "License";
        public IconList Icon { get; set; } = IconList.LicenseSettings;
        public bool HiddenByGPO => _gpoSettings.HideLicenseTab;
        public bool BlockedByGPO => false;
        public bool HasNotSupportedFeatures => false;

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();

            Title = Translation.LicenseTabText;
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(Translation));
            RaisePropertyChanged(nameof(LicenseStatusText));
        }

        protected virtual async Task OfflineActivationCommandExecute(object obj)
        {
            if (_offlineActivator == null)
                return;

            var lastActivationKey = LicenseKey;

            if (string.IsNullOrWhiteSpace(lastActivationKey))
            {
                lastActivationKey = _licenseChecker.GetSavedLicenseKey().ValueOr("");
            }

            var interaction = new OfflineActivationInteraction(lastActivationKey);
            await _interactionRequest.RaiseAsync(interaction);

            if (interaction.Success)
            {
                var activation = _offlineActivator.ValidateOfflineActivationString(interaction.LicenseServerAnswer);

                try
                {
                    activation.MatchSome(a => _offlineActivator.SaveActivation(a));
                }
                catch (SecurityException)
                {
                    _logger.Info("Can't save activation. Please share the license for all users.");
                }
                //Just to show in UI
                //LicenseChecker in UpdateActivation can't save activation
                await UpdateActivation(activation);
                RaiseLicensePropertyChangedEvents();
            }
        }

        protected virtual bool OnlineActivationCommandCanExecute(object o)
        {
            return true;
        }

        protected virtual bool OfflineActivationCommandCanExecute(object o)
        {
            return true;
        }

        protected virtual async Task OnlineActivationCommandExecute(object o)
        {
            var lastActivationKey = LicenseKey;

            if (string.IsNullOrWhiteSpace(lastActivationKey))
            {
                lastActivationKey = _licenseChecker.GetSavedLicenseKey().ValueOr("");
            }

            var title = Translation.EnterLicenseKey;
            var questionText = Translation.EnterLicenseKeyColon;


            var inputInteraction = new InputInteraction(title, questionText, ValidateLicenseKey);
            inputInteraction.InputText = lastActivationKey;

            await _interactionRequest.RaiseAsync(inputInteraction);

            var key = inputInteraction.InputText;
            if (inputInteraction.Success && !string.IsNullOrEmpty(key))
            {
                await Activate(key);
                RaiseLicensePropertyChangedEvents();
            }
        }

        private void RaiseLicensePropertyChangedEvents()
        {
            RaisePropertyChanged(nameof(LicenseKey));
            RaisePropertyChanged(nameof(LicenseStatusText));
            RaisePropertyChanged(nameof(LicenseStatusForView));
            RaisePropertyChanged(nameof(LicenseExpiryDate));
            RaisePropertyChanged(nameof(ActivationValidTill));
            RaisePropertyChanged(nameof(LastActivationTime));
            RaisePropertyChanged(nameof(Licensee));
            RaisePropertyChanged(nameof(MachineId));

        }

        public InputValidation ValidateLicenseKey(string s)
        {
            var validationResult = _licenseKeySyntaxChecker.ValidateLicenseKey(s);

            string message;
            switch (validationResult)
            {
                case ValidationResult.InvalidCharacters:
                    message = Translation.LicenseKeyContainsIllegalCharacters;
                    return new InputValidation(false, message);
                case ValidationResult.WrongFormat:
                    message = Translation.GetLicenseKeyHasWrongFormatMessage("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-12345");
                    return new InputValidation(false, message);
                case ValidationResult.Valid:
                default:
                    break;
            }

            return new InputValidation(true);
        }

        private async Task Activate(string key)
        {
            var activation = _licenseChecker.ActivateWithoutSaving(key.Replace("-", ""));
            await UpdateActivation(activation);
        }

        private async Task UpdateActivation(Option<Activation, LicenseError> activation)
        {
            var oldKey = _activation.Match(a => a.Key, e => "");

            //Save only valid activation. Invalid activations might throw exceptions during saving.
            if (activation.Exists(a => a.IsLicenseStillValid()))
            {
                activation.MatchSome(a =>
                {
                    _activation = activation;
                    _licenseChecker.SaveActivation(a);
                });
            }
            //Save activation if current key is blocked
            else if (activation.Exists(a => (a.Result == Result.BLOCKED) && a.Key == oldKey))
            {
                activation.MatchSome(a =>
                {
                    _activation = activation;
                    _licenseChecker.SaveActivation(a);
                });
            }

            //Notify user
            await InvokeActivationResponse(activation);
        }

        public event EventHandler CloseLicenseWindowEvent;

        private void RaiseCloseWindow()
        {
            CloseLicenseWindowEvent?.Invoke(this, EventArgs.Empty);
        }

        private async Task InvokeActivationResponse(Option<Activation, LicenseError> activation)
        {
            if (!activation.Exists(a => a.IsActivationStillValid()))
            {
                var failedTitle = Translation.ActivationFailed;
                var failedMessage = Translation.ActivationFailedMessage + Environment.NewLine + DetermineLicenseStatusText(activation);
                var failedInteraction = new MessageInteraction(failedMessage, failedTitle, MessageOptions.OK, MessageIcon.Error);
                await _interactionRequest.RaiseAsync(failedInteraction);
            }
            else if (ShareLicenseForAllUsersEnabled)
            {
                //StoreLicenseForAllUsersQuery is also a Successful Message
                if (activation.HasValue)
                {
                    var a = activation.ValueOr(() => null);
                    await StoreLicenseForAllUsersQuery(a);
                    RaiseCloseWindow();
                }
            }
            else
            {
                var successTitle = Translation.ActivationSuccessful;
                var successMessage = Translation.ActivationSuccessfulMessage;
                var successInteraction = new MessageInteraction(successMessage, successTitle, MessageOptions.OK, MessageIcon.PDFForge);
                await _interactionRequest.RaiseAsync(successInteraction);
                RaiseCloseWindow();
            }
        }

        private string DetermineLicenseStatusText(Option<Activation, LicenseError> activation)
        {
            return activation.Match(DetermineLicenseStatusText, e =>
            {
                switch (e)
                {
                    case LicenseError.NoActivation:
                    case LicenseError.NoLicenseKey:
                        return Translation.LicenseStatusNoLicenseKey;
                    default: return Translation.LicenseStatusNoServerConnection;
                }
            });
        }

        private LicenseStatusState DetermineLicenseStatusState(Activation activation)
        {
            var returnValue = new LicenseStatusState {Translation = Translation.LicenseStatusError, Status = LicenseStatusForView.Invalid};

            switch (activation.Result)
            {
                case Result.OK:
                    //Activation must be checked before license
                    if (!activation.IsActivationStillValid())
                    {
                        returnValue.Translation = Translation.LicenseStatusActivationExpired;
                    }
                    else if (!activation.IsLicenseStillValid())
                    {
                        returnValue.Translation = Translation.LicenseStatusLicenseExpired;
                    }
                    else if (activation.LicenseType == LicenseType.PERPETUAL && DateTime.Now > activation.LicenseExpires)
                    {
                        returnValue.Translation = Translation.LicenseStatusValidForVersionButLicenseExpired;
                    }
                    else
                    {
                        returnValue.Translation = Translation.LicenseStatusValid;
                        returnValue.Status = LicenseStatusForView.Valid;
                    }
                    break;
                case Result.BLOCKED:
                    returnValue.Translation = Translation.LicenseStatusBlocked;
                    break;
                case Result.LICENSE_EXPIRED:
                    returnValue.Translation = Translation.LicenseStatusVersionNotCoveredByLicense;
                    break;
                case Result.LICENSE_LIMIT_REACHED:
                    returnValue.Translation = Translation.LicenseStatusNumberOfActivationsExceeded;
                    break;
                case Result.INVALID_LICENSE_KEY:
                    returnValue.Translation = Translation.LicenseStatusInvalidLicenseKey;
                    break;
                case Result.NO_LICENSE_KEY:
                    returnValue.Translation = Translation.LicenseStatusNoLicenseKey;
                    break;
                case Result.NO_SERVER_CONNECTION:
                    returnValue.Translation = Translation.LicenseStatusNoServerConnection;
                    break;
                case Result.UNKNOWN_VERSION:
                case Result.AUTH_FAILED:
                case Result.MACHINE_MISMATCH:
                case Result.VERSION_MISMATCH:
                case Result.PRODUCT_MISMATCH:
                case Result.NO_ACTIVATION:
                case Result.NO_HX_DLL:
                case Result.ERROR:
                default:
                    break;
            }
            return returnValue;
        }

        private LicenseStatusForView DetermineLicenseStatus(Activation activation) => DetermineLicenseStatusState(activation).Status;
        private string DetermineLicenseStatusText(Activation activation) => DetermineLicenseStatusState(activation).Translation;

        private LicenseStatusForView DetermineLicenseStatus(Option<Activation, LicenseError> activation)
        {
            return activation.Match(DetermineLicenseStatus, e => LicenseStatusForView.Invalid);
        }


        private void ManageLicensesCommandExecute(object o)
        {
            try
            {
                _processStarter.Start(Urls.LicenseServerUrl);
            }
            catch
            {
                // ignored
            }
        }

        private async Task StoreLicenseForAllUsersQuery(Activation activation)
        {
            var interaction = new StoreLicenseForAllUsersInteraction(activation.LSA, activation.Key);
            await _interactionRequest.RaiseAsync(interaction);
        }

        private struct LicenseStatusState
        {
            public string Translation;
            public LicenseStatusForView Status;
        }

        public void MountView()
        {
            
        }

        public void UnmountView()
        {
        }
    }

    public enum LicenseStatusForView
    {
        Valid,
        ValidForVersionButLicenseExpired,
        Invalid
    }
}
