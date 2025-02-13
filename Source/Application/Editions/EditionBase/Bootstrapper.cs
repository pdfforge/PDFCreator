using pdfforge.Banners;
using pdfforge.Banners.Helper;
using pdfforge.CustomScriptAction;
using pdfforge.DataStorage;
using pdfforge.Mail;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Helper;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.Actions.AttachToOutlookItem;
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
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Services.Trial;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Setup.Shared.Helper;
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
using pdfforge.PDFCreator.UI.Presentation.Helper.Font;
using pdfforge.PDFCreator.UI.Presentation.Helper.TestPage;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.UI.Presentation.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Architect;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Dialogs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Home;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay;
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
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.PageNumbers;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.CsScript;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.ForwardToOtherProfile;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Dropbox;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.EmailWeb;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.FTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.HTTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailClient;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Print;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.RunProgram;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DirectConversion;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
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
using pdfforge.PDFCreator.UI.RssFeed;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.Views.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Pdf;
using pdfforge.PDFCreator.Utilities.Registry;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.UserGuide;
using pdfforge.PDFCreator.Utilities.Web;
using pdfforge.PDFCreator.Utilities.WindowsApi;
using pdfforge.UsageStatistics;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using pdfforge.PDFCreator.Conversion.Actions.Actions.OneDrive;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OneDrive;
using SystemInterface;
using SystemInterface.Diagnostics;
using SystemInterface.IO;
using SystemWrapper;
using SystemWrapper.Diagnostics;
using SystemWrapper.IO;
using Translatable;
using Container = SimpleInjector.Container;
using GeneralSettingsView = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.GeneralSettingsView;
using HashUtil = pdfforge.PDFCreator.Utilities.HashUtil;
using IDownloader = pdfforge.PDFCreator.Core.Services.Download.IDownloader;
using IHashUtil = pdfforge.PDFCreator.Utilities.IHashUtil;
using IProcessStarter = pdfforge.PDFCreator.Utilities.Process.IProcessStarter;
using IRegistry = SystemInterface.Microsoft.Win32.IRegistry;
using IWebLinkLauncher = pdfforge.PDFCreator.Utilities.Web.IWebLinkLauncher;
using ManagePrintJobsWindow = pdfforge.PDFCreator.UI.Presentation.Windows.ManagePrintJobsWindow;
using MicrosoftAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft.MicrosoftAccountView;
using PrintJobShell = pdfforge.PDFCreator.UI.Presentation.PrintJobShell;
using ProcessStarter = pdfforge.PDFCreator.Utilities.Process.ProcessStarter;
using RegistryWrap = SystemWrapper.Microsoft.Win32.RegistryWrap;
using SmtpAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.SmtpAccountView;
using WebClientDownloader = pdfforge.PDFCreator.Core.Services.Download.WebClientDownloader;
using WebLinkLauncher = pdfforge.PDFCreator.Utilities.Web.WebLinkLauncher;
using WorkflowEditorTestPageUserControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings.WorkflowEditorTestPageUserControl;
using pdfforge.PDFCreator.UI.Interactions.Feedback;
using pdfforge.PDFCreator.UI.Presentation.Helper.Feedback;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Feedback;
using pdfforge.PDFCreator.UI.Presentation.Windows.Feedback;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public abstract class Bootstrapper
    {
        private WorkflowFactory _workflowFactory;

        public abstract void InitializeServices(Container container);

        protected abstract string EditionName { get; }
        protected abstract Color EditionHighlightColor { get; }
        protected abstract bool HideLicensing { get; }
        protected abstract string BannerProductName { get; }

        protected abstract EditionHelper EditionHelper { get; }

        public void RegisterMainApplication(Container container)
        {
            container.Options.SuppressLifestyleMismatchVerification = true;

            RegisterActivationHelper(container);
            container.RegisterSingleton(() => new ApplicationNameProvider(EditionName));
            container.RegisterSingleton<HighlightColorRegistration>(() => new HighlightColorRegistration(EditionHighlightColor));
            container.RegisterSingleton(() => new LicenseOptionProvider(HideLicensing));
            container.RegisterSingleton(() => EditionHelper);
            container.RegisterSingleton<IPdfEditorHelper, PdfEditorHelper>();
            container.RegisterSingleton<DropboxAppData>(() => new DropboxAppData(Data.Decrypt(DropboxAppKey.Encrypted_DropboxAppKey)));
            container.RegisterSingleton<AhaAppData>(() => new AhaAppData(Data.Decrypt(AhaResources.Encrypted_AhaAuthKey)));
            container.RegisterSingleton<IDropboxTokenCache, DropboxTokenCache>();

            container.RegisterSingleton<IInstallationPathProvider>(() => InstallationPathProviders.PDFCreatorProvider);

            _workflowFactory = new WorkflowFactory(container);
            container.Register<AutoSaveWorkflow>(); // Workflow is registered so all dependencies can be verified in test
            container.Register<InteractiveWorkflow>(); // Workflow is registered so all dependencies can be verified in test

            container.RegisterSingleton<ICommandLocator>(() => new CommandLocator(container));

            container.Register<IWorkflowFactory>(() => _workflowFactory);

            container.Register<IOutputFileMover, AutosaveOutputFileMover>();

            container.Register<ISplitDocumentFilePathHelper, SplitDocumentFilePathHelper>();
            container.Register<ITargetFilePathComposer, TargetFilePathComposer>();

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
            container.Register<IAttachToOutlookItem, AttachToOutlookItem>();
            container.Register<IAttachToOutlookItemAssistant, AttachToOutlookItemAssistant>();
            container.Register<ITestFileDummyHelper, TestFileDummyHelper>();
            container.RegisterSingleton<ITempDirectoryHelper, TempDirectoryHelper>();

            container.Register<IActionManager, ActionManager>();

            container.Register<ICommandLineUtil, CommandLineUtil>();
            container.Register<IPrinterWrapper, PrinterWrapper>();
            container.Register<IJobPrinter, GhostScriptPrinter>();
            container.Register<ICanvasToFileHelper, CanvasToFileHelper>();

            container.Register<IJobDataUpdater, JobDataUpdater>();

            container.Register<IPageNumberCalculator, PageNumberCalculator>();

            container.Register<IMergeBeforeActionsHelper, MergeBeforeActionsHelper>();
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
            container.Register<IDirectImageConversionHelper, DirectImageConversionHelper>();
            container.RegisterSingleton<IFileConversionAssistant, FileConversionAssistant>();
            container.Register<IPrintFileHelper, PrintFileAssistant>();
            container.Register<IUacAssistant, UacAssistant>();
            container.Register<ITestPageCreator, TestPageCreator>();
            container.Register<ITestPageHelper, TestPageHelper>();
            container.RegisterSingleton<IPdfArchitectCheck, PdfArchitectCheck>();
            container.Register<IGhostscriptDiscovery, GhostscriptDiscovery>();
            container.Register<ProfileRemoveCommand>();
            container.Register<ProfileRenameCommand>();
            container.Register<IPrintTestPageAsyncCommand, PrintTestPageAsyncCommand>();

            container.RegisterSingleton<IRegistry, RegistryWrap>();
            container.RegisterSingleton<IFile, FileWrap>();
            container.RegisterSingleton<IDirectory, DirectoryWrap>();
            container.RegisterSingleton<IProcess, ProcessWrap>();
            container.RegisterSingleton<IPath, PathWrap>();
            container.RegisterSingleton<IPathUtil, PathUtil>();
            container.RegisterSingleton<IFileVersionInfoHelper, FileVersionInfoHelper>();
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
            container.RegisterSingleton<IPipeMessageHandler, NewPipeJobHandler>();

            container.RegisterSingleton<IVersionHelper>(() => new VersionHelper(GetType().Assembly));

            container.Register<IKernel32Wrapper, Kernel32Wrapper>();
            container.Register<ITerminalServerDetection, TerminalServerDetection>();

            container.Register<IRepairSpoolFolderAssistant, RepairSpoolFolderAssistant>();
            container.Register<ISpoolFolderAccess, SpoolFolderAccess>();
            container.Register<IShellExecuteHelper, ShellExecuteHelper>();
            container.RegisterSingleton<IPrinterPortReader, PrinterPortReader>();
            container.RegisterSingleton<IPrinterMappingsHelper, PrinterMappingsHelper>();
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
            container.Register<IPrinterAssistant, PrinterAssistant>();
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
            container.RegisterInitializer<IIniSettingsLoader>(loader => loader.SettingsVersion = (new CreatorAppSettings()).SettingsVersion);
            container.RegisterSingleton<IDataStorageFactory, DataStorageFactory>();
            container.RegisterSingleton<IJobInfoManager, JobInfoManager>();
            container.Register<IStaticPropertiesHack, StaticPropertiesHack>();

            container.Register<IManagePrintJobExceptionHandler, ManagePrintJobExceptionHandler>();
            container.Register<IFolderCleaner, FolderCleaner>();
            container.Register<IPdfCreatorFolderCleanUp, PdfCreatorFolderCleanUp>();

            container.Register<ISpooledJobFinder, SpooledJobFinder>();

            container.Register<IFtpConnectionFactory, FtpConnectionFactory>();
            container.Register<IFtpConnectionTester, FtpConnectionTester>();
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
            container.Register<IFontSelectorControlViewModelFactory, FontSelectorControlControlViewModelFactory>();

            container.Register<IFailedJobHandler, BaseFailedJobHandler>();

            container.RegisterSingleton<IRegionHelper, RegionHelper>();
            container.Register<ISaveChangedSettingsCommand, SaveChangedSettingsCommand>();
            container.RegisterSingleton<IGraphManager, GraphManager>();
            container.RegisterInitializer<FtpAccountViewModel>(model => model.AllowConversionInterrupts = true);
            container.RegisterInitializer<HttpAccountViewModel>(model => model.AllowConversionInterrupts = true);
            container.RegisterInitializer<SmtpAccountViewModel>(model => model.AllowConversionInterrupts = true);
            container.RegisterInitializer<PrintActionViewModel>(model => model.PrinterDialogOptionEnabled = true);

            container.RegisterSingleton<IWorkflowEditorSubViewProvider>(() => new WorkflowEditorSubViewProvider(nameof(SaveView), nameof(MetadataView), nameof(OutputFormatView)));
            container.RegisterSingleton<IGpoSettings>(GetGpoSettings);
            container.Register<UsageStatisticsViewModelBase, PdfCreatorUsageStatisticsViewModel>();
            container.Register<RestoreSettingsViewModelBase, RestoreSettingsViewModel>();
            container.Register<TestPageSettingsViewModelBase, CreatorTestPageSettingsViewModel>();
            container.Register<LoadSpecificProfileViewModelBase, LoadSpecificProfileViewModel>();
            container.RegisterSingleton<IPositionToUnitConverterFactory, PositionToUnitConverterFactory>();
            container.RegisterSingleton<ISignaturePositionAndSizeHelper, SignaturePositionAndSizeHelper>();
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

            container.RegisterSingleton<ICampaignHelper, CampaignHelper>();

            container.Register<ICommandBuilderProvider, CommandBuilderProvider>();
            container.Register<ISelectFilesUserControlViewModelFactory, SelectFilesUserControlViewModelFactory>();

            container.Register<IDropboxHttpListener, DropboxHttpListener>();
            container.Register<IDropboxCodeExchanger, DropboxCodeExchanger>();
            container.Register<IDropboxUserInfoManager, DropboxUserInfoManager>();

            container.Register<ICustomScriptHandler, CustomScriptHandler>();
            container.RegisterSingleton<ICustomScriptLoader, CsScriptLoader>();

            container.Register<IFeedbackSender, AhaFeedbackSender>();

            RegisterTranslationUpdater(container);
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
            RegisterBannerManagerWrapper(container, BannerProductName);
            RegisterWebLinkLauncher(container);
            RegisterUsageStatistics(container);
            RegisterSettingsViewModel(container);
            RegisterDirectImageConversion(container);

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
                action.Target = RegionViewName.HomeView;
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

        private void RegisterTranslationUpdater(Container container)
        {
            var testDoubleTextLength = Debugger.IsAttached 
                                       && Environment.GetEnvironmentVariable("PDFCREATOR_TESTDOUBLETEXTLENGTH", EnvironmentVariableTarget.User) == "true";

            if (testDoubleTextLength)
            {
                container.RegisterSingleton<ITranslationUpdater, DoublingTranslationUpdater>();
            }
            else
            {
                container.RegisterSingleton<ITranslationUpdater, TranslationUpdater>();
            }
        }

        protected virtual void RegisterBannerManagerWrapper(Container container, string productName)
        {
            var cacheDirectory = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\pdfforge\PDFCreator\banners");
            var bannerUrl = Urls.BannerIndexUrl;
            var cacheDuration = TimeSpan.FromHours(1);

            var useStaging = Environment.CommandLine.IndexOf("/Banners=staging", StringComparison.InvariantCultureIgnoreCase) >= 0;

            if (useStaging)
            {
                cacheDirectory += "-staging";
                bannerUrl = Urls.BannerIndexUrlStaging;
                cacheDuration = TimeSpan.Zero;
            }

            container.Register<IBannerManagerWrapper>(() =>
            {
                var trackingParameters = container.GetInstance<TrackingParameters>();
                var usageStatisticsOptions = container.GetInstance<UsageStatisticsOptions>();
                var languageProvider = container.GetInstance<IApplicationLanguageProvider>();
                var versionHelper = container.GetInstance<IVersionHelper>();

                var bannerOptions = new BannerOptions(
                    productName,
                    versionHelper.FormatWithThreeDigits(),
                    languageProvider.GetApplicationLanguage(),
                    bannerUrl,
                    cacheDirectory,
                    cacheDuration,
                    trackingParameters.ToParamList());

                // We can create a new instance here as we don't use overlays
                var windowHandleProvider = new WindowHandleProvider();

                var bannerManager = BannerManagerFactory.BuildOnlineBannerManager(bannerOptions, usageStatisticsOptions, windowHandleProvider, new List<DefaultBanner>());

                return new BannerManagerWrapper(bannerManager, container.GetInstance<ICampaignHelper>(), container.GetInstance<EditionHelper>());
            });
        }

        private void RegisterSettingsViewModel(Container container)
        {
        }

        protected abstract void RegisterDirectImageConversion(Container container);

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
                (RegionNames.ProfileSaveCancelButtonsRegion, typeof(SaveCancelButtonsControl)),
                (RegionNames.ApplicationSaveCancelButtonsRegion, typeof(SaveCancelButtonsControl)),
                (RegionNames.GeneralSettingsTabContentRegion, typeof(GeneralSettingsView)),

                (RegionNames.GeneralSettingsButtonRegion, typeof(CreatorSettingsButtonsView)),

                (RegionNames.GeneralSettingsRegion, typeof(LanguageSelectionSettingsView)),
                (RegionNames.GeneralSettingsRegion, typeof(UpdateIntervalSettingsView)),
                (RegionNames.GeneralSettingsRegion, typeof(DefaultPrinterSettingsView)),
                (RegionNames.GeneralSettingsRegion, typeof(HomeViewSettingsView)),
                (RegionNames.GeneralSettingsRegion, typeof(HotStandbySettingsView)),
                (RegionNames.GeneralSettingsRegion, typeof(ExplorerIntegrationSettingsView)),
                (RegionNames.GeneralSettingsRegion, typeof(UsageStatisticsView)),

                (RegionNames.DebugSettingsTabContentRegion, typeof(LoggingSettingView)),
                (RegionNames.DebugSettingsTabContentRegion, typeof(TestPageSettingsView)),
                (RegionNames.DebugSettingsTabContentRegion, typeof(RestoreSettingsView)),
                (RegionNames.DebugSettingsTabContentRegion, typeof(ExportSettingView)),

                (RegionNames.DirectConversionTabContentRegion, typeof(DirectConvertView)),
                (RegionNames.PrinterSaveButtonRegion, typeof(SaveButtonControl))
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
            container.RegisterSingleton<IUsageStatisticsJsonSerializer, UsageStatisticsJsonSerializer>();
            container.RegisterSingleton<IUsageStatisticsSender, AvqUsageStatisticsSender>();
            container.RegisterSingleton<IUsageMetricFactory, UsageMetricFactory>();
        }

        private void RegisterWebLinkLauncher(Container container)
        {
            var installationPathProvider = InstallationPathProviders.PDFCreatorProvider;
            container.Register(() => TrackingParameterReader.ReadFromRegistry(installationPathProvider.ApplicationRegistryPath));
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
            container.Register<IForwardToFurtherProfileViewModel, ForwardToFurtherProfileActionViewModel>();

            //Register Actions in default order
            container.Collection.Register<IAction>(new[]
            {
                // preparation
                typeof(UserTokensAction),
                typeof(PreConversionScriptAction),
                typeof(ForwardToFurtherProfileAction),

                // modification
                typeof(CoverAction),
                typeof(AttachmentAction),
                typeof(BackgroundAction),
                typeof(WatermarkAction),
                typeof(StampAction),
                typeof(PageNumbersAction),
                typeof(EncryptionAction),
                typeof(SigningAction),
                typeof(PostConversionScriptAction),

                // sending
                typeof(OpenFileAction),
                typeof(ScriptAction),
                typeof(PrintingAction),
                typeof(MailClientAction),
                typeof(MailWebAction),
                typeof(SmtpMailAction),
                typeof(DropboxAction),
                typeof(OneDriveAction),
                typeof(FtpAction),
                typeof(HttpAction),
            });

            container.RegisterSingleton<IActionOrderHelper, ActionOrderHelper>();

            container.Register<IActionLocator, ActionLocator>();

            // The order determines the order in which the actions will be shown in the UI
            var facades = new[]
            {
                // pre action
                typeof(PresenterActionFacade<UserTokenActionView, UserTokenActionViewModel>),
                typeof(PresenterActionFacade<CsScriptActionView, CsScriptActionViewModel>),
                typeof(PresenterActionFacade<ForwardToFurtherProfileActionView, IForwardToFurtherProfileViewModel>),

                // modify action
                typeof(PresenterActionFacade<CoverActionView, CoverActionViewModel>),
                typeof(PresenterActionFacade<AttachmentActionView, AttachmentActionViewModel>),
                typeof(PresenterActionFacade<StampActionView, StampActionViewModel>),
                typeof(PresenterActionFacade<WatermarkActionView, WatermarkActionViewModel>),
                typeof(PresenterActionFacade<BackgroundActionView,BackgroundActionViewModel>),
                typeof(PresenterActionFacade<PageNumbersActionView, PageNumbersActionViewModel>),
                typeof(PresenterActionFacade<EncryptionActionView, EncryptionActionViewModel>),
                typeof(PresenterActionFacade<SignatureActionView, SignatureActionViewModel>),

                // send action
                typeof(PresenterActionFacade<EmailClientActionView, EMailClientActionViewModel>),
                typeof(PresenterActionFacade<EmailWebActionView, MailWebActionViewModel>),
                typeof(PresenterActionFacade<SmtpActionView, SmtpActionViewModel>),
                typeof(PresenterActionFacade<OpenViewerActionView, OpenViewerActionViewModel>),
                typeof(PresenterActionFacade<ScriptActionView, ScriptActionViewModel>),
                typeof(PresenterActionFacade<PrintActionView, PrintActionViewModel>),
                typeof(PresenterActionFacade<FTPActionView, FtpActionViewModel>),
                typeof(PresenterActionFacade<HttpActionView, HttpActionViewModel>),
                typeof(PresenterActionFacade<OneDriveActionView, OneDriveActionViewModel>),
                typeof(PresenterActionFacade<DropboxActionView, DropboxActionViewModel>)
            };

            container.Collection.Register<IPresenterActionFacade>(facades);

            foreach (var facadeType in facades)
            {
                var viewType = facadeType.GenericTypeArguments[0];
                container.RegisterTypeForNavigation(viewType);
            }

            container.Register<ISmtpMailAction, SmtpMailAction>();
            container.Register<IEMailClientAction, MailClientAction>();
            container.Register<IOpenFileAction, OpenFileAction>();
            container.Register<IHttpAction, HttpAction>();

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
            ViewRegistry.RegisterInteraction(typeof(InputInteraction), typeof(InputInteractionView));
            ViewRegistry.RegisterInteraction(typeof(MessageInteraction), typeof(MessageView));
            ViewRegistry.RegisterInteraction(typeof(ManagePrintJobsInteraction), typeof(ManagePrintJobsWindow));
            ViewRegistry.RegisterInteraction(typeof(EncryptionPasswordInteraction), typeof(EncryptionPasswordsOverlay));
            ViewRegistry.RegisterInteraction(typeof(EditEmailTextInteraction), typeof(EditEmailTextUserControl));
            ViewRegistry.RegisterInteraction(typeof(UpdateDownloadInteraction), typeof(UpdateDownloadWindow));
            ViewRegistry.RegisterInteraction(typeof(RecommendPdfArchitectInteraction), typeof(RecommendPdfArchitectView), new WindowOptions { ResizeMode = ResizeMode.NoResize });
            ViewRegistry.RegisterInteraction(typeof(PasswordOverlayInteraction), typeof(PasswordOverlay));
            ViewRegistry.RegisterInteraction(typeof(SignaturePasswordInteraction), typeof(SignaturePasswordOverlayView));
            ViewRegistry.RegisterInteraction(typeof(FtpAccountInteraction), typeof(FtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(SmtpAccountInteraction), typeof(SmtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(HttpAccountInteraction), typeof(HttpAccountView));
            ViewRegistry.RegisterInteraction(typeof(MicrosoftAccountInteraction), typeof(MicrosoftAccountView));
            ViewRegistry.RegisterInteraction(typeof(TimeServerAccountInteraction), typeof(TimeServerAccountView));
            ViewRegistry.RegisterInteraction(typeof(TitleReplacementEditInteraction), typeof(TitleReplacementEditUserControl));
            ViewRegistry.RegisterInteraction(typeof(RestartApplicationInteraction), typeof(RestartApplicationInteractionView));
            ViewRegistry.RegisterInteraction(typeof(WorkflowEditorOverlayInteraction), typeof(WorkflowEditorOverlayView));
            ViewRegistry.RegisterInteraction(typeof(AddActionOverlayInteraction), typeof(AddActionOverlayView));
            ViewRegistry.RegisterInteraction(typeof(SelectFileInteraction), typeof(SelectFileView));
            ViewRegistry.RegisterInteraction(typeof(EditEmailDifferingFromInteraction), typeof(EditEmailDifferingFromView));
            ViewRegistry.RegisterInteraction(typeof(SignaturePositionAndSizeInteraction), typeof(SignaturePositionAndSizeView));
            ViewRegistry.RegisterInteraction(typeof(DrawSignatureInteraction), typeof(DrawSignatureView));
            ViewRegistry.RegisterInteraction(typeof(OverwriteOrAppendInteraction), typeof(OverwriteOrAppendOverlay));
            ViewRegistry.RegisterInteraction(typeof(LoadSpecificProfileInteraction), typeof(LoadSpecificProfileView));
            ViewRegistry.RegisterInteraction(typeof(EditPrinterProfileUserInteraction), typeof(EditPrinterProfileUserUserControl));
            ViewRegistry.RegisterInteraction(typeof(FeedbackInteraction), typeof(FeedbackWindowView),
                new WindowOptions { ApplyWindowCustomization = window => window.WindowStartupLocation = WindowStartupLocation.CenterScreen });
            ViewRegistry.RegisterInteraction(typeof(FeedbackSentInteraction), typeof(FeedbackSentView),
                new WindowOptions
                {
                    ResizeMode = ResizeMode.NoResize,
                    ApplyWindowCustomization = window => window.WindowStartupLocation = WindowStartupLocation.CenterScreen
                });
            RegisterObsidianLicenseInteractions();
        }

        protected abstract void RegisterObsidianLicenseInteractions();

        protected virtual Type[] GetStartupActions()
        {
            return new[]
            {
                typeof(StartupNavigationAction)
            };
        }

        protected virtual void ModifyApplicationSettingsTabs()
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
            container.RegisterTypeForNavigation<SecurityPasswordsStepView>();
            container.RegisterTypeForNavigation<QuickActionView>();
            container.RegisterTypeForNavigation<FtpPasswordStepView>();
            container.RegisterTypeForNavigation<SmtpPasswordStepView>();
            container.RegisterTypeForNavigation<HttpPasswordStepView>();
            container.RegisterTypeForNavigation<SignaturePasswordStepView>();
            container.RegisterTypeForNavigation<ProfessionalHintStepView>();
            container.RegisterTypeForNavigation<ProgressView>();
            container.RegisterTypeForNavigation<ErrorView>();
            container.RegisterTypeForNavigation<DropboxShareLinkStepView>();
            container.RegisterTypeForNavigation<UpdateHintView>();
            container.RegisterTypeForNavigation<WorkflowEditorView>();
            container.RegisterTypeForNavigation<SaveView>();
            container.RegisterTypeForNavigation<MetadataView>();
            container.RegisterTypeForNavigation<OutputFormatView>();
            container.RegisterTypeForNavigation<WorkflowEditorTestPageUserControl>();
            container.RegisterTypeForNavigation<GeneralSettingsView>();
            container.RegisterTypeForNavigation<DebugSettingView>();
            container.RegisterTypeForNavigation<TitleReplacementsView>();
            container.RegisterTypeForNavigation<DefaultViewerView>();
            container.RegisterTypeForNavigation<DirectImageConversionSettingView>();
        }
    }
}
