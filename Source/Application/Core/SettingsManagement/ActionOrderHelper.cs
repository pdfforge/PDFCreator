using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IActionOrderHelper
    {
        void ForceDefaultOrder(ConversionProfile conversionWorkflow);

        void ForceDefaultOrder(List<string> actionOrderList);

        void EnsureEncryptionAndSignatureOrder(ConversionProfile conversionProfile);

        bool IsActionInDefaultOrder(string action);

        bool IsFirstActionBeforeSecond(ConversionProfile conversionProfile, string nameOfFirstActionSetting, string nameOfSecondActionSettings);

        bool HasCorruptOrder(ConversionProfile conversionWorkflow);
    }

    public class ActionOrderHelper : IActionOrderHelper
    {
        private readonly List<string> _defaultOrder;

        public ActionOrderHelper(IEnumerable<string> defaultOrder)
        {
            _defaultOrder = defaultOrder.ToList();
        }

        public void ForceDefaultOrder(ConversionProfile conversionWorkflow)
        {
            conversionWorkflow.ActionOrder.Sort((x, y) => Comparison(x, y, conversionWorkflow));
        }

        public void ForceDefaultOrder(List<string> actionOrderList)
        {
            actionOrderList.Sort(ComparisonWithoutProfile);
        }

        public bool IsActionInDefaultOrder(string action)
        {
            return _defaultOrder.Contains(action);
        }

        public void EnsureEncryptionAndSignatureOrder(ConversionProfile conversionProfile)
        {
            var orderList = conversionProfile.ActionOrder.ToList();
            conversionProfile.ActionOrder.Sort((x, y) =>
            {
                var xIndex = GetSortingIndex(x);
                var yIndex = GetSortingIndex(y);

                return xIndex - yIndex;
            });

            int GetSortingIndex(string elementName)
            {
                int value = orderList.IndexOf(elementName);
                if (elementName == nameof(Signature))
                    value = Int32.MaxValue;
                if (elementName == nameof(Security))
                    value = Int32.MaxValue - 1;
                return value;
            }
        }

        public bool HasCorruptOrder(ConversionProfile conversionWorkflow)
        {
            var orderList = conversionWorkflow.ActionOrder.ToList();
            var orderListCount = orderList.Count;

            if (orderList.Contains(nameof(Signature)))
            {
                if (orderList.IndexOf(nameof(Signature)) != orderListCount - 1)
                    return true;

                if (orderList.IndexOf(nameof(Security)) != orderListCount - 2)
                    return true;
            }
            else
            {
                if (orderList.Contains(nameof(Security)) && orderList.IndexOf(nameof(Security)) != orderListCount - 1)
                    return true;
            }

            return false;
        }

        private int ComparisonWithoutProfile(string x, string y)
        {
            var xIndex = _defaultOrder.IndexOf(x);
            var yIndex = _defaultOrder.IndexOf(y);
            return xIndex - yIndex;
        }

        private int Comparison(string x, string y, ConversionProfile profile)
        {
            var xIndex = _defaultOrder.IndexOf(x);
            var yIndex = _defaultOrder.IndexOf(y);

            return xIndex - yIndex;
        }

        public bool IsFirstActionBeforeSecond(ConversionProfile conversionProfile, string nameOfFirstActionSetting, string nameOfSecondActionSettings)
        {
            var firstIndex = conversionProfile.ActionOrder.IndexOf(nameOfFirstActionSetting);
            if (firstIndex < 0)
                return false;
            var secondIndex = conversionProfile.ActionOrder.IndexOf(nameOfSecondActionSettings);
            return firstIndex < secondIndex;
        }
    }
}
