using NSubstitute;
using NUnit.Framework;
using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Threading.Tasks;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class LicenseSettingsOnlineViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _savedActivation = null;
            //_expectedLicenseKey = null;
            _activationFromServer = null;

            _process = Substitute.For<IProcessStarter>();
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseChecker.GetSavedActivation().Returns(x => _savedActivation.SomeNotNull(LicenseError.NoActivation));
            _licenseChecker.ActivateWithoutSaving(Arg.Any<string>()).Returns(key => _activationFromServer.SomeNotNull(LicenseError.NoActivation));
            _interactionRequest = new UnitTestInteractionRequest();
        }

        private IProcessStarter _process;
        private UnitTestInteractionRequest _interactionRequest;

        private Activation _savedActivation;
        private Activation _activationFromServer;

        private ILicenseChecker _licenseChecker;

        private LicenseSettingsViewModel BuildViewModel()
        {
            return new LicenseSettingsOnlineViewModel(_process, _licenseChecker, _interactionRequest, new TranslationUpdater(new TranslationFactory(), new ThreadManager()), null);
        }

        [Test]
        public void CheckCanExecuteOfflineActivationCommand_ReturnsFalse()
        {
            var licenseSettingsViewModel = BuildViewModel();
            Assert.IsFalse(licenseSettingsViewModel.OfflineActivationAsyncCommand.CanExecute(null));
        }

        [Test]
        public void CreateViewModel_RequestShowOfflineActivation_ReturnsFalse()
        {
            var licenseSettingsViewModel = BuildViewModel();
            Assert.IsFalse(licenseSettingsViewModel.ShowOfflineActivation);
        }

        [Test]
        public void ExecuteOfflineActivationCommand_DoesNothing()
        {
            var licenseSettingsViewModel = BuildViewModel();

            Assert.ThrowsAsync<InvalidOperationException>(async () => await licenseSettingsViewModel.OfflineActivationAsyncCommand.ExecuteAsync(null));
        }
    }

    [TestFixture]
    public class LicenseSettingsOfflineViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _savedActivation = null;
            //expectedLicenseKey = null;
            _activationFromServer = null;

            _process = Substitute.For<IProcessStarter>();
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseChecker.GetSavedActivation().Returns(x => _savedActivation.SomeNotNull(LicenseError.NoActivation));
            _licenseChecker.ActivateWithoutSaving(Arg.Any<string>()).Returns(key => _activationFromServer.SomeNotNull(LicenseError.NoActivation));
            _offlineActivator = Substitute.For<IOfflineActivator>();
            _interactionInvoker = Substitute.For<IInteractionRequest>();
        }

        private IProcessStarter _process;
        private IInteractionRequest _interactionInvoker;

        //private string _expectedLicenseKey;

        private Activation _savedActivation;
        private Activation _activationFromServer;

        private ILicenseChecker _licenseChecker;
        private IOfflineActivator _offlineActivator;

        private LicenseSettingsViewModel BuildViewModel()
        {
            return new LicenseSettingsOfflineViewModel(_process, _licenseChecker, _offlineActivator, _interactionInvoker, new TranslationUpdater(new TranslationFactory(), new ThreadManager()), null);
        }

        [Test]
        public void CheckCanExecuteOnlineActivationCommand_ReturnsFalse()
        {
            var licenseSettingsViewModel = BuildViewModel();
            Assert.IsFalse(licenseSettingsViewModel.OnlineActivationAsyncCommand.CanExecute(null));
        }

        [Test]
        public void CreateViewModel_RequestShowOnlineActivation_ReturnsFalse()
        {
            var licenseSettingsViewModel = BuildViewModel();
            Assert.IsFalse(licenseSettingsViewModel.ShowOnlineActivation);
        }

        [Test]
        public void ExecuteOnlineActivationCommand_DoesNothing()
        {
            var licenseSettingsViewModel = BuildViewModel();

            Assert.ThrowsAsync<InvalidOperationException>(async () => await licenseSettingsViewModel.OnlineActivationAsyncCommand.ExecuteAsync(null));
        }
    }

    [TestFixture]
    public class LicenseSettingsViewModelTest
    {
        private IProcessStarter _process;
        private UnitTestInteractionRequest _interactionRequest;

        private string _expectedLicenseKey;

        private Activation _savedActivation;
        private Activation _activationFromServer;

        private const string ValidLicenseKey = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";
        private const string ValidLicenseKey_Normalized = "AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-FFFFF";

        private LicenseSettingsTranslation _translation = new LicenseSettingsTranslation();

        private ILicenseChecker _licenseChecker;
        private IOfflineActivator _offlineActivator;

        [SetUp]
        public void Setup()
        {
            _savedActivation = null;
            _expectedLicenseKey = null;
            _activationFromServer = null;

            _process = Substitute.For<IProcessStarter>();
            _licenseChecker = Substitute.For<ILicenseChecker>();
            _licenseChecker.GetSavedActivation().Returns(x => _savedActivation.SomeNotNull(LicenseError.NoActivation));
            _licenseChecker.ActivateWithoutSaving(Arg.Any<string>()).Returns(key => _activationFromServer.SomeNotNull(LicenseError.NoActivation));
            _offlineActivator = Substitute.For<IOfflineActivator>();
            _interactionRequest = new UnitTestInteractionRequest();
        }

        private LicenseSettingsViewModel BuildViewModel()
        {
            return new LicenseSettingsViewModel(_process, _licenseChecker, _offlineActivator, _interactionRequest, new TranslationUpdater(new TranslationFactory(), new ThreadManager()), null);
        }

        private Activation BuildValidActivation(string key)
        {
            var activation = new Activation(true);

            activation.ActivatedTill = DateTime.Today.AddDays(7);
            activation.SetResult(Result.OK, "OK");
            activation.Key = key;

            return activation;
        }

        private Activation BuildBlockedActivation(string key)
        {
            var activation = new Activation(true);

            activation.ActivatedTill = DateTime.MinValue;
            activation.SetResult(Result.BLOCKED, "Blocked");
            activation.Key = key;

            return activation;
        }

        private void CreateLicenseKeyEnteredInteraction(string licenseKey)
        {
            _interactionRequest.RegisterInteractionHandler<InputInteraction>(inputInteraction =>
            {
                inputInteraction.Success = true;
                inputInteraction.InputText = licenseKey;
            });
        }

        private void CreateCancelledInputInteraction()
        {
            _interactionRequest.RegisterInteractionHandler<InputInteraction>(inputInteraction =>
            {
                inputInteraction.Success = false;
                inputInteraction.InputText = "";
            });
        }

        [TestCase("")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAA")]
        [TestCase("!!!!!-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAAA")]
        [TestCase("AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA-AAAAA")]
        public void IsValidNewLicenseKey_WithInvalidKey_IsFalse(string testKey)
        {
            var viewModel = BuildViewModel();
            var validationResult = viewModel.ValidateLicenseKey(testKey);

            Assert.IsFalse(validationResult.IsValid);
        }

        [TestCase("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-12345")]
        [TestCase("ZZZZZ-YYYYY-DEFGH-DDDDD-12345-67890")]
        public void IsValidNewLicenseKey_WithValidKey_IsTrue(string testKey)
        {
            var viewModel = BuildViewModel();

            var validationResult = viewModel.ValidateLicenseKey(testKey);

            Assert.IsTrue(validationResult.IsValid);
        }

        [TestCase(Result.BLOCKED, nameof(LicenseSettingsTranslation.LicenseStatusBlocked), LicenseStatusForView.Invalid)]
        [TestCase(Result.LICENSE_EXPIRED, nameof(LicenseSettingsTranslation.LicenseStatusVersionNotCoveredByLicense), LicenseStatusForView.Invalid)]
        [TestCase(Result.LICENSE_LIMIT_REACHED, nameof(LicenseSettingsTranslation.LicenseStatusNumberOfActivationsExceeded), LicenseStatusForView.Invalid)]
        [TestCase(Result.INVALID_LICENSE_KEY, nameof(LicenseSettingsTranslation.LicenseStatusInvalidLicenseKey), LicenseStatusForView.Invalid)]
        [TestCase(Result.NO_LICENSE_KEY, nameof(LicenseSettingsTranslation.LicenseStatusNoLicenseKey), LicenseStatusForView.Invalid)]
        [TestCase(Result.NO_SERVER_CONNECTION, nameof(LicenseSettingsTranslation.LicenseStatusNoServerConnection), LicenseStatusForView.Invalid)]
        [TestCase(Result.UNKNOWN_VERSION, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        [TestCase(Result.AUTH_FAILED, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        [TestCase(Result.MACHINE_MISMATCH, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        [TestCase(Result.VERSION_MISMATCH, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        [TestCase(Result.PRODUCT_MISMATCH, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        [TestCase(Result.NO_ACTIVATION, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        [TestCase(Result.NO_HX_DLL, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        [TestCase(Result.ERROR, nameof(LicenseSettingsTranslation.LicenseStatusError), LicenseStatusForView.Invalid)]
        public void ResultLicenseLimitReached_GetLicenseStatusText_LicenseStatusLimitReached(Result result, String propName, LicenseStatusForView status)
        {
            var activation = new Activation(false);
            activation.ActivatedTill = DateTime.Now.Subtract(TimeSpan.FromMinutes(60));
            activation.SetResult(result, $"{result}:{propName}:{status}");

            _licenseChecker.GetSavedActivation().Returns(x => activation.SomeNotNull(LicenseError.NoActivation));

            var viewModel = BuildViewModel();

            var licenseText = viewModel.LicenseStatusText;
            var viewStatus = viewModel.LicenseStatusForView;

            var translation = typeof(LicenseSettingsTranslation).GetProperty(propName).GetValue(viewModel.Translation);

            Assert.IsTrue(translation.Equals(licenseText));
            Assert.IsTrue(viewStatus.Equals(status));
        }

        [Test]
        public void ActivationValidTill_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            var date = DateTime.Now;
            _savedActivation = new Activation(true) { ActivatedTill = date };

            var viewModel = BuildViewModel();

            Assert.AreEqual(date.ToString(CultureInfo.InstalledUICulture), viewModel.ActivationValidTill);
        }

        [Test]
        public void ActivationValidTill_WithActivationTillDateMinValue_ReturnsEmptyString()
        {
            _savedActivation = new Activation(true) { ActivatedTill = DateTime.MinValue };

            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.ActivationValidTill);
        }

        [Test]
        public void ActivationValidTill_WithNullActivation_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.ActivationValidTill);
        }

        [Test]
        public void LastActivation_ForDateTimeMinValue_ReturnsEmptyString()
        {
            var activation = new Activation(true);
            var date = DateTime.MinValue;
            activation.TimeOfActivation = date;

            _savedActivation = activation;

            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public async Task OfflineActivationCommand_Executed_RaisesCloseWindowEvent()
        {
            var viewModel = BuildViewModel();
            var wasCalled = false;
            const string expectedOfflineLsa = "OfflineLsa";
            _offlineActivator.ValidateOfflineActivationString(expectedOfflineLsa).Returns(BuildValidActivation("KEY").Some<Activation, LicenseError>());

            _interactionRequest.RegisterInteractionHandler<OfflineActivationInteraction>(offlineActivationInteraction =>
            {
                offlineActivationInteraction.Success = true;
                offlineActivationInteraction.LicenseServerAnswer = expectedOfflineLsa;
            });

            viewModel.CloseLicenseWindowEvent += (s, e) =>
                {
                    wasCalled = true;
                };

            await viewModel.OfflineActivationAsyncCommand.ExecuteAsync(null);
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public async Task OfflineActivationCommand_Execute_OfflineActivatorThrowsSecurityException_ExceptionGetsCaught()
        {
            var viewModel = BuildViewModel();

            _offlineActivator.When(x => x.SaveActivation(Arg.Any<Activation>()))
                .Do(x => { throw new SecurityException(); });

            _offlineActivator.ValidateOfflineActivationString(Arg.Any<String>()).Returns(BuildValidActivation("").Some<Activation, LicenseError>());

            _interactionRequest.RegisterInteractionHandler<OfflineActivationInteraction>(offlineActivationInteraction =>
            {
                offlineActivationInteraction.Success = true;
                offlineActivationInteraction.LicenseServerAnswer = "OfflineLsa";
            });

            await viewModel.OfflineActivationAsyncCommand.ExecuteAsync(null);
        }

        [Test]
        public void LastActivation_ReturnsActivatedTillAsStringInInstalledUICulture()
        {
            _savedActivation = new Activation(true);

            var date = DateTime.Now;
            _savedActivation.TimeOfActivation = date;

            var viewModel = BuildViewModel();

            Assert.AreEqual(date.ToString(CultureInfo.InstalledUICulture), viewModel.LastActivationTime);
        }

        [Test]
        public void LastActivation_WithNullActivation_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public void LastActivation_WithTimeOfActivationOnDateMinValue_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LastActivationTime);
        }

        [Test]
        public void LicenseExpired_GetLicenseStatusTextAndViewStatus_LicenseStatusExpired()
        {
            var propName = nameof(LicenseSettingsTranslation.LicenseStatusLicenseExpired);
            var status = LicenseStatusForView.Invalid;

            var activation = new Activation(false);
            activation.SetResult(Result.OK, $"{Result.OK}:{propName}:{status}");
            activation.ActivatedTill = DateTime.Now.AddDays(60);
            activation.LicenseExpires = DateTime.Now.AddMinutes(-60);
            activation.LicenseType = LicenseType.NON_PERPETUAL;

            _licenseChecker.GetSavedActivation().Returns(x => activation.SomeNotNull(LicenseError.NoActivation));

            var viewModel = BuildViewModel();

            var licenseText = viewModel.LicenseStatusText;
            var viewStatus = viewModel.LicenseStatusForView;

            var translation = typeof(LicenseSettingsTranslation).GetProperty(propName).GetValue(viewModel.Translation);

            Assert.IsTrue(translation.Equals(licenseText));
            Assert.IsTrue(viewStatus.Equals(status));
        }

        [Test]
        public void LicenseExpiredButPerpetualLicense_GetLicenseStatusTextAndViewStatus_ReturnsProperTranslation()
        {
            var propName = nameof(LicenseSettingsTranslation.LicenseStatusValidForVersionButLicenseExpired);
            var status = LicenseStatusForView.Invalid;

            var activation = new Activation(false);
            activation.SetResult(Result.OK, $"{Result.OK}:{propName}:{status}");
            activation.ActivatedTill = DateTime.Now.AddDays(60);
            activation.LicenseExpires = DateTime.Now.AddMinutes(-60);
            activation.LicenseType = LicenseType.PERPETUAL;

            _licenseChecker.GetSavedActivation().Returns(x => activation.SomeNotNull(LicenseError.NoActivation));

            var viewModel = BuildViewModel();

            var licenseText = viewModel.LicenseStatusText;
            var viewStatus = viewModel.LicenseStatusForView;

            var translation = typeof(LicenseSettingsTranslation).GetProperty(propName).GetValue(viewModel.Translation);

            Assert.IsTrue(translation.Equals(licenseText));
            Assert.IsTrue(viewStatus.Equals(status));
        }

        [Test]
        public void LicenseExpiryDateString_WithActivationNull_ReturnsEmptyString()
        {
            _savedActivation = null;

            var viewModel = BuildViewModel();

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLifetimeLicense_ReturnsNever()
        {
            _savedActivation = new Activation(true);
            _savedActivation.LicenseExpires = new DateTime(2038, 01, 01);

            var viewModel = BuildViewModel();

            Assert.AreEqual(_translation.Never, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseExpiryDateString_WithLimitedLicense_ReturnsCorrectDate()
        {
            _savedActivation = new Activation(true);
            _savedActivation.LicenseExpires = DateTime.Now;

            var viewModel = BuildViewModel();

            Assert.AreEqual(DateTime.Today, DateTime.Parse(viewModel.LicenseExpiryDate));
        }

        [Test]
        public void LicenseExpiryDateString_WithMinExpiryDate_ReturnsEmptyString()
        {
            _savedActivation = new Activation(true);
            _savedActivation.LicenseExpires = DateTime.MinValue;

            var viewModel = BuildViewModel();

            Assert.AreEqual(string.Empty, viewModel.LicenseExpiryDate);
        }

        [Test]
        public void LicenseKeyString_WithExpectedLength_IsFormattedWithDashes()
        {
            _savedActivation = new Activation(true);
            _savedActivation.Key = "AAAAABBBBBCCCCCDDDDDEEEEEFFFFF";

            var viewModel = BuildViewModel();

            Assert.AreEqual(ValidLicenseKey_Normalized, viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKeyString_WithNullStringKey_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();

            Assert.AreEqual("", viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKeyString_WithUnexpectedLength_GetsNormalized()
        {
            _savedActivation = new Activation(true);
            _savedActivation.Key = "UNeXPECtEDLENgTH";

            var viewModel = BuildViewModel();

            Assert.AreEqual("UNEXP-ECTED-LENGT-H", viewModel.LicenseKey);
        }

        [Test]
        public void LicenseStatus_ReturnsStatusFromEdition()
        {
            _savedActivation = BuildBlockedActivation("Blocked ActivationKey");

            var viewModel = BuildViewModel();

            Assert.AreEqual(LicenseStatusForView.Invalid, viewModel.LicenseStatusForView);
            Assert.AreEqual(_translation.LicenseStatusBlocked, viewModel.LicenseStatusText);
        }

        [Test]
        public void ManageLicenseCommandExecute_StartsProcessWithLicenseServerUrl()
        {
            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.ManageLicensesCommand.CanExecute(null));

            viewModel.ManageLicensesCommand.Execute(null);
            _process.Received().Start(Urls.LicenseServerUrl);
        }

        [Test]
        public void OfflineAActivationCommand_WithProductAndActivation_IsExecutable()
        {
            _savedActivation = BuildValidActivation(ValidLicenseKey);

            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.OfflineActivationAsyncCommand.CanExecute(null));
        }

        [Test]
        public async Task OfflineActivationCommandExecute_LicenseCheckerOfflineActivationStringFromLicenseServer_GetsCalledWithExpectedOfflineLsa()
        {
            _savedActivation = null;

            var viewModel = BuildViewModel();

            const string expectedOfflineLsa = "OfflineLsa";

            _interactionRequest.RegisterInteractionHandler<OfflineActivationInteraction>(offlineActivationInteraction =>
            {
                offlineActivationInteraction.Success = true;
                offlineActivationInteraction.LicenseServerAnswer = expectedOfflineLsa;
            });

            await viewModel.OfflineActivationAsyncCommand.ExecuteAsync(null);

            _offlineActivator.Received().ValidateOfflineActivationString(expectedOfflineLsa);
        }

        [Test]
        public async Task OfflineActivationCommandExecute_OfflineActivationInteractionContainsLicenseKeyWithDashes_KeyGetsNormalized()
        {
            _savedActivation = new Activation(true) { Key = ValidLicenseKey_Normalized };

            var viewModel = BuildViewModel();

            _interactionRequest.RegisterInteractionHandler<OfflineActivationInteraction>(offlineActivationInteraction =>
            {
                Assert.AreEqual(ValidLicenseKey_Normalized, offlineActivationInteraction.LicenseKey);
            });

            await viewModel.OfflineActivationAsyncCommand.ExecuteAsync(null);
        }

        [Test]
        public async Task OfflineActivationCommandExecute_UserCancelsOfflineActivation_ActivationHelper()
        {
            var viewModel = BuildViewModel();

            _interactionRequest.RegisterInteractionHandler<OfflineActivationInteraction>(offlineActivationInteraction =>
            {
                offlineActivationInteraction.Success = false;
            });

            await viewModel.OfflineActivationAsyncCommand.ExecuteAsync(null);

            _offlineActivator.DidNotReceive().ActivateOfflineActivationString(Arg.Any<string>());
            _licenseChecker.DidNotReceive().SaveActivation(Arg.Any<Activation>());
        }

        [Test]
        public async Task OfflineActivatorIsNull_ExecuteOfflineActivationCommandExecute_ValidateOfflineActivationStringDoesntGetCalled()
        {
            var called = false;

            _interactionRequest.RegisterInteractionHandler<OfflineActivationInteraction>(offlineActivationInteraction => called = true);

            _offlineActivator = null;
            var viewModel = BuildViewModel();
            await viewModel.OfflineActivationAsyncCommand.ExecuteAsync(null);
            Assert.IsFalse(called);
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsNotValid_LicenseCheckerActivationIsNotValid_DoNotSaveNewEditionAndInformUser()
        {
            _savedActivation = null;

            _expectedLicenseKey = "given-key";

            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);

            _activationFromServer = new Activation(true);
            _activationFromServer.Key = _expectedLicenseKey.Replace("-", "");
            _activationFromServer.SetResult(Result.LICENSE_EXPIRED, "Expired");

            var viewModel = BuildViewModel();

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.DidNotReceive().SaveActivation(Arg.Any<Activation>());

            var messageInteraction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.ActivationFailed, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsNotValid_LicenseCheckerActivationIsValid_SaveNewActivationAndStoreLicenseForAllUsersQuery()
        {
            _expectedLicenseKey = "not empty";
            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);
            _licenseChecker.ActivateWithoutSaving(_expectedLicenseKey).Returns(x => BuildValidActivation(_expectedLicenseKey).Some<Activation, LicenseError>());

            var viewModel = BuildViewModel();

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _interactionRequest.AssertWasRaised<StoreLicenseForAllUsersInteraction>();
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsNotValid_LicenseCheckerActivationIsValid_SavesActivation()
        {
            _savedActivation = null;
            _expectedLicenseKey = "given-key";

            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);

            _activationFromServer = BuildValidActivation(_expectedLicenseKey);
            var viewModel = BuildViewModel();

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.Received().SaveActivation(_activationFromServer);
            Assert.AreEqual(_expectedLicenseKey.ToUpper(), viewModel.LicenseKey);
        }

        [Test]
        public async Task OnlineActivationCommand_LicenseCheckerActivationIsValid_CallsCloseLicenseWindowEvent()
        {
            var wasCalled = false;
            _savedActivation = null;
            _expectedLicenseKey = "given-key";

            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);

            _activationFromServer = BuildValidActivation(_expectedLicenseKey);
            var viewModel = BuildViewModel();

            viewModel.CloseLicenseWindowEvent += (s, e) =>
            {
                wasCalled = true;
            };

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsValid_ActivationWithNewKeyIsBlocked_DoNotSaveNewActivation()
        {
            _savedActivation = BuildValidActivation("saved activation key");
            _activationFromServer = BuildBlockedActivation("Not the saved activation key");

            CreateLicenseKeyEnteredInteraction("not null or empty");

            var viewModel = BuildViewModel();

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.DidNotReceive().SaveActivation(_activationFromServer);
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsValid_LicenseCheckerActivationIsNotValid_DoNotSaveNewActivationAndInformUser()
        {
            _savedActivation = null;

            _expectedLicenseKey = "not empty";
            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);

            var viewModel = BuildViewModel();

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.DidNotReceive().SaveActivation(Arg.Any<Activation>());

            var messageInteraction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.ActivationFailed, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsValid_LicenseCheckerActivationIsValid_SaveNewActivationAndStoreLicenseForAllUsersQuery()
        {
            _expectedLicenseKey = "not empty";

            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);

            _activationFromServer = BuildValidActivation(_expectedLicenseKey);

            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.Received().SaveActivation(_activationFromServer);
            _interactionRequest.AssertWasRaised<StoreLicenseForAllUsersInteraction>();
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsValid_LicenseCheckerActivationIsValid_ShareLicenseForAllUsersDisabled__SaveLicenseAndInformUser()
        {
            _expectedLicenseKey = "not empty";
            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);

            _activationFromServer = BuildValidActivation(_expectedLicenseKey);

            var viewModel = BuildViewModel();

            viewModel.ShareLicenseForAllUsersEnabled = false; //Important for this test!!!!

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.Received().SaveActivation(_activationFromServer);
            _interactionRequest.AssertWasNotRaised<StoreLicenseForAllUsersInteraction>();

            var messageInteraction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.ActivationSuccessful, messageInteraction.Title);
            Assert.AreEqual(_translation.ActivationSuccessfulMessage, messageInteraction.Text);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.PDFForge, messageInteraction.Icon);
        }

        [Test]
        public async Task OnlineActivationCommand_CurrentActivationIsValid_ReActivationBlocksCurrentKey_SaveNewActivationAndInformUser()
        {
            _savedActivation = BuildValidActivation("saved activation key");
            _activationFromServer = new Activation(true);
            _activationFromServer.Key = "saved activation key";
            _activationFromServer.SetResult(Result.BLOCKED, "Blocked");

            CreateLicenseKeyEnteredInteraction("not null or empty");
            var viewModel = BuildViewModel();

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.Received().SaveActivation(_activationFromServer);
            var messageInteraction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.ActivationFailed, messageInteraction.Title);
            Assert.AreEqual(MessageOptions.OK, messageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public async Task OnlineActivationCommand_WhenExecuted_CallsLicenseChecker()
        {
            _expectedLicenseKey = "ABCDEF";
            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);

            var viewModel = BuildViewModel();
            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.Received().ActivateWithoutSaving(_expectedLicenseKey);
        }

        [Test]
        public async Task OnlineActivationCommand_WhenExecuted_RaisesPropertyChanged()
        {
            _expectedLicenseKey = "ABCDEF";
            CreateLicenseKeyEnteredInteraction(_expectedLicenseKey);
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            Assert.Contains(nameof(viewModel.LicenseKey), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatusForView), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatusText), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseExpiryDate), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LastActivationTime), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.ActivationValidTill), propertyChangedEvents);
        }

        [Test]
        public async Task OnlineActivationCommand_WhenKeyIsNull_DoesNotCallLicenseChecker()
        {
            CreateCancelledInputInteraction();

            var viewModel = BuildViewModel();

            await viewModel.OnlineActivationAsyncCommand.ExecuteAsync(null);

            _licenseChecker.DidNotReceiveWithAnyArgs().ActivateWithoutSaving("");
        }

        [Test]
        public void OnlineActivationCommand_WithProductAndActivation_IsExecutable()
        {
            _savedActivation = BuildValidActivation(ValidLicenseKey);

            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.OnlineActivationAsyncCommand.CanExecute(null));
        }

        [Test]
        public void SetTranslation_RaiseTranslationAndLicenseStatusTextPropertyChanged()
        {
            var viewModel = BuildViewModel();
            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);

            viewModel.Translation = new LicenseSettingsTranslation();

            Assert.Contains(nameof(viewModel.Translation), propertyChangedEvents);
            Assert.Contains(nameof(viewModel.LicenseStatusText), propertyChangedEvents);
        }

        [Test]
        public void SetupViewModel_RequestShowOfflineActivation_ReturnsTrue()
        {
            var viewModel = BuildViewModel();
            Assert.IsTrue(viewModel.ShowOfflineActivation);
        }

        [Test]
        public void SetupViewModel_RequestShowOnlineActivation_ReturnsTrue()
        {
            var viewModel = BuildViewModel();
            Assert.IsTrue(viewModel.ShowOnlineActivation);
        }

        [Test]
        public void SetupViewModelInvalidActivation_RequestLicensee_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();
            var viewModelLicensee = viewModel.Licensee;
            Assert.IsTrue(string.IsNullOrWhiteSpace(viewModelLicensee));
        }

        [Test]
        public void SetupViewModelInvalidMachineId_RequestMachineId_ReturnsEmptyString()
        {
            var viewModel = BuildViewModel();
            var viewModelLicensee = viewModel.Licensee;
            Assert.IsTrue(string.IsNullOrWhiteSpace(viewModelLicensee));
        }

        [Test]
        public void SetupViewModelValidActivation_RequestLicensee_ReturnsLicenseeString()
        {
            var activation = new Activation(false);
            activation.SetResult(Result.OK, $"");

            var activationLicensee = "activationLicensee";
            activation.Licensee = activationLicensee;

            _licenseChecker.GetSavedActivation().Returns(x => activation.SomeNotNull(LicenseError.NoActivation));

            var viewModel = BuildViewModel();
            Assert.IsTrue(activationLicensee.Equals(viewModel.Licensee));
        }

        [Test]
        public void SetupViewModelValidMachineId_RequestMachineId_ReturnsMaschineId()
        {
            var activation = new Activation(false);
            activation.SetResult(Result.OK, $"");

            var machineId = "machineId";
            activation.MachineId = machineId;

            _licenseChecker.GetSavedActivation().Returns(x => activation.SomeNotNull(LicenseError.NoActivation));

            var viewModel = BuildViewModel();
            Assert.IsTrue(machineId.Equals(viewModel.MachineId));
        }

        [Test]
        public void ThrowErrorOnProcessStart_StartProcesViaManageLicensesCommand_CaptureError()
        {
            var viewModel = BuildViewModel();
            _process.When(starter => starter
                    .Start(Arg.Any<string>()))
                .Do(x => { throw new Exception("Should be Caught"); });

            Assert.DoesNotThrow(() => viewModel.ManageLicensesCommand.Execute(null));
        }

        [Test]
        public void ValidActivation_GetLicenseStatusText_LicenseStatusValid()
        {
            var activation = new Activation(false);
            activation.ActivatedTill = DateTime.Now.AddMinutes(60);
            activation.LicenseExpires = DateTime.Now.AddMinutes(60);
            activation.SetResult(Result.OK, "All A OK");

            _licenseChecker.GetSavedActivation().Returns(x => activation.SomeNotNull(LicenseError.NoActivation));

            var viewModel = BuildViewModel();

            var licenseText = viewModel.LicenseStatusText;
            var translationLicenseStatusValid = viewModel.Translation.LicenseStatusValid;

            Assert.IsTrue(licenseText.Equals(translationLicenseStatusValid));
        }

        [Test]
        public void ValidationExpired_GetLicenseStatusText_ActivationStatusExpired()
        {
            var propName = nameof(LicenseSettingsTranslation.LicenseStatusActivationExpired);
            var status = LicenseStatusForView.Invalid;

            //setup special Condition
            var activation = new Activation(false) { ActivatedTill = DateTime.Now.Subtract(TimeSpan.FromMinutes(60)) };

            activation.SetResult(Result.OK, $"{Result.OK}:{propName}:{status}");

            _licenseChecker.GetSavedActivation().Returns(x => activation.SomeNotNull(LicenseError.NoActivation));

            var viewModel = BuildViewModel();

            var licenseText = viewModel.LicenseStatusText;
            var viewStatus = viewModel.LicenseStatusForView;

            var translation = typeof(LicenseSettingsTranslation).GetProperty(propName).GetValue(viewModel.Translation);

            Assert.IsTrue(translation.Equals(licenseText));
            Assert.IsTrue(viewStatus.Equals(status));
        }
    }
}
