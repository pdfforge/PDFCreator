using GongSolutions.Wpf.DragDrop;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class WorkflowEditorActionDropTargetHandler<T> : DefaultDropHandler where T : IAction

    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (CheckDragInfo(dropInfo)) return;
            base.DragOver(dropInfo);
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (CheckDragInfo(dropInfo)) return;
            base.Drop(dropInfo);
        }

        private bool CheckDragInfo(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo == null)
            {
                dropInfo.NotHandled = true;
                return true;
            }

            if (dropInfo.DragInfo.Data is IPresenterActionFacade actionFacade)
            {
                if (dropInfo.InsertIndex != 0)
                {
                    var presenterActionFacades = dropInfo.TargetCollection.Cast<IPresenterActionFacade>().ToArray();
                    var prevItem = presenterActionFacades.ElementAt(dropInfo.InsertIndex - 1);
                    if (typeof(IFixedOrderAction).IsAssignableFrom(prevItem.SettingsType))
                    {
                        dropInfo.NotHandled = true;
                        return true;
                    }
                }

                if (!typeof(T).IsAssignableFrom(actionFacade.Action))
                {
                    dropInfo.NotHandled = true;
                    return true;
                }
            }
            return false;
        }
    }
}
