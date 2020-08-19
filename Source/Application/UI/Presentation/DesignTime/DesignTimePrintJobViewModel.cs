using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
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
                  null,//IEventAggregator
                  null,//ISelectedProfileProvider
                  null,
                  null,
                  null,
                  null,
                  null,
                  null)
        {
            var jobInfo = new JobInfo()
            {
                Metadata = new Metadata() { Author = "Max Mustermann", Keywords = "keywords...", PrintJobAuthor = "print job author", PrintJobName = "My Print Job", Subject = "This is the subject line", Title = "My Document Title" }
            };

            var job = new Job(jobInfo, new ConversionProfile(), new Accounts())
            {
                OutputFileTemplate = @"C:\My Documents\MyFile.pdf"
            };

            job.Profile = ProfilesWrapper.First().ConversionProfile;

            SetNewJob(job);
        }
    }
}
