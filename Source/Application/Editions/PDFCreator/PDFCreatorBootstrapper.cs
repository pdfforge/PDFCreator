using Banners;
using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Update;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Banner;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Version;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities.Web;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace pdfforge.PDFCreator.Editions.PDFCreator
{
    public class PDFCreatorBootstrapper : Bootstrapper
    {
        protected override string EditionName => "Free";
        protected override Color EditionHighlightColor => Color.FromRgb(215, 40, 40);
        protected override bool HideLicensing => true;

        protected override EditionHelper EditionHelper => new EditionHelper(true);

        protected override void RegisterSettingsLoader(Container container)
        {
            container.RegisterSingleton<ISettingsLoader, SettingsLoader>();
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
            container.Register<IInteractiveWorkflowManagerFactory, InteractiveWorkflowManagerFactoryWithProfessionalHintHintStep>();
        }

        protected override void RegisterJobBuilder(Container container)
        {
            container.Register<IJobBuilder, JobBuilderFree>();
        }

        protected override void RegisterActivationHelper(Container container)
        {
            container.Register<ILicenseChecker, UnlicensedLicenseChecker>();
            container.Register<IOfflineActivator, UnlicensedOfflineActivator>();

            container.Register<IPrioritySupportUrlOpenCommand, DisabledPrioritySupportUrlOpenCommand>();
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

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            defaultConditions.Add(typeof(TerminalServerNotAllowedCondition));

            return defaultConditions;
        }

        protected override void RegisterProfessionalHintHelper(Container container)
        {
            container.RegisterSingleton<IProfessionalHintHelper, ProfessionalHintHelper>();
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new DefaultSettingsProvider();
        }

        protected override void RegisterBannerManager(Container container)
        {
            var cacheDirectory = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\pdfforge\PDFCreator\banners");
            var bannerUrl = Urls.BannerIndexUrl;

            var useStaging = Environment.CommandLine.IndexOf("/Banners=staging", StringComparison.InvariantCultureIgnoreCase) >= 0;

            if (useStaging)
            {
                cacheDirectory += "-staging";
                bannerUrl = Urls.BannerIndexUrlStaging;
            }

            var bannerOptions = new BannerOptions(bannerUrl, cacheDirectory, TimeSpan.FromDays(1));
            container.RegisterInstance(bannerOptions);
            container.Register<IBannerManager, OnlineBannerManager>();
            container.Register<IBannerMetricFactory, BannerMetricFactory>();
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
    }
}
