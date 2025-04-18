﻿using Newtonsoft.Json;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities;
using System.Windows.Input;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public abstract class UsageStatisticsViewModelBase : AGeneralSettingsItemControlModel
    {
        protected readonly ICurrentSettings<Conversion.Settings.UsageStatistics> UsageStatisticsProvider;
        private readonly ApplicationNameProvider _applicationNameProvider;
        protected readonly IUsageStatisticsJsonSerializer UsageStatisticsJsonSerializer;
        protected readonly IOsHelper OsHelper;

        protected string ApplicationNameWithEdition => _applicationNameProvider?.ApplicationNameWithEdition;

        public string PrivacyPolicy => Urls.PrivacyPolicyUrl;

        public abstract HelpTopic HelpTopic { get; }
        public abstract bool IsDisabledByGpo { get; }

        public abstract bool ShowServiceSample { get; }

        public ICommand VisitWebsiteCommand { get; }
        public ICommand ShowUserGuideCommand { get; }
        public string UsageStatisticsExplanationText => Translation.FormatUsageStatisticsExplanationText(ApplicationNameWithEdition);

        public string SampleStatisticsJobData => GetJobSampleData();
        public string SampleStatisticsData => GetServiceSampleData();

        public bool EnableUsageStatistics
        {
            get
            {
                return UsageStatisticsProvider.Settings.Enable && !IsDisabledByGpo;
            }
            set
            {
                UsageStatisticsProvider.Settings.Enable = value;
                RaisePropertyChanged(nameof(EnableUsageStatistics));
            }
        }

        protected UsageStatisticsViewModelBase(IOsHelper osHelper,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings,
            ITranslationUpdater translationUpdater, ICurrentSettings<Conversion.Settings.UsageStatistics> usageStatisticsProvider, 
            ICommandLocator commandLocator, ApplicationNameProvider applicationNameProvider,
            IUsageStatisticsJsonSerializer usageStatisticsJsonSerializer)
            : base(translationUpdater, currentSettingsProvider, gpoSettings)
        {
            OsHelper = osHelper;
            UsageStatisticsProvider = usageStatisticsProvider;
            _applicationNameProvider = applicationNameProvider;
            UsageStatisticsJsonSerializer = usageStatisticsJsonSerializer;

            VisitWebsiteCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PrivacyPolicyUrl);
            ShowUserGuideCommand = commandLocator.GetCommand<ShowUserGuideCommand>();
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(nameof(UsageStatisticsExplanationText));
        }

        protected abstract string GetJobSampleData();
        protected abstract string GetServiceSampleData();
    }
}
