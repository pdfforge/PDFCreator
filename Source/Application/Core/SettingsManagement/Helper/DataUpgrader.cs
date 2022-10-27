using System;
using System.Collections.Generic;
using pdfforge.DataStorage;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public class DataUpgrader
    {
        public Data Data { get; set; }

        protected void MoveValue(string oldPath, string newPath)
        {
            var value = Data.GetValue(oldPath);
            Data.SetValue(newPath, value);
            Data.RemoveValue(oldPath);
        }

        protected void MapValue(string path, Func<string, string> mapFunction)
        {
            var value = Data.GetValue(path);
            var newValue = mapFunction(value);
            Data.SetValue(path, newValue);
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
