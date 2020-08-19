using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class ActionOrderChecker : IActionOrderChecker
    {
        private readonly IActionOrderHelper _actionOrderHelper;
        private Type[] _allowedTypes;

        public ActionOrderChecker(IActionOrderHelper actionOrderHelper)
        {
            _actionOrderHelper = actionOrderHelper;

            var iProfileSettingsType = typeof(IProfileSetting);
            var settingsAssembly = iProfileSettingsType.Assembly;
            _allowedTypes = settingsAssembly.GetTypes().Where(x => iProfileSettingsType.IsAssignableFrom(x)).ToArray();
        }

        public void Check(IEnumerable<ConversionProfile> profiles)
        {
            if (profiles == null)
                throw new ArgumentNullException();

            foreach (var settingsConversionProfile in profiles)
            {
                var orderCopy = settingsConversionProfile.ActionOrder.ToList();
                foreach (var entry in orderCopy)
                {
                    var type = _allowedTypes.FirstOrDefault(x => x.Name == entry);
                    if (type == null || !typeof(IProfileSetting).IsAssignableFrom(type))
                    {
                        settingsConversionProfile.ActionOrder.Remove(entry);
                    }
                }

                var list = new List<string>();
                CheckAllChildSettings(list, settingsConversionProfile);
                CheckSingleSetting(list, settingsConversionProfile.PdfSettings.Security);
                CheckSingleSetting(list, settingsConversionProfile.PdfSettings.Signature);
                var hasCorruptOrder = _actionOrderHelper.HasCorruptOrder(settingsConversionProfile);

                if (list.Count != settingsConversionProfile.ActionOrder.Count || hasCorruptOrder)
                {
                    settingsConversionProfile.ActionOrder = list;
                    _actionOrderHelper.ForceDefaultOrder(settingsConversionProfile);
                }
            }
        }

        private void CheckAllChildSettings(List<string> list, object source)
        {
            foreach (var propertyInfo in source.GetType().GetProperties())
            {
                var possibleSettings = propertyInfo.GetValue(source);
                CheckSingleSetting(list, possibleSettings);
            }
        }

        private void CheckSingleSetting(List<string> list, object source)
        {
            if (!(source is IProfileSetting setting) || !_actionOrderHelper.IsActionInDefaultOrder(setting.GetType().Name))
                return;

            if (setting.Enabled)
            {
                list.Add(setting.GetType().Name);
            }
        }
    }

    public interface IActionOrderChecker
    {
        void Check(IEnumerable<ConversionProfile> profiles);
    }
}
