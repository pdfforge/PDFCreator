using System;
using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
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

                var enabledActionsList = GetListOfActiveActions(settingsConversionProfile);

                // get an action ordered list with only distinct and enabled actions
                var actionOrderList = settingsConversionProfile
                    .ActionOrder
                    .Distinct()
                    .Where(s => enabledActionsList.Contains(s))
                    .ToList();

                AddMissingActiveActions(actionOrderList, enabledActionsList);
                UpdateActionOrderList(settingsConversionProfile, actionOrderList);

                if (_actionOrderHelper.HasCorruptOrder(settingsConversionProfile))
                    _actionOrderHelper.EnsureEncryptionAndSignatureOrder(settingsConversionProfile);
            }
        }

        private void UpdateActionOrderList(ConversionProfile profile, IEnumerable<string> newOrderList)
        {
            profile.ActionOrder.Clear();
            profile.ActionOrder.AddRange(newOrderList);
        }

        private void AddMissingActiveActions(List<string> targetActionOrderList, List<string> enabledActionsList)
        {
            if (enabledActionsList.Count == targetActionOrderList.Count)
                return;

            var missingActions = enabledActionsList.Except(targetActionOrderList).ToList();
            _actionOrderHelper.ForceDefaultOrder(missingActions);
            targetActionOrderList.AddRange(missingActions);
        }

        private List<string> GetListOfActiveActions(ConversionProfile settingsConversionProfile)
        {
            var enabledActionsList = new List<string>();
            CheckAllChildSettings(enabledActionsList, settingsConversionProfile);
            CheckSingleSetting(enabledActionsList, settingsConversionProfile.PdfSettings.Security);
            CheckSingleSetting(enabledActionsList, settingsConversionProfile.PdfSettings.Signature);
            return enabledActionsList;
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
