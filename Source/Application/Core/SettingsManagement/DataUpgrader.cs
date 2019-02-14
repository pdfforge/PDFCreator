using pdfforge.DataStorage;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class DataUpgrader
    {
        public Data Data { get; set; }

        public void MoveValue(string oldPath, string newPath)
        {
            var v = Data.GetValue(oldPath);
            Data.SetValue(newPath, v);
            Data.RemoveValue(oldPath);
        }

        public void MoveValue(string name, string oldSection, string newSection)
        {
            MoveValue(oldSection + name, newSection + name);
        }

        public void MapValue(string path, Func<string, string> mapFunction)
        {
            var v = Data.GetValue(path);
            Data.SetValue(path, mapFunction(v));
        }

        private IEnumerable<string> GetSubSections(string path)
        {
            try
            {
                return Data.GetSubSections(path);
            }
            catch
            {
                return new string[0];
            }
        }

        public void MoveSection(string path, string newPath)
        {
            var keyValuePairs = Data.GetValues(path);

            foreach (var value in keyValuePairs)
            {
                MoveValue(path + value.Key, newPath + value.Key);
            }

            var subSections = GetSubSections(path);
            foreach (var s in subSections)
            {
                var subAddress = s.Remove(0, path.Length);
                var oldSubPath = path + subAddress;
                var newSubPath = newPath + subAddress;

                foreach (var value in Data.GetValues(oldSubPath))
                {
                    MoveValue(oldSubPath + value.Key, newSubPath + value.Key);
                }
            }

            Data.RemoveSection(path.TrimEnd('\\'));
        }
    }
}
