using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public interface IActionOrderHelper
    {
        void ForceDefaultOrder(ConversionProfile conversionWorkflow);

        void ForceDefaultOrder(List<string> actionOrderList);

        void EnsureEncryptionAndSignatureOrder(ConversionProfile conversionProfile);

        bool IsActionInDefaultOrder(string action);

        bool HasCorruptOrder(ConversionProfile conversionWorkflow);
    }

    public class ActionOrderHelper : IActionOrderHelper
    {
        private readonly IEnumerable<string> _defaultOrder;

        public ActionOrderHelper(IEnumerable<string> defaultOrder)
        {
            _defaultOrder = defaultOrder;
        }

        public void ForceDefaultOrder(ConversionProfile conversionWorkflow)
        {
            conversionWorkflow.ActionOrder.Sort(Comparison);
        }

        public void ForceDefaultOrder(List<string> actionOrderList)
        {
            actionOrderList.Sort(Comparison);
        }

        public bool IsActionInDefaultOrder(string action)
        {
            return _defaultOrder.Contains(action);
        }

        public void EnsureEncryptionAndSignatureOrder(ConversionProfile conversionProfile)
        {
            conversionProfile.ActionOrder.Sort(Comparison);
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

        private int Comparison(string x, string y)
        {
            var defaultOrderList = _defaultOrder.ToList();

            var xIndex = defaultOrderList.IndexOf(x);
            var yIndex = defaultOrderList.IndexOf(y);
            return xIndex - yIndex;
        }
    }
}
