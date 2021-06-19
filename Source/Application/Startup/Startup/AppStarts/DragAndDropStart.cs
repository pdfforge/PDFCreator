using Newtonsoft.Json;
using pdfforge.PDFCreator.Core.Controller;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class DragAndDropStart : MaybePipedStart
    {
        private readonly IFileConversionAssistant _fileConversionAssistant;

        public DragAndDropStart(IFileConversionAssistant fileConversionAssistant, IMaybePipedApplicationStarter maybePipedApplicationStarter)
            : base(maybePipedApplicationStarter)
        {
            _fileConversionAssistant = fileConversionAssistant;
        }

        public IList<string> DroppedFiles { get; set; } = new List<string>();

        protected override string ComposePipeMessage()
        {
            var parameterJson = JsonConvert.SerializeObject(AppStartParameters);

            return "DragAndDrop|" + parameterJson + "|" + string.Join("|", DroppedFiles);
        }

        protected override bool StartApplication()
        {
            _fileConversionAssistant.HandleFileList(DroppedFiles, AppStartParameters);
            return true;
        }
    }
}
