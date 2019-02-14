using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class UpdateVersion
    {
        private const string STR_BUGFIX = "bugfix";
        private const string STR_FEATURE = "feature";
        private const string STR_REFACTOR = "refactor";
        private const string STR_TASK = "task";

        public UpdateVersion(Version version, List<UpdateChange> changes, DateTime releaseDate)
        {
            Version = version;
            Changes = changes.OrderByDescending(x => x.Priority).ToList();
            ReleaseDate = releaseDate;
        }

        public Version Version { get; }
        public List<UpdateChange> Changes { get; }
        public DateTime ReleaseDate { get; }
    }
}
