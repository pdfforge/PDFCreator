using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
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
            var pipeMessage = "DragAndDrop";
            if (AppStartParameters.ManagePrintJobs)
                pipeMessage += "+ManagePrintJobs";

            return pipeMessage + "|" + string.Join("|", DroppedFiles);
        }

        protected override bool StartApplication()
        {
            var list = new List<(string path, AppStartParameters paramters)>();
            foreach (var droppedFile in DroppedFiles)
            {
                list.Add((droppedFile, AppStartParameters));
            }
            _fileConversionAssistant.HandleFileList(list);
            return true;
        }
    }
}
