using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public abstract class UsageStatisticsViewModelBase : AGeneralSettingsItemControlModel
    {
        protected readonly ICurrentSettings<Conversion.Settings.UsageStatistics> UsageStatisticsProvider;
        private readonly ApplicationNameProvider _applicationNameProvider;
        protected readonly IOsHelper OsHelper;

        protected string ApplicationNameWithEdition => _applicationNameProvider.ApplicationNameWithEdition;

        public string PrivacyPolicy => Urls.PrivacyPolicyUrl;

        public abstract HelpTopic HelpTopic { get;  }
        public abstract bool IsDisabledByGpo { get; }

        public ICommand VisitWebsiteCommand { get; }

        protected UsageStatisticsViewModelBase(IOsHelper osHelper, 
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, 
            ITranslationUpdater translationUpdater, ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider, ICommandLocator commandLocator, ApplicationNameProvider applicationNameProvider)
            : base(translationUpdater, currentSettingsProvider, gpoSettings)
        {
            OsHelper = osHelper;
            UsageStatisticsProvider = usageStatisticsProvider;
            _applicationNameProvider = applicationNameProvider;

            ShowUserGuideCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic);
            VisitWebsiteCommand = commandLocator.GetInitializedCommand<UrlOpenCommand,string>(Urls.PrivacyPolicyUrl);
        }

        public ICommand ShowUserGuideCommand { get; set; }

        protected string ConvertToJson(IUsageMetric metric)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(metric, settings);
        }

    }

}
