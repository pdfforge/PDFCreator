using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public abstract class UsageStatisticsViewModelBase : AGeneralSettingsItemControlModel
    {
        protected readonly ICurrentSettings<UsageStatistics> UsageStatisticsProvider;
        private readonly ApplicationNameProvider _applicationNameProvider;
        protected readonly IVersionHelper VersionHelper;
        protected readonly IOsHelper OsHelper;
        private readonly IProcessStarter _processStarter;
        protected string ApplicationName => _applicationNameProvider.ApplicationNameWithEdition.ToLower().Replace(" ", "_");
        protected string ApplicationNameWithEdition => _applicationNameProvider.ApplicationNameWithEdition;

        public string PrivatePolicy => Urls.PrivatePolicyUrl;

        public abstract HelpTopic HelpTopic { get;  }
        public abstract bool IsDisabledByGpo { get; }

        public ICommand VisitWebsiteCommand => new DelegateCommand(VisitWebsiteExecute);

        protected UsageStatisticsViewModelBase(IVersionHelper versionHelper, IOsHelper osHelper, 
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, IProcessStarter processStarter,
            ITranslationUpdater translationUpdater, ICurrentSettings<UsageStatistics> usageStatisticsProvider, ApplicationNameProvider applicationNameProvider, ICommandLocator commandLocator)
            : base(translationUpdater, currentSettingsProvider, gpoSettings)
        {
            VersionHelper = versionHelper;
            OsHelper = osHelper;
            _processStarter = processStarter;
            UsageStatisticsProvider = usageStatisticsProvider;
            _applicationNameProvider = applicationNameProvider;

            ShowUserGuideCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic);
        }

        public ICommand ShowUserGuideCommand { get; set; }

        private void VisitWebsiteExecute(object o)
        {
            try
            {
                _processStarter.Start(PrivatePolicy);
            }
            catch
            {
                // ignored
            }
        }

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
