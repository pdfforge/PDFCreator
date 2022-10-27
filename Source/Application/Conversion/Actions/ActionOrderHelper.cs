using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class ActionOrderHelper : IActionOrderHelper
    {
        private readonly IEnumerable<IAction> _allActionsInDefaultOrder;
        private List<string> _allActionSettingNamesInDefaultOrder;
        private List<string> _preConversionSettingNamesInDefaultOrder;
        private List<string> _conversionActionSettingNamesInDefaultOrder;
        private List<string> _postConversionSettingNamesInDefaultOrder;

        public ActionOrderHelper(IEnumerable<IAction> allActionsInDefaultOrder)
        {
            //Init later to avoid System.InvalidOperationException
            //because of a LifeStyleMismatch when resolving injected collection during object construction.
            _allActionsInDefaultOrder = allActionsInDefaultOrder;
        }

        private void Init()
        {
            if (_allActionSettingNamesInDefaultOrder != null)
                return;

            _allActionSettingNamesInDefaultOrder = _allActionsInDefaultOrder
                .Select(x => x.SettingsType.Name)
                .ToList();

            _preConversionSettingNamesInDefaultOrder = _allActionsInDefaultOrder
                .OfType<IPreConversionAction>()
                .Select(x => x.SettingsType.Name)
                .ToList();

            _conversionActionSettingNamesInDefaultOrder = _allActionsInDefaultOrder
                .OfType<IConversionAction>()
                .Select(x => x.SettingsType.Name)
                .ToList();

            _postConversionSettingNamesInDefaultOrder = _allActionsInDefaultOrder
                .OfType<IPostConversionAction>()
                .Select(x => x.SettingsType.Name)
                .ToList();
        }

        public void CleanUpAndEnsureValidOrder(IEnumerable<ConversionProfile> profiles)
        {
            if (profiles == null)
                throw new ArgumentNullException();

            foreach (var profile in profiles)
            {
                var enabledActionsList = GetEnabledActions(profile);

                // get a distinct ordered list with only known and enabled actions
                var actionOrderList = profile.ActionOrder
                    .Distinct()
                    .Where(s => enabledActionsList.Contains(s))
                    .ToList();

                var missingEnabledActions = enabledActionsList.Except(actionOrderList).ToList();
                actionOrderList.AddRange(missingEnabledActions);

                EnsureValidOrder(actionOrderList);

                profile.ActionOrder.Clear();
                profile.ActionOrder.AddRange(actionOrderList);
            }
        }

        private int Comparison(string x, string y)
        {
            var xIndex = _allActionSettingNamesInDefaultOrder.IndexOf(x);
            var yIndex = _allActionSettingNamesInDefaultOrder.IndexOf(y);
            return xIndex - yIndex;
        }

        public void EnsureValidOrder(List<string> currentActionOrderList)
        {
            Init();

            //always sort whole PreConversionActions because there is just one reasonable order
            var preConversionActions = currentActionOrderList
                .Where(x => _preConversionSettingNamesInDefaultOrder.Contains(x))
                .ToList();
            preConversionActions.Sort(Comparison);

            var modifyActions = currentActionOrderList
                .Where(x => _conversionActionSettingNamesInDefaultOrder.Contains(x))
                .ToList();
            //Move Security to end of list
            if (modifyActions.Contains(nameof(Security)))
            {
                modifyActions.Remove(nameof(Security));
                modifyActions.Add(nameof(Security));
            }
            //Move Signature to end of list after Security
            if (modifyActions.Contains(nameof(Signature)))
            {
                modifyActions.Remove(nameof(Signature));
                modifyActions.Add(nameof(Signature));
            }

            //Leave SendActions unsorted
            var sendActions = currentActionOrderList
                .Where(x => _postConversionSettingNamesInDefaultOrder.Contains(x))
                .ToList();

            currentActionOrderList.Clear();
            currentActionOrderList.AddRange(preConversionActions.Concat(modifyActions).Concat(sendActions).Distinct());
        }

        public IEnumerable<string> GetEnabledActions(ConversionProfile profile)
        {
            return _allActionsInDefaultOrder.Where(x => x.IsEnabled(profile))
                .Select(x => x.SettingsType.Name)
                .ToList();
        }
    }
}
