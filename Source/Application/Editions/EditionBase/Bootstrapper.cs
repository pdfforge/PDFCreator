using Microsoft.Win32;
using pdfforge.DataStorage;
using pdfforge.Mail;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Helper;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Dropbox;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.Conversion;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.JobInfoQueue;
using pdfforge.PDFCreator.Core.Printing;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Cache;
using pdfforge.PDFCreator.Core.Services.Download;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Editions.EditionBase.CreatorTab;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Banner;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Converter;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.UI.Presentation.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Architect;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Home;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Encryption;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Printer;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.ProfessionalHintStep;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Attachment;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Cover;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Encryption;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.ForwardToOtherProfile;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Dropbox;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.FTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.HTTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailClient;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Print;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.RunProgram;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.UI.Presentation.Windows.Startup;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.UI.Presentation.WorkflowQuery;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using pdfforge.PDFCreator.UI.PrismHelper;
using pdfforge.PDFCreator.UI.PrismHelper.Prism.SimpleInjector;
using pdfforge.PDFCreator.UI.PrismHelper.Tab;
using pdfforge.PDFCreator.UI.RssFeed;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Pdf;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Registry;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.UserGuide;
using pdfforge.PDFCreator.Utilities.Web;
using pdfforge.UsageStatistics;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;
using SystemWrapper;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.Views.Windows;
using Translatable;
using FtpAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.FtpAccountView;
using InputBoxUserControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Dialogs.InputBoxUserControl;
using LicenseUpdateControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License.LicenseUpdateControl;
using ManagePrintJobsWindow = pdfforge.PDFCreator.UI.Presentation.Windows.ManagePrintJobsWindow;
using PrintJobShell = pdfforge.PDFCreator.UI.Presentation.PrintJobShell;
using SmtpAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.SmtpAccountView;
using StoreLicenseForAllUsersControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License.StoreLicenseForAllUsersControl;
using WorkflowEditorTestPageUserControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings.WorkflowEditorTestPageUserControl;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public abstract class Bootstrapper
    {
        private WorkflowFactory _workflowFactory;

        protected abstract string EditionName { get; }
        protected abstract Color EditionHighlightColor { get; }
        protected abstract bool HideLicensing { get; }

        protected abstract EditionHelper EditionHelper { get; }

        public void RegisterMainApplication(Container container)
        {
            container.Options.SuppressLifestyleMismatchVerification = true;

            RegisterActivationHelper(container);
            container.RegisterSingleton(() => new ApplicationNameProvider(EditionName));
            container.RegisterSingleton<IHightlightColorRegistration>(() => new HightlightColorRegistration(EditionHighlightColor));
            container.RegisterSingleton(() => new LicenseOptionProvider(HideLicensing));
            container.RegisterSingleton(() => EditionHelper);
            container.RegisterSingleton<DropboxAppData>(() => new DropboxAppData(Data.Decrypt(DropboxAppKey.Encrypted_DropboxAppKey)));
            container.RegisterSingleton<IDropboxTokenCache, DropboxTokenCache>();

            var regPath = @"Software\pdfforge\PDFCreator";
            container.RegisterSingleton<IInstallationPathProvider>(() => new InstallationPathProvider(regPath, regPath + @"\Settings", "{0001B4FD-9EA3-4D90-A79E-FD14BA3AB01D}", RegistryHive.CurrentUser));

            _workflowFactory = new WorkflowFactory(container);
            container.Register<AutoSaveWorkflow>(); // Workflow is registered so all dependencies can be verified in test
            container.Register<InteractiveWorkflow>(); // Workflow is registered so all dependencies can be verified in test

            container.RegisterSingleton<ICommandLocator>(() => new CommandLocator(container));

            container.Register<IWorkflowFactory>(() => _workflowFactory);

            container.Register<IOutputFileMover, AutosaveOutputFileMover>();

            container.Register<ITargetFilePathComposer, TargetFilePathComposer>();
            container.Register<IErrorNotifier, ErrorNotifierInteractive>();
            container.Register<IInteractiveProfileChecker, InteractiveProfileChecker>();
            container.Register<IInteractiveFileExistsChecker, InteractiveFileExistsChecker>();

            container.RegisterSingleton<IRecommendArchitectAssistant, RecommendArchitectAssistant>();
            container.Register<IStartupActionHandler, StartupActionHandler>();
            container.RegisterSingleton<IInteractionInvoker, InteractionInvoker>();

            RegisterInteractionRequestPerThread(container);

            container.Register<ISoundPlayer, SoundPlayer>();
            container.RegisterSingleton<IWpfTopMostHelper, WpfTopMostHelper>();
            container.Register<ISmtpTest, SmtpTestMailAssistant>();
            container.Register<IClientTestMailAssistant, ClientTestMailAssistant>();
            container.Register<IMailHelper, MailHelper>();
            container.Register<ITestFileDummyHelper, TestFileDummyHelper>();
            container.RegisterSingleton<ITempDirectoryHelper, TempDirectoryHelper>();

            container.Register<IActionManager, ActionManager>();

            container.Register<ICommandLineUtil, CommandLineUtil>();
            container.Register<IPrinterWrapper, PrinterWrapper>();
            container.Register<IJobPrinter, GhostScriptPrinter>();

            container.Register<IJobDataUpdater, JobDataUpdater>();

            container.Register<IPageNumberCalculator, PageNumberCalculator>();

            container.Register<IJobRunner, JobRunner>();
            container.Register<IActionExecutor, ActionExecutor>();
            container.Register<IConverterFactory, ConverterFactory>();
            container.Register<IPsConverterFactory, GhostscriptConverterFactory>();

            container.RegisterSingleton<IUpdateDownloader, UpdateDownloader>();

            container.RegisterSingleton<IFileCacheFactory, FileCacheFactory>();
            container.RegisterSingleton<IDownloader, WebClientDownloader>();

            container.Register<IJobCleanUp, JobCleanUp>();

            container.RegisterSingleton<ISystemPrinterProvider, SystemPrinterProvider>();

            container.Register<IDirectConversion, DirectConversion>();
            container.Register<IDirectConversionHelper, DirectConversionHelper>();
            container.Register<IDirectConversionInfFileHelper, DirectConversionInfFileHelper>();
            container.RegisterSingleton<IFileConversionAssistant, FileConversionAssistant>();
            container.Register<IPrintFileHelper, PrintFileAssistant>();
            container.Register<IUacAssistant, UacAssistant>();
            container.Register<ITestPageHelper, TestPageHelper>();
            container.RegisterSingleton<IPdfArchitectCheck, PdfArchitectCheck>();
            container.Register<IGhostscriptDiscovery, GhostscriptDiscovery>();
            container.Register<ProfileRemoveCommand>();
            container.Register<ProfileRenameCommand>();
            container.Register<IPrintTestPageAsyncCommand, PrintTestpageAsyncCommand>();

            container.RegisterSingleton<IRegistry, RegistryWrap>();
            container.RegisterSingleton<IFile, FileWrap>();
            container.RegisterSingleton<IDirectory, DirectoryWrap>();
            container.RegisterSingleton<IPath, PathWrap>();
            container.RegisterSingleton<IPathUtil, PathUtil>();
            container.RegisterSingleton<IEnvironment, EnvironmentWrap>();
            container.RegisterSingleton<IDirectoryAccessControl, DirectoryAccessControl>();
            container.RegisterSingleton<IDirectoryHelper, DirectoryHelper>();
            container.RegisterSingleton<IReadableFileSizeFormatter, ReadableReadableFileSizeFormatter>();

            container.RegisterSingleton<IProcessStarter, ProcessStarter>();

            container.RegisterSingleton<IDateTimeProvider, DateTimeProvider>();

            container.RegisterSingleton<IAssemblyHelper>(() => new AssemblyHelper(GetType().Assembly));
            container.RegisterSingleton<IProgramDataDirectoryHelper>(() => new ProgramDataDirectoryHelper("PDFCreator"));
            container.RegisterSingleton<IOsHelper, OsHelper>();
            container.Register<IFontHelper, FontHelper>();
            container.Register<IOpenFileInteractionHelper, OpenFileInteractionHelper>();
            container.Register<IPDFCreatorNameProvider, PDFCreatorNameProvider>();
            container.Register<ITokenButtonFunctionProvider, TokenButtonFunctionProvider>();

            container.RegisterSingleton<IJobInfoQueueManager, JobInfoQueueManager>();
            container.Register<IJobInfoQueue, JobInfoQueue>(Lifestyle.Singleton);
            container.Register<IThreadManager, ThreadManager>(Lifestyle.Singleton);
            container.Register<IPipeServerManager, PipeServerManager>(Lifestyle.Singleton);
            container.RegisterSingleton<ITranslationUpdater, TranslationUpdater>();
            container.RegisterSingleton<IPipeMessageHandler, NewPipeJobHandler>();

            container.RegisterSingleton<IVersionHelper>(() => new VersionHelper(GetType().Assembly));
            container.Register<ITerminalServerDetection, TerminalServerDetection>();

            container.Register<IRepairSpoolFolderAssistant, RepairSpoolFolderAssistant>();
            container.Register<ISpoolFolderAccess, SpoolFolderAccess>();
            container.Register<IShellExecuteHelper, ShellExecuteHelper>();
            container.RegisterSingleton<IPrinterPortReader, PrinterPortReader>();
            RegisterPrinterHelper(container);

            container.Register<IFileAssoc, FileAssoc>();
            container.RegisterSingleton<IHashUtil, HashUtil>();
            container.RegisterSingleton<ISignaturePasswordCheck, SignaturePasswordCheckCached>();

            container.Register<IWelcomeSettingsHelper, WelcomeSettingsHelper>();

            container.Register<IEmailClientFactory, EmailClientFactory>();
            container.Register<IProfileChecker, ProfileChecker>();
            container.Register<IDefaultViewerCheck, DefaultViewerCheck>();

            container.Collection.Register<ISettingsNavigationCheck>(new[]
        {
                typeof(NavigateProfileCheck),
                typeof(NavigateDefaultViewerCheck)
            });

            container.Register<ITabSwitchSettingsCheck, TabSwitchSettingsCheck>();
            container.Register<EvaluateSavingRelevantSettingsAndNotifyUserCommand>();
            container.Register<EvaluateTabSwitchRelevantSettingsAndNotifyUserCommand>();

            container.Register<IAppSettingsChecker, AppSettingsChecker>();
            container.Register<IPrinterActionsAssistant, PrinterActionsAssistant>();
            container.Register<IRepairPrinterAssistant, RepairPrinterAssistant>();
            container.Register<IDispatcher, DispatcherWrapper>();
            container.RegisterSingleton<ISettingsMover, SettingsMover>();
            container.RegisterSingleton<IRegistryUtility, RegistryUtility>();
            container.RegisterSingleton<ITokenHelper, TokenHelper>();
            container.RegisterSingleton<ICancellationTokenSourceFactory, CancellationTokenSourceFactory>();
            container.Register<IMaybePipedApplicationStarter, MaybePipedApplicationStarter>();
            container.Register<ITokenReplacerFactory, TokenReplacerFactory>();

            container.RegisterSingleton<ISettingsChanged, SettingsChanged>();

            container.RegisterSingleton<IJobHistoryStorage, JobHistoryJsonFileStorage>();
            container.RegisterSingleton<IJobHistoryActiveRecord, JobHistoryActiveRecord>();

            container.RegisterSingleton<IDefaultSettingsBuilder, PDFCreatorDefaultSettingsBuilder>();
            container.RegisterInitializer<PDFCreatorDefaultSettingsBuilder>(x =>
            {
                x.WithEmailSignature = EditionHelper.IsFreeEdition;
                x.EncryptionLevel = EditionHelper.EncryptionLevel;
            });

            container.RegisterSingleton<ISettingsBackup, SettingsBackup>();
            container.RegisterSingleton<IMigrationStorageFactory>(() =>
                new MigrationStorageFactory((baseStorage, targetVersion, settingsBackup) => new CreatorSettingsMigrationStorage(baseStorage, container.GetInstance<IFontHelper>(), targetVersion, settingsBackup)));

            container.Register<IIniSettingsAssistant, CreatorIniSettingsAssistant>();
            container.RegisterSingleton<IIniSettingsLoader, IniSettingsLoader>();
            container.RegisterSingleton<IActionOrderChecker, ActionOrderChecker>();
            container.RegisterInitializer<IIniSettingsLoader>(loader => loader.SettingsVersion = (new CreatorAppSettings()).SettingsVersion);
            container.RegisterSingleton<IDataStorageFactory, DataStorageFactory>();
            container.RegisterSingleton<IJobInfoManager, JobInfoManager>();
            container.Register<IStaticPropertiesHack, StaticPropertiesHack>();

            container.Register<IManagePrintJobExceptionHandler, ManagePrintJobExceptionHandler>();
            container.Register<IFolderCleaner, FolderCleaner>();
            container.Register<IPdfCreatorFolderCleanUp, PdfCreatorFolderCleanUp>();

            container.Register<ISpooledJobFinder, SpooledJobFinder>();

            container.Register<IFtpConnectionFactory, FtpConnectionFactory>();
            container.Register<IScriptActionHelper, ScriptAction>();

            container.Register<IWorkflowNavigationHelper, WorkflowNavigationHelper>();

            container.Register<IDropboxService, DropboxService>();
            container.Register<IClipboardService, ClipboardService>();
            container.Register<IWinInetHelper, WinInetHelper>();
            container.RegisterSingleton<ITitleReplacerProvider, SettingsTitleReplacerProvider>();
            container.RegisterSingleton<IUpdateChangeParser, UpdateChangeParser>();

            container.RegisterSingleton<IMainWindowThreadLauncher, MainShellLauncher>();
            container.Register<ILastSaveDirectoryHelper, LastSaveDirectoryHelper>();
            container.Register<ITokenViewModelFactory, TokenViewModelFactory>();

            container.RegisterSingleton<IRegionHelper, RegionHelper>();
            container.Register<ISaveChangedSettingsCommand, SaveChangedSettingsCommand>();
            container.RegisterInitializer<FtpAccountViewModel>(model => model.AllowConversionInterrupts = true);
            container.RegisterInitializer<HttpAccountViewModel>(model => model.AllowConversionInterrupts = true);
            container.RegisterInitializer<SmtpAccountViewModel>(model => model.AllowConversionInterrupts = true);
            container.RegisterInitializer<PrintUserControlViewModel>(model => model.PrinterDialogOptionEnabled = true);
            container.RegisterSingleton<IWorkflowEditorSubViewProvider>(() => new WorkflowEditorSubViewProvider(nameof(SaveView), nameof(MetadataView), nameof(OutputFormatUserControl)));
            container.RegisterSingleton<IGpoSettings>(GetGpoSettings);
            container.Register<UsageStatisticsViewModelBase, PdfCreatorUsageStatisticsViewModel>();
            container.RegisterSingleton<ISigningPositionToUnitConverterFactory, SigningPositionToUnitConverterFactory>();
            container.RegisterSingleton<ZxcvbnProvider>();

            container.Register<IChangeJobCheckAndProceedCommandBuilder, ChangeJobCheckAndProceedCommandBuilder>();
            container.Register<IBrowseFileCommandBuilder, BrowseFileCommandBuilder>();

            container.Register<IPdfVersionHelper, PdfVersionHelper>();
            container.Register<IFontPathHelper, FontPathHelper>();

            container.Register<IJobFolderBuilder, JobFolderBuilder>();
            container.Register<IJobInfoDuplicator, JobInfoDuplicator>();
            container.Register<ISourceFileInfoDuplicator, SourceFileInfoDuplicator>();
            container.Register<IUniqueFilenameFactory, UniqueFilenameFactory>();
            container.Register<IUniqueDirectory, UniqueDirectory>();
            container.Register<IDeleteTempFolderCommand, DeleteTempFolderCommand>();

            container.Register<ILicenseExpirationReminder, LicenseExpirationReminder>();

            container.Register<ICommandBuilderProvider, CommandBuilderProvider>();
            container.Register<ISelectFilesUserControlViewModelFactory, SelectFilesUserControlViewModelFactory>();

            container.Register<IDropboxHttpListener, DropboxHttpListener>();
            container.Register<IDropboxCodeExchanger, DropboxCodeExchanger>();
            container.Register<IDropboxUserInfoManager, DropboxUserInfoManager>();

            RegisterSettingsLoader(container);
            RegisterCurrentSettingsProvider(container);
            RegisterFolderProvider(container);
            RegisterUserGuideHelper(container);
            RegisterTranslator(container);
            RegisterMailSignatureHelper(container);
            RegisterParameterSettingsManager(container);
            RegisterSettingsHelper(container);
            RegisterStartupConditions(container);
            RegisterActions(container);
            RegisterFileNameQuery(container);
            RegisterUpdateAssistant(container);
            RegisterJobBuilder(container);
            RegisterInteractiveWorkflowManagerFactory(container);
            RegisterPdfProcessor(container);
            RegisterUserTokenExtractor(container);
            RegisterProfessionalHintHelper(container);
            RegisterNotificationService(container);
            RegisterAllTypedSettingsProvider(container);
            RegisterBannerManager(container);
            RegisterWebLinkLauncher(container);
            RegisterUsageStatistics(container);

            container.RegisterSingleton(BuildCustomization);
            container.RegisterSingleton<IRssHttpClientFactory, RssHttpClientFactory>();
            container.RegisterSingleton<IPdfCreatorUsageStatisticsManager, PdfCreatorUsageStatisticsManager>();

            container.RegisterSingleton<IJobEventsManager, JobEventsManager>();
            container.RegisterSingleton<IRssService, RssService>();
            container.RegisterSingleton<IAppSettingsProvider>(() =>
           {
               var settings = container.GetInstance<ISettingsProvider>();
               return new ApplicationSettingsProvider(() => settings.Settings.ApplicationSettings);
           });

            container.RegisterInitializer<ShellManager>(shellManager =>
            {
                shellManager.SetMainShellRegionToViewRegister(MainShellViewRegister());
                shellManager.SetPrintJobShellRegionToViewRegister(PrintJobShellViewRegister());
            });

            container.RegisterInitializer<StartupNavigationAction>(action =>
            {
                action.Region = RegionNames.MainRegion;
                action.Target = RegionNames.HomeView;
            });

            container.RegisterSingleton<IStartupRoutine>(() =>
            {
                var startupActions = GetStartupActions().Select(x => (IStartupAction)container.GetInstance(x));
                return new StartupRoutine(startupActions);
            });

            container.Collection.Register<IJobEventsHandler>(new[]
            {
                typeof(UsageStatisticsEventsHandler)
            });
        }

        protected List<(string, Type)> MainShellViewRegister()
        {
            return new List<(string, Type)>
            {
                (RegionNames.MainRegion, typeof(HomeView)),
                (RegionNames.HomeViewBannerRegion, typeof(BannerView)),
                (RegionNames.RssFeedRegion, typeof(RssFeedView)),
                (RegionNames.SaveOutputFormatMetadataView, typeof(SaveOutputFormatMetadataView)),
                (RegionNames.WorkflowEditorView, typeof(WorkflowEditorView)),
                (RegionNames.TestButtonWorkflowEditorRegion, typeof(WorkflowEditorTestPageUserControl)),
                (RegionNames.AddActionWorkflowEditorRegion, typeof(AddActionUserControl)),
                (RegionNames.ProfileSaveCancelButtonsRegion, typeof(SaveCancelButtonsControl)),
                (RegionNames.ApplicationSaveCancelButtonsRegion, typeof(SaveCancelButtonsControl))
            };
        }

        protected virtual List<(string, Type)> PrintJobShellViewRegister()
        {
            return new List<(string, Type)>();
        }

        private void RegisterUsageStatistics(Container container)
        {
            container.RegisterSingleton<UsageStatisticsOptions>(() =>
            {
                var version = container.GetInstance<IVersionHelper>()
                    .FormatWithThreeDigits();

                var product = container.GetInstance<ApplicationNameProvider>().ProductIdentifier;

                return new UsageStatisticsOptions(Urls.UsageStatisticsEndpointUrl, product, version);
            });
            container.RegisterSingleton<IUsageStatisticsSender, UsageStatisticsSender>();
            container.RegisterSingleton<IUsageMetricFactory, UsageMetricFactory>();
        }

        private void RegisterWebLinkLauncher(Container container)
        {
            container.Register(TrackingParameterReader.ReadFromRegistry);
            container.Register<IWebLinkLauncher, WebLinkLauncher>();
        }

        private void RegisterInteractionRequestPerThread(Container container)
        {
            var threadMapping = new Dictionary<Thread, IInteractionRequest>();

            container.Register<IInteractionRequest>(() =>
            {
                var thread = Thread.CurrentThread;
                if (threadMapping.ContainsKey(thread))
                    return threadMapping[thread];

                var interactionRequest = new InteractionRequest();
                threadMapping[thread] = interactionRequest;

                return interactionRequest;
            });
        }

        private void RegisterCurrentSettingsProvider(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<CurrentSettingsProvider>(container);
            container.AddRegistration(typeof(ISelectedProfileProvider), registration);
            container.AddRegistration(typeof(ICurrentSettingsProvider), registration);
            container.AddRegistration(typeof(CurrentSettingsProvider), registration);
        }

        private void RegisterAllTypedSettingsProvider(Container container)
        {
            // keep redundancies for overview's sake
            RegisterTypedSettingsProvider<Accounts>(
                settings => settings.ApplicationSettings.Accounts,
                container
                );

            RegisterTypedSettingsProvider<ObservableCollection<PrinterMapping>>(
                settings => settings.ApplicationSettings.PrinterMappings,
                container
                );

            RegisterTypedSettingsProvider<ObservableCollection<TitleReplacement>>(
                settings => settings.ApplicationSettings.TitleReplacement, container
                );

            RegisterTypedSettingsProvider<ObservableCollection<DefaultViewer>>(
                settings => settings.DefaultViewerList,
                container
                );

            RegisterTypedSettingsProvider<ObservableCollection<ConversionProfile>>(
                settings => settings.ConversionProfiles,
                container
            );

            RegisterTypedSettingsProvider<CreatorAppSettings>(
                settings => settings.CreatorAppSettings,
                container
            );

            RegisterTypedSettingsProvider<ApplicationSettings>(
                settings => settings.ApplicationSettings,
                container
            );

            RegisterTypedSettingsProvider<UpdateInterval>(
                settings => settings.ApplicationSettings.UpdateInterval,
                container
            );

            RegisterTypedSettingsProvider<Conversion.Settings.UsageStatistics>(
                settings => settings.ApplicationSettings.UsageStatistics,
                container

            );

            RegisterTypedSettingsProvider<RssFeed>(
                settings => settings.ApplicationSettings.RssFeed,
                container
            );
        }

        private void RegisterTypedSettingsProvider<TTarget>(Expression<Func<PdfCreatorSettings, TTarget>> expression, Container container)
        {
            container.RegisterSingleton<ICurrentSettings<TTarget>>(() => new CreatorCurrentSettings<TTarget>(expression, container.GetInstance<CurrentSettingsProvider>()));
        }

        protected abstract void RegisterSettingsLoader(Container container);

        protected abstract void RegisterUpdateAssistant(Container container);

        protected abstract void RegisterJobBuilder(Container container);

        protected abstract void RegisterInteractiveWorkflowManagerFactory(Container container);

        protected abstract void RegisterActivationHelper(Container container);

        protected abstract void RegisterUserTokenExtractor(Container container);

        protected abstract void RegisterPdfProcessor(Container container);

        protected abstract IGpoSettings GetGpoSettings();

        protected virtual void RegisterMailSignatureHelper(Container container)
        {
            container.Register<IMailSignatureHelper, MailSignatureHelperLicensed>();
        }

        protected virtual void RegisterProfessionalHintHelper(Container container)
        {
            container.Register<IProfessionalHintHelper, ProfessionalHintHelperDisabled>();
        }

        protected virtual ViewCustomization BuildCustomization()
        {
            return ViewCustomization.DefaultCustomization;
        }

        private void RegisterPrinterHelper(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<PrinterHelper>(container);
            container.AddRegistration(typeof(IPrinterProvider), registration);
            container.AddRegistration(typeof(IPrinterHelper), registration);
        }

        private void RegisterFileNameQuery(Container container)
        {
            var registration = Lifestyle.Transient.CreateRegistration<InteractiveFileNameQuery>(container);
            container.AddRegistration(typeof(IFileNameQuery), registration);
            container.AddRegistration(typeof(IRetypeFileNameQuery), registration);
        }

        protected abstract IList<Type> GetStartupConditions(IList<Type> defaultConditions);

        private void RegisterStartupConditions(Container container)
        {
            var defaultConditions = new[]
            {
                typeof(SpoolerRunningCondition),
                typeof(CheckSpoolFolderCondition),
                typeof(GhostscriptCondition),
                typeof(PrinterInstalledCondition)
            }.ToList();

            var conditions = GetStartupConditions(defaultConditions);
            container.Collection.Register<IStartupCondition>(conditions);

            container.Register<ICheckAllStartupConditions, CheckAllStartupConditions>();
        }

        private void RegisterActions(Container container)
        {
            container.Register<ForwardToFurtherProfileActionBase, ForwardToFurtherProfileAction>();
            container.Register<IForwardToFurtherProfileViewModel, ForwardToFurtherProfileViewModel>();

            //Register Actions in default order
            container.Collection.Register<IAction>(new[]
            {
                // preparation
                typeof(UserTokensAction),
                typeof(ForwardToFurtherProfileAction),

                // modification
                typeof(CoverAction),
                typeof(AttachmentAction),
                typeof(BackgroundAction),
                typeof(WatermarkAction),
                typeof(StampAction),
                typeof(EncryptionAction),
                typeof(SigningAction),

                // sending
                typeof(DefaultViewerAction),
                typeof(ScriptAction),
                typeof(PrintingAction),
                typeof(EMailClientAction),
                typeof(SmtpMailAction),
                typeof(DropboxAction),
                typeof(FtpAction),
                typeof(HttpAction),
            });

            container.RegisterSingleton<IActionOrderHelper>(() =>
            {
                var list = container
                    .GetInstance<IEnumerable<IAction>>()
                    .Select(x => x.SettingsType.Name);

                return new ActionOrderHelper(list);
            });

            container.Register<IActionLocator, ActionLocator>();

            // The order determines the order in which the actions will be shown in the UI
            var facades = new[]
            {
                // pre action
                typeof(PresenterActionFacade<UserTokenUserControl>),
                typeof(PresenterActionFacade<ForwardToFurtherProfileView>),

                // modify action
                typeof(PresenterActionFacade<CoverUserControl>),
                typeof(PresenterActionFacade<AttachmentUserControl>),
                typeof(PresenterActionFacade<StampUserControl>),
                typeof(PresenterActionFacade<BackgroundUserControl>),
                typeof(PresenterActionFacade<WatermarkView>),
                typeof(PresenterActionFacade<EncryptionUserControl>),
                typeof(PresenterActionFacade<SigningUserControl>),

                // send action
                typeof(PresenterActionFacade<OpenViewerActionUserControl>),
                typeof(PresenterActionFacade<MailClientUserControl>),
                typeof(PresenterActionFacade<PrintUserControl>),
                typeof(PresenterActionFacade<ScriptUserControl>),
                typeof(PresenterActionFacade<FTPActionUserControl>),
                typeof(PresenterActionFacade<SmtpActionView>),
                typeof(PresenterActionFacade<HttpActionUserControl>),
                typeof(PresenterActionFacade<DropboxUserControl>),
            };

            container.Collection.Register<IPresenterActionFacade>(facades);

            foreach (var facadeType in facades)
            {
                var viewType = facadeType.GenericTypeArguments[0];
                container.RegisterTypeForNavigation(viewType);
            }

            container.Register<ISmtpMailAction, SmtpMailAction>();
            container.Register<IEMailClientAction, EMailClientAction>();
            container.Register<IDefaultViewerAction, DefaultViewerAction>();

            RegisterActionInitializer(container);
        }

        protected virtual void RegisterActionInitializer(Container container)
        { }

        private void RegisterFolderProvider(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<FolderProvider>(container);

            container.AddRegistration(typeof(ITempFolderProvider), registration);
            container.AddRegistration(typeof(ISpoolerProvider), registration);
            container.AddRegistration(typeof(IAppDataProvider), registration);
        }

        private void RegisterUserGuideHelper(Container container)
        {
            container.RegisterSingleton<IUserGuideLauncher, UserGuideLauncher>();
            var registration = Lifestyle.Singleton.CreateRegistration<UserGuideHelper>(container);
            container.AddRegistration(typeof(UserGuideHelper), registration);
            container.AddRegistration(typeof(IUserGuideHelper), registration);
        }

        private void RegisterTranslator(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<TranslationHelper>(container);
            container.AddRegistration(typeof(BaseTranslationHelper), registration);
            container.AddRegistration(typeof(TranslationHelper), registration);
            container.AddRegistration(typeof(ILanguageProvider), registration);
            container.AddRegistration(typeof(ITranslationHelper), registration);

            var translationFactory = new TranslationFactory();
            container.RegisterSingleton(() => translationFactory);
            var cachedTranslationFactory = new CachedTranslationFactory(translationFactory);
            registration = Lifestyle.Singleton.CreateRegistration(() => cachedTranslationFactory, container);
            container.AddRegistration(typeof(CachedTranslationFactory), registration);
            container.AddRegistration(typeof(ITranslationFactory), registration);
        }

        private void RegisterParameterSettingsManager(Container container)
        {
            container.RegisterSingleton<IStoredParametersManager, StoredParametersManager>();
        }

        protected abstract SettingsProvider CreateSettingsProvider();

        protected virtual void RegisterBannerManager(Container container)
        {
            container.Register<IBannerManager, BannerManagerDefault>();
        }

        private void RegisterSettingsHelper(Container container)
        {
            container.RegisterSingleton<ISettingsManager, PDFCreatorSettingsManager>();

            // Register the same SettingsHelper for SettingsHelper and ISettingsProvider
            var registration = Lifestyle.Singleton.CreateRegistration(CreateSettingsProvider, container);
            container.AddRegistration(typeof(SettingsProvider), registration);
            container.AddRegistration(typeof(ISettingsProvider), registration);
            container.AddRegistration(typeof(IApplicationLanguageProvider), registration);
        }

        public void RegisterObsidianInteractions()
        {
            ViewRegistry.RegisterInteraction(typeof(UpdateOverviewInteraction), typeof(UpdateHintView));
            ViewRegistry.RegisterInteraction(typeof(PrintJobInteraction), typeof(PrintJobShell));
            ViewRegistry.RegisterInteraction(typeof(InputInteraction), typeof(InputBoxUserControl));
            ViewRegistry.RegisterInteraction(typeof(MessageInteraction), typeof(MessageView));
            ViewRegistry.RegisterInteraction(typeof(ManagePrintJobsInteraction), typeof(ManagePrintJobsWindow));
            ViewRegistry.RegisterInteraction(typeof(EncryptionPasswordInteraction), typeof(EncryptionPasswordsUserControl));
            ViewRegistry.RegisterInteraction(typeof(EditEmailTextInteraction), typeof(EditEmailTextUserControl));
            ViewRegistry.RegisterInteraction(typeof(UpdateDownloadInteraction), typeof(UpdateDownloadWindow));
            ViewRegistry.RegisterInteraction(typeof(RecommendPdfArchitectInteraction), typeof(RecommendPdfArchitectView), new WindowOptions { ResizeMode = ResizeMode.NoResize });
            ViewRegistry.RegisterInteraction(typeof(PasswordOverlayInteraction), typeof(PasswordOverlay));
            ViewRegistry.RegisterInteraction(typeof(SignaturePasswordInteraction), typeof(SignaturePasswordOverlayView));
            ViewRegistry.RegisterInteraction(typeof(OfflineActivationInteraction), typeof(OfflineActivationUserControl));
            ViewRegistry.RegisterInteraction(typeof(LicenseInteraction), typeof(LicenseUpdateControl));
            ViewRegistry.RegisterInteraction(typeof(StoreLicenseForAllUsersInteraction), typeof(StoreLicenseForAllUsersControl));
            ViewRegistry.RegisterInteraction(typeof(FtpAccountInteraction), typeof(FtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(SmtpAccountInteraction), typeof(SmtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(HttpAccountInteraction), typeof(HttpAccountView));
            ViewRegistry.RegisterInteraction(typeof(TimeServerAccountInteraction), typeof(TimeServerAccountView));
            ViewRegistry.RegisterInteraction(typeof(TitleReplacementEditInteraction), typeof(TitleReplacementEditUserControl));
            ViewRegistry.RegisterInteraction(typeof(RestartApplicationInteraction), typeof(RestartApplicationInteractionView));
            ViewRegistry.RegisterInteraction(typeof(WorkflowEditorOverlayInteraction), typeof(WorkflowEditorOverlayView));
            ViewRegistry.RegisterInteraction(typeof(AddActionOverlayInteraction), typeof(AddActionOverlayView));
            ViewRegistry.RegisterInteraction(typeof(SelectFileInteraction), typeof(SelectFileView));
            ViewRegistry.RegisterInteraction(typeof(EditEmailDifferingFromInteraction), typeof(EditEmailDifferingFromView));
        }

        public virtual ApplicationSettingsTabs DefineApplicationSettingsTabs()
        {
            var applicationSettingsTabs = new ApplicationSettingsTabs();

            ModifyApplicationSettingsTabs(applicationSettingsTabs);

            return applicationSettingsTabs;
        }

        protected virtual Type[] GetStartupActions()
        {
            return new[]
            {
                typeof(StartupNavigationAction)
            };
        }

        protected virtual void ModifyApplicationSettingsTabs(TabRegion applicationSettingsTabs)
        {
        }

        public virtual void RegisterEditionDependentRegions(IRegionManager regionManager)
        {
        }

        protected abstract void RegisterNotificationService(Container container);

        public void RegisterPrismNavigation(Container container)
        {
            RegisterNavigationViews(container);
        }

        private void RegisterNavigationViews(Container container)
        {
            container.RegisterTypeForNavigation<AboutView>();
            container.RegisterTypeForNavigation<AccountsView>();
            container.RegisterTypeForNavigation<ArchitectView>();
            container.RegisterTypeForNavigation<HomeView>();
            container.RegisterTypeForNavigation<PrinterView>();
            container.RegisterTypeForNavigation<ProfilesView>();
            container.RegisterTypeForNavigation<ApplicationSettingsView>();
            container.RegisterTypeForNavigation<PrintJobView>();
            container.RegisterTypeForNavigation<PdfPasswordView>();
            container.RegisterTypeForNavigation<QuickActionView>();
            container.RegisterTypeForNavigation<FtpPasswordView>();
            container.RegisterTypeForNavigation<SmtpPasswordView>();
            container.RegisterTypeForNavigation<HttpPasswordView>();
            container.RegisterTypeForNavigation<SignaturePasswordStepView>();
            container.RegisterTypeForNavigation<ProfessionalHintStepView>();
            container.RegisterTypeForNavigation<ProgressView>();
            container.RegisterTypeForNavigation<DropboxShareLinkStepView>();
            container.RegisterTypeForNavigation<UpdateHintView>();
            container.RegisterTypeForNavigation<WorkflowEditorView>();
            container.RegisterTypeForNavigation<SaveView>();
            container.RegisterTypeForNavigation<MetadataView>();
            container.RegisterTypeForNavigation<OutputFormatUserControl>();
            container.RegisterTypeForNavigation<WorkflowEditorTestPageUserControl>();
            RegisterPrismTabs(container);
        }

        private void RegisterPrismTabs(Container container)
        {
            var applicationSettingsTabs = DefineApplicationSettingsTabs();
            container.RegisterSingleton<IApplicationSettingsTabs>(() => applicationSettingsTabs);
            applicationSettingsTabs.RegisterNavigationViews(container);
        }
    }
}
