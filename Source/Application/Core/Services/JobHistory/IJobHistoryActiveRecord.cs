using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Services.JobHistory
{
    public interface IJobHistoryActiveRecord
    {
        IList<HistoricJob> History { get; set; }

        event EventHandler HistoryChanged;

        Task Load();

        /// <param name="force">Set true to save history even is history is disabled (e.g. for deleting history)</param>
        void Save(bool force = false);

        Task Refresh();

        void Delete();

        void Add(Job job);

        HistoricJob TransformToHistoricJob(Job job);

        void Remove(HistoricJob historicJob);

        bool HistoryEnabled { get; set; }
    }
}
