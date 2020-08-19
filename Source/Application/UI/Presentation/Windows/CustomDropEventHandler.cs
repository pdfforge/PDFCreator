using GongSolutions.Wpf.DragDrop;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class CustomDropEventHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo == null)
            {
                dropInfo.NotHandled = true;
                return;
            }

            base.DragOver(dropInfo);
        }
    }
}
