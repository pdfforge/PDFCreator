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
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Translation;
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
using pdfforge.PDFCreator.UI.Notifications;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Banner;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands;
using pdfforge.PDFCreator.UI.Presentation.Commands.FirstTimeCommands;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.Converter;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Helper.Update;
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
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Print;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
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
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.Views.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Registry;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.UserGuide;
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
using Translatable;
using FtpAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.FtpAccountView;
using IJobHistoryManager = pdfforge.PDFCreator.Core.Services.JobHistory.IJobHistoryManager;
using InputBoxUserControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Dialogs.InputBoxUserControl;
using JobHistoryManager = pdfforge.PDFCreator.Core.Services.JobHistory.JobHistoryManager;
using LicenseUpdateControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License.LicenseUpdateControl;
using ManagePrintJobsWindow = pdfforge.PDFCreator.UI.Presentation.Windows.ManagePrintJobsWindow;
using PrintJobShell = pdfforge.PDFCreator.UI.Presentation.PrintJobShell;
using SmtpAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.SmtpAccountView;
using StoreLicenseForAllUsersControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License.StoreLicenseForAllUsersControl;

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
            container.RegisterSingleton(() => new DropboxAppData(Data.Decrypt(DropboxAppKey.Encrypted_DropboxAppKey), "https://www.dropbox.com/1/oauth2/redirect_receiver"));

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

            container.RegisterSingleton<IRecommendArchitect, RecommendArchitect>();
            container.RegisterSingleton<IRecommendArchitectUpgrade, RecommendArchitectUpgrade>();
            container.Register<IStartupActionHandler, StartupActionHandler>();
            container.RegisterSingleton<IInteractionInvoker, InteractionInvoker>();

            RegisterInteractionRequestPerThread(container);

            container.Register<ISoundPlayer, SoundPlayer>();
            container.RegisterSingleton<IWpfTopMostHelper, WpfTopMostHelper>();
            container.Register<ISmtpTest, SmtpTestEmailAssistant>();
            container.Register<IClientTestEmail, ClientTestEmail>();
            container.Register<IActionManager, ActionManager>();

            container.Register<IJobPrinter, GhostScriptPrinter>();

            container.Register<IJobDataUpdater, JobDataUpdater>();

            container.Register<IPageNumberCalculator, PageNumberCalculator>();

            container.Register<IJobRunner, JobRunner>();
            container.Register<IConverterFactory, ConverterFactory>();
            container.Register<IPsConverterFactory, GhostscriptConverterFactory>();
            container.RegisterSingleton<IUpdateHelper, UpdateHelper>();

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

            container.RegisterSingleton<IRegistry, RegistryWrap>();
            container.RegisterSingleton<IFile, FileWrap>();
            container.RegisterSingleton<IDirectory, DirectoryWrap>();
            container.RegisterSingleton<IPath, PathWrap>();
            container.RegisterSingleton<IPathUtil, PathUtil>();
            container.RegisterSingleton<IEnvironment, EnvironmentWrap>();
            container.RegisterSingleton<IDirectoryHelper, DirectoryHelper>();
            container.RegisterSingleton<IReadableFileSizeFormatter, ReadableReadableFileSizeFormatter>();

            container.RegisterSingleton<IProcessStarter, ProcessStarter>();
            container.RegisterSingleton<IDateTimeProvider, DateTimeProvider>();

            container.RegisterSingleton<IAssemblyHelper>(() => new AssemblyHelper(GetType().Assembly));
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

            container.Register<ISignaturePasswordCheck, SignaturePasswordCheck>();

            container.Register<IWelcomeSettingsHelper, WelcomeSettingsHelper>();

            container.Register<IEmailClientFactory, EmailClientFactory>();
            container.Register<IProfileChecker, ProfileChecker>();
            container.Register<IDefaultViewerCheck, DefaultViewerCheck>();

            container.RegisterCollection<ISettingsNavigationCheck>(new[]
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
            container.Register<IMaybePipedApplicationStarter, MaybePipedApplicationStarter>();
            container.Register<ITokenReplacerFactory, TokenReplacerFactory>();

            container.RegisterSingleton<ISettingsChanged, SettingsChanged>();

            container.RegisterSingleton<IJobHistoryStorage, JobHistoryJsonFileStorage>();
            container.RegisterSingleton<IJobHistoryManager, JobHistoryManager>();

            container.RegisterSingleton<IDefaultSettingsBuilder, CreatorDefaultSettingsBuilder>();

            container.RegisterSingleton<IMigrationStorageFactory>(() =>
                new MigrationStorageFactory((baseStorage, targetVersion) => new CreatorSettingsMigrationStorage(baseStorage, targetVersion)));

            container.Register<IIniSettingsAssistant, CreatorIniSettingsAssistant>();
            container.RegisterSingleton<IIniSettingsLoader, IniSettingsLoader>();
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
            container.RegisterSingleton<IGpoSettings>(GetGpoSettings);
            container.Register<UsageStatisticsViewModelBase, PdfCreatorUsageStatisticsViewModel>();
            container.RegisterSingleton<ISigningPositionToUnitConverterFactory, SigningPositionToUnitConverterFactory>();

            RegisterSettingsLoader(container);
            RegisterCurrentSettingsProvider(container);
            RegisterFolderProvider(container);
            RegisterUserGuideHelper(container);
            RegisterTranslator(container);
            RegisterMailSigantureHelper(container);
            RegisterParameterSettingsManager(container);
            RegisterSettingsHelper(container);
            RegisterStartupConditions(container);
            RegisterActions(container);
            RegisterActionChecks(container);
            RegisterFileNameQuery(container);
            RegisterUpdateAssistant(container);
            RegisterJobBuilder(container);
            RegisterInteractiveWorkflowManagerFactory(container);
            RegisterPdfProcessor(container);
            RegisterUserTokenExtractor(container);
            RegisterPlusHintHelper(container);
            RegisterFirstTimeCommand(container);
            RegisterNotificationService(container);
            RegisterAllTypedSettingsProvider(container);
            RegisterBannerManager(container);
            container.RegisterSingleton(BuildCustomization);
            container.RegisterSingleton<IHttpHandler, HttpClientWrapper>();
            container.RegisterSingleton<IRssHttpClientFactory, RssHttpClientFactory>();
            container.RegisterSingleton<IKernel32, Kernel32>();
            container.RegisterSingleton<IMachineIdGenerator, MachineIdGenerator>();
            container.RegisterSingleton<IPdfCreatorUsageStatisticsManager, PdfCreatorUsageStatisticsManager>();
            container.RegisterSingleton<IUsageStatisticsSender, UsageStatisticsSender>();
            container.RegisterSingleton<IJobEventsManager, JobEventsManager>();
            container.RegisterSingleton<IRssService, RssService>();

            container.RegisterInitializer<StartupNavigationAction>(action =>
            {
                action.Region = RegionNames.MainRegion;
                action.Target = MainRegionViewNames.HomeView;
            });

            container.RegisterCollection<IStartupAction>(RegisterAndGetStartupActions(container));

            container.RegisterCollection<IJobEventsHandler>(new[]
            {
                typeof(UsageStatisticsEventsHandler)
            });
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

            RegisterTypedSettingsProvider<UsageStatistics>(
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

        protected void RegisterFirstTimeCommand(Container container)
        {
            container.RegisterCollection<IFirstTimeCommand>(new[]
            {
                typeof(ResetShowQuickActionCommand)
            });
        }

        protected abstract void RegisterSettingsLoader(Container container);

        protected abstract void RegisterUpdateAssistant(Container container);

        protected abstract void RegisterJobBuilder(Container container);

        protected abstract void RegisterInteractiveWorkflowManagerFactory(Container container);

        protected abstract void RegisterActivationHelper(Container container);

        protected abstract void RegisterUserTokenExtractor(Container container);

        protected abstract void RegisterPdfProcessor(Container container);

        protected abstract IGpoSettings GetGpoSettings();

        protected virtual void RegisterMailSigantureHelper(Container container)
        {
            container.Register<IMailSignatureHelper, MailSignatureHelperLicensed>();
        }

        protected virtual void RegisterPlusHintHelper(Container container)
        {
            container.Register<IPlusHintHelper, PlusHintHelperDisabled>();
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

        private void RegisterActionChecks(Container container)
        {
            container.RegisterCollection<ICheckable>(new[]
            {
                typeof(FtpAction),
                typeof(ScriptAction),
                typeof(DropboxAction),
                typeof(HttpAction),
                typeof(SmtpMailAction)
            });
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
            container.RegisterCollection<IStartupCondition>(conditions);

            container.Register<ICheckAllStartupConditions, CheckAllStartupConditions>();
        }

        private void RegisterActions(Container container)
        {
            //Register empty collection
            container.RegisterCollection<IPreConversionAction>();

            container.RegisterCollection<IPostConversionAction>(new[]
            {
                typeof(ScriptAction),
                typeof(DefaultViewerAction),
                typeof(DropboxAction),
                typeof(EMailClientAction),
                typeof(FtpAction),
                typeof(PrintingAction),
                typeof(HttpAction),
                typeof(SmtpMailAction)
            });

            container.Register<ISmtpMailAction, SmtpMailAction>();
            container.Register<IEMailClientAction, EMailClientAction>();
            container.Register<IDefaultViewerAction, DefaultViewerAction>();
        }

        private void RegisterFolderProvider(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<FolderProvider>(container);

            container.AddRegistration(typeof(ITempFolderProvider), registration);
            container.AddRegistration(typeof(ISpoolerProvider), registration);
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
            container.RegisterSingleton<ISettingsManager, SettingsManager>();

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
            ViewRegistry.RegisterInteraction(typeof(DropboxAccountInteraction), typeof(DropboxAccountWindow));
            ViewRegistry.RegisterInteraction(typeof(StoreLicenseForAllUsersInteraction), typeof(StoreLicenseForAllUsersControl));
            ViewRegistry.RegisterInteraction(typeof(FtpAccountInteraction), typeof(FtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(SmtpAccountInteraction), typeof(SmtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(HttpAccountInteraction), typeof(HttpAccountView));
            ViewRegistry.RegisterInteraction(typeof(TimeServerAccountInteraction), typeof(TimeServerAccountView));
            ViewRegistry.RegisterInteraction(typeof(TitleReplacementEditInteraction), typeof(TitleReplacementEditUserControl));
            ViewRegistry.RegisterInteraction(typeof(RestartApplicationInteraction), typeof(RestartApplicationInteractionView));
        }

        public ProfileSettingsTabs DefineProfileSettingsTabs()
        {
            var profileTabs = new ProfileSettingsTabs();

            ModifyProfileSettingsTabs(profileTabs);

            return profileTabs;
        }

        public virtual ApplicationSettingsTabs DefineApplicationSettingsTabs()
        {
            var applicationSettingsTabs = new ApplicationSettingsTabs();

            ModifyApplicationSettingsTabs(applicationSettingsTabs);

            return applicationSettingsTabs;
        }

        protected virtual void ModifyProfileSettingsTabs(TabRegion profileSettingsTabs)
        {
        }

        protected virtual Type[] RegisterAndGetStartupActions(Container container)
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

        public virtual void RegisterNotificationService(Container container)
        {
            container.RegisterSingleton<INotificationService, NotificationService>();
        }

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
            container.RegisterTypeForNavigation<PlusHintView>();
            container.RegisterTypeForNavigation<ProgressView>();
            container.RegisterTypeForNavigation<DropboxShareLinkStepView>();
            container.RegisterTypeForNavigation<UpdateHintView>();
            RegisterPrismTabs(container);
        }

        private void RegisterPrismTabs(Container container)
        {
            var profileSettingsTabs = DefineProfileSettingsTabs();
            var applicationSettingsTabs = DefineApplicationSettingsTabs();

            container.RegisterSingleton<IProfileSettingsTabs>(() => profileSettingsTabs);
            container.RegisterSingleton<IApplicationSettingsTabs>(() => applicationSettingsTabs);

            profileSettingsTabs.RegisterNavigationViews(container);
            applicationSettingsTabs.RegisterNavigationViews(container);
        }
    }
}
