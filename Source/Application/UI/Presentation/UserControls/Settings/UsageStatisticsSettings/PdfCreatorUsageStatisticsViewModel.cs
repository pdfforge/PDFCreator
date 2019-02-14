using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Windows.Input;
using pdfforge.PDFCreator.Core.Controller;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
    public class PdfCreatorUsageStatisticsViewModel : UsageStatisticsViewModelBase
    {
        public string SampleStatisticsJobData => GetJobSampleData();
        public bool ShowServiceSample => false;
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


        public PdfCreatorUsageStatisticsViewModel(IVersionHelper versionHelper, IOsHelper osHelper, ICommandLocator commandLocator,
            ICurrentSettingsProvider currentSettingsProvider, IGpoSettings gpoSettings, IProcessStarter processStarter,
            ICurrentSettings<UsageStatistics> usageStatisticsProvider, ITranslationUpdater translationUpdater, ApplicationNameProvider applicationNameProvider)
            : base(versionHelper, osHelper, currentSettingsProvider, gpoSettings, processStarter, translationUpdater, usageStatisticsProvider, applicationNameProvider, commandLocator)
        {
        }

        public string UsageStatisticsExplanationText => Translation.FormatUsageStatisticsExplanationText(base.ApplicationNameWithEdition);

        public override HelpTopic HelpTopic => HelpTopic.AppGeneral;
        public override bool IsDisabledByGpo => GpoSettings.DisableUsageStatistics;

        private string GetJobSampleData()
        {
            var metric = new PdfCreatorUsageStatisticsMetric()
            {
                MachineId = "sample machineId",
                Product = base.ApplicationName,
                Version = base.VersionHelper?.ApplicationVersion?.ToString(),
                OperatingSystem = base.OsHelper.GetWindowsVersion(),
                Duration = TimeSpan.Zero.Milliseconds,
                OutputFormat = OutputFormat.Pdf.ToString(),
                Mode = Mode.Interactive,
                QuickActions = true,
                OpenViewer = true,
                OpenWithPdfArchitect = true,
                Status = "Success",
                Attachment = true,
                Background = true,
                Dropbox = true,
                Cover = true,
                NumberOfCopies = 1,
                Script = true,
                CustomScript = true,
                TotalPages = 1,
                Mailclient = true,
                Print = true,
                Signature = true,
                Encryption = true,
                UserToken = true,
                Ftp = true,
                Http = true,
                Smtp = true,
                Stamp = true
            };

            return ConvertToJson(metric);
        }

    }

    public class DesignTimePdfCreatorUsageStatisticsViewModel : PdfCreatorUsageStatisticsViewModel
    {
        public DesignTimePdfCreatorUsageStatisticsViewModel() : base(new DesignTimeVersionHelper(), new OsHelper(), new DesignTimeCommandLocator(),
                                                            new DesignTimeCurrentSettingsProvider(), new GpoSettingsDefaults(), new ProcessStarter(),
                                                            new DesignTimeCurrentSettings<UsageStatistics>(), new DesignTimeTranslationUpdater(),
                                                             new DesignTimeApplicationNameProvider())
        {

        }
    }
}
