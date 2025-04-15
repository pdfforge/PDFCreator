using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface.ImagesToPdf;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Presentation.Helper.Interfaces;
using pdfforge.PDFCreator.UI.Presentation.Helper.SetupDownloadHelper;
using pdfforge.PDFCreator.Utilities;
using IWebLinkLauncher = pdfforge.PDFCreator.Utilities.Web.IWebLinkLauncher;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.Obsidian.Helper;
using System.Windows;
using pdfforge.PDFCreator.Utilities.Update;

namespace pdfforge.PDFCreator.Editions.PDFCreator
{
    public class PDFCreatorBootstrapper : Bootstrapper
    {
        protected override string EditionName => "Free";
        protected override Color EditionHighlightColor => Color.FromRgb(215, 40, 40);
        protected override bool HideLicensing => true;
        protected override string BannerProductName => "pdfcreator";

        protected override EditionHelper EditionHelper => new EditionHelper(Edition.Free, false);

        protected override void RegisterDirectImageConversion(Container container)
        {
            container.Register<IImagesToPdf, ITextImagesToPdf>();
        }

        public override void InitializeServices(Container container)
        {
        }

        protected override void RegisterSettingsLoader(Container container)
        {
            container.RegisterSingleton<IBaseSettingsBuilder, DefaultBaseSettingsBuilder>();
            container.RegisterSingleton<ISettingsLoader, PDFCreatorSettingsLoader>();
            container.RegisterSingleton<ISharedSettingsLoader, FreeSharedSettingsLoader>();
        }

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateHelper, UpdateHelper>();
            container.RegisterSingleton<IUpdateLauncher>(() => new SimpleUpdateLauncher(container.GetInstance<IWebLinkLauncher>()));
            container.RegisterSingleton<IOnlineVersionHelper, OnlineVersionHelper>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorUpdateInfoUrl, "PDFCreator", Urls.PdfCreatorUpdateChangelogUrl));
        }

        protected override void RegisterInteractiveWorkflowManagerFactory(Container container)
        {
            container.Register<IInteractiveWorkflowManagerFactory, InteractiveWorkflowManagerFactoryWithConditionalHintSteps>();
        }

        protected override void RegisterJobBuilder(Container container)
        {
            container.Register<IJobBuilder, JobBuilderFree>();
        }

        protected override void RegisterActivationHelper(Container container)
        {
            container.Register<ILicenseChecker, UnlicensedLicenseChecker>();
            container.Register<IOfflineActivator, UnlicensedOfflineActivator>();

            container.Register<IPrioritySupportUrlOpenCommand, OpenBusinessHintInsteadOfPrioritySupportCommand>();
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IUserTokenExtractor, UserTokenExtractorDummy>();
        }

        protected override IGpoSettings GetGpoSettings()
        {
            return new GpoSettingsDefaults();
        }

        protected override void RegisterMailSignatureHelper(Container container)
        {
            container.Register<IMailSignatureHelper, MailSignatureHelperFreeVersion>();
        }

        protected override void RegisterActionInitializer(Container container)
        {
            container.RegisterInitializer<SmtpMailAction>(a => a.Init(true));
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {

            return defaultConditions;
        }

        protected override void RegisterConditionalHintManager(Container container)
        {
            container.RegisterSingleton<IConditionalHintManager, ConditionalHintManager>();
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new DefaultSettingsProvider();
        }

        protected override void RegisterObsidianLicenseInteractions()
        {
        }

        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, ITextPdfProcessor>();
        }

        public override void RegisterEditionDependentRegions(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(RegionNames.BusinessHintStatusBarRegion, typeof(BusinessHintStatusBarControl));
        }

        protected override void RegisterNotificationService(Container container)
        {
            container.RegisterSingleton<INotificationService, DisabledNotificationService>();
        }

        protected override void RegisterBlockedInEnvironmentHelpers(Container container)
        {
            container.Register<IRequestHelper, RequestHelper>();
            container.Register<ISetupDownloadHelper, SetupDownloadHelper>();
        }
    }
}
