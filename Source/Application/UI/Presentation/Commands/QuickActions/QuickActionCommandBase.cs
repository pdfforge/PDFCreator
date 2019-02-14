using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.QuickActions
{
    public abstract class QuickActionCommandBase<T> : TranslatableCommandBase<T> where T : ITranslatable, new()
    {
        public QuickActionCommandBase(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected List<string> GetPaths(object obj)
        {
            var files = new List<string>();
            switch (obj)
            {
                case Job job:
                    files = job.OutputFiles.ToList();
                    break;

                case Core.Services.JobHistory.HistoricJob historicJob:
                    foreach (var historicFile in historicJob.HistoricFiles)
                        files.Add(historicFile.Path);
                    break;

                case string path:
                    files.Add(path);
                    break;

                default:
                    throw new NotSupportedException("Object is not type of Job, HistoricJob or String");
            }
            return files;
        }
    }
}
