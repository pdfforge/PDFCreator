using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeJobHistoryActiveRecord : IJobHistoryActiveRecord
    {
        public IList<HistoricJob> History { get; set; }

#pragma warning disable CS0067

        public event EventHandler HistoryChanged;

#pragma warning restore CS0067

        public DesignTimeJobHistoryActiveRecord()
        {
            var metaData = new Metadata
            {
                Author = "Arthur Author",
                Title = "Creative Title",
                Subject = "Even more creative Subject",
                Keywords = "Keyword1 Keyword2 Keyword7"
            };

            var hjPdf = new HistoricFile(@"X:\Foldername\PeeDeeEff.pdf", "PeeDeeEff.pdf", @"X:\Foldername\", "ABC123");
            var historicFiles = new List<HistoricFile> { hjPdf };
            var historicJob1 = new HistoricJob(historicFiles, OutputFormat.Pdf, DateTime.Now, metaData, 23, false);

            var hjJpg1 = new HistoricFile(@"X:\Foldername\JotPeeGee1.jpg", "JotPeeGee1.jpg", @"X:\Foldername\", "DEF456");
            var hjJpg3 = new HistoricFile(@"X:\Foldername\JotPeeGee3.jpg", "JotPeeGee3.jpg", @"X:\Foldername\", "GHI789");
            historicFiles = new List<HistoricFile> { hjJpg1, hjJpg3 };
            var historicJob2 = new HistoricJob(historicFiles, OutputFormat.Pdf, DateTime.Now, metaData, 3, true);

            History = new List<HistoricJob> { historicJob1, historicJob2 };
        }

        public Task Load()
        {
            throw new NotImplementedException();
        }

        public void Save(bool force)
        {
            throw new NotImplementedException();
        }

        public Task Refresh()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Add(Job job)
        {
            throw new NotImplementedException();
        }

        public HistoricJob TransformToHistoricJob(Job job)
        {
            throw new NotImplementedException();
        }

        public void Remove(HistoricJob historicJob)
        {
            throw new NotImplementedException();
        }

        public bool HistoryEnabled
        {
            get => true;
            set => throw new NotImplementedException();
        }
    }
}
