using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class Release
    {
        public Version Version { get; set; }
        public bool IsStable { get; set; }
        public string ReleaseDate { get; set; }
        public List<ChangeLogItem> Changes { get; set; }
        public List<Download> Downloads { get; set; }

        public Release Copy()
        {
            var returnVal = new Release
            {
                Version = Version,
                IsStable = IsStable,
                ReleaseDate = ReleaseDate,
                Changes = new List<ChangeLogItem>(),
                Downloads = new List<Download>()
            };

            if (Changes != null)
            {
                foreach (var updateChange in Changes)
                {
                    returnVal.Changes.Add(updateChange.Copy());
                }
            }

            if (Downloads != null)
            {
                foreach (var download in Downloads)
                {
                    returnVal.Downloads.Add(download.Copy());
                }
            }

            return returnVal;
        }

        public List<ChangeLogItem> Bugfixes
        {
            get
            {
                if (Changes == null)
                    return new List<ChangeLogItem>();

                return Changes.FindAll(change => change.Type == UpdateChangeType.Bugfix.ToString());
            }
        }

        public List<ChangeLogItem> Features
        {
            get
            {
                if (Changes == null)
                    return new List<ChangeLogItem>();

                return Changes.FindAll(change => change.Type == UpdateChangeType.Feature.ToString());
            }
        }

        public List<ChangeLogItem> Tasks
        {
            get
            {
                if (Changes == null)
                    return new List<ChangeLogItem>();

                return Changes.FindAll(change => change.Type == UpdateChangeType.Other.ToString());
            }
        }
    }

    public enum UpdateChangeType
    {
        Feature,
        Bugfix,
        Other
    }

    public class ChangeLogItem
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public int Priority { get; set; }
        public string BulletPoint => "- " + Text;

        public ChangeLogItem Copy()
        {
            var returnVal = new ChangeLogItem
            {
                Type = Type,
                Text = Text,
                Priority = Priority
            };
            return returnVal;
        }
    }

    public class Download
    {
        public string Path { get; set; }
        public string Filename { get; set; }
        public string SourceUrl { get; set; }
        public long Size { get; set; }
        public string Md5 { get; set; }
        public string PersistentName { get; set; }

        public Download Copy()
        {
            var returnVal = new Download
            {
                Path = Path,
                Filename = Filename,
                SourceUrl = SourceUrl,
                Size = Size,
                Md5 = Md5,
                PersistentName = PersistentName
            };
            return returnVal;
        }
    }
}
