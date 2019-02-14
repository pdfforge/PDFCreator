using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IAppSettingsChecker
    {
        ActionResult CheckDefaultViewers(IEnumerable<DefaultViewer> defaultViewers);
    }

    public class AppSettingsChecker : IAppSettingsChecker
    {
        private readonly IDefaultViewerCheck _defaultViewerCheck;

        public AppSettingsChecker(IDefaultViewerCheck defaultViewerCheck)
        {
            _defaultViewerCheck = defaultViewerCheck;
        }

        public ActionResult CheckDefaultViewers(IEnumerable<DefaultViewer> defaultViewers)
        {
            var result = new ActionResult();

            foreach (var defaultViewer in defaultViewers)
            {
                result.AddRange(_defaultViewerCheck.Check(defaultViewer));
            }

            return result;
        }
    }
}