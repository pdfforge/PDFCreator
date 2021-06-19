using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public partial class DesignTimePrintJobViewModel : PrintJobViewModel
    {
        public DesignTimePrintJobViewModel()
            : base(new DesignTimeSettingsProvider(),
                  new TranslationUpdater(new TranslationFactory(), new ThreadManager()),
                  new DesignTimeJobInfoQueue(),
                  new DesignTimeCommandLocator(),
                  new DesignTimeEventAggregator(), //IEventAggregator
                  null,//ISelectedProfileProvider
                  null,
                  null,
                  new DesignTimeTargetFilePathComposer(),
                  null,
                  new DesignTimeChangeJobCheckAndProceedCommandBuilder(),
                  new DesignTimeBrowseFileCommandBuilder(),
                  new DispatcherWrapper(),
                  new DesignTimeJobDataUpdater()
                  )
        {
            var jobInfo = new JobInfo()
            {
                Metadata = new Metadata() { Author = "Max Mustermann", Keywords = "keywords...", PrintJobAuthor = "print job author", PrintJobName = "My Print Job", Subject = "This is the subject line", Title = "My Document Title" }
            };

            var job = new Job(jobInfo, new ConversionProfile(), CurrentJobSettings.Empty())
            {
                OutputFileTemplate = @"C:\My Documents\MyFile.pdf"
            };

            job.Profile = ProfilesWrapper != null ? ProfilesWrapper.First().ConversionProfile : new ConversionProfile();

            SetNewJob(job);
        }
    }
}
