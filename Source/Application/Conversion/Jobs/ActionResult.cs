using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public class ActionResult : List<ErrorCode>
    {
        public ActionResult()
        {
        }

        public ActionResult(ErrorCode errorCode)
        {
            Add(errorCode);
        }

        public bool IsSuccess => Count == 0;

        public static implicit operator bool(ActionResult actionResult)
        {
            return actionResult.IsSuccess;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.Select(x => x.ToString()).ToArray());
        }
    }

    public class ActionResultDict : Dictionary<string, ActionResult>
    {
        public bool Success
        {
            get { return Values.FirstOrDefault(aR => !aR) == null; }
        }

        public static implicit operator bool(ActionResultDict actionResultDict)
        {
            return actionResultDict.Success;
        }

        public void Merge(ActionResultDict actionResultDict)
        {
            foreach (var key in actionResultDict.Keys)
            {
                if (this.ContainsKey(key))
                {
                    foreach (var actionResult in actionResultDict[key])
                    {
                        if (!this[key].Contains(actionResult))
                            this[key].Add(actionResult);
                    }
                }
                else
                    this.Add(key, actionResultDict[key]);
            }
        }
    }
}
