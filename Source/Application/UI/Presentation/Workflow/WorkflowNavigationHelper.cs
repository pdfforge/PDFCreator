using Prism.Regions;
using System;
using System.Linq;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IWorkflowNavigationHelper
    {
        IWorkflowViewModel GetValidatedViewModel(IRegion region, string navigationUri);

        IWorkflowErrorViewModel GetValidatedErrorViewModel(IRegion region, string navigationUri);
    }

    public class WorkflowNavigationHelper : IWorkflowNavigationHelper
    {
        public IWorkflowViewModel GetValidatedViewModel(IRegion region, string navigationUri)
        {
            var viewModel = GetRawModel(region, navigationUri) as IWorkflowViewModel;

            if (viewModel == null)
                throw new InvalidOperationException($"The ViewModels in the PrintJobShell need to implement {nameof(IWorkflowViewModel)}");

            return viewModel;
        }

        public IWorkflowErrorViewModel GetValidatedErrorViewModel(IRegion region, string navigationUri)
        {
            var viewModel = GetRawModel(region, navigationUri) as IWorkflowErrorViewModel;

            if (viewModel == null)
                throw new InvalidOperationException($"The ErrorViewModels in the PrintJobShell need to implement {nameof(IWorkflowErrorViewModel)}");

            return viewModel;
        }

        private object GetRawModel(IRegion region, string navigationUri)
        {
            var activeView = region.ActiveViews.Cast<UserControl>().First();
            var activeViewName = activeView.GetType().Name;

            if (activeViewName != navigationUri) 
                throw new InvalidOperationException($"It was requested to navigate to view '{navigationUri}', but the active view is '{activeViewName}'");

            return activeView.DataContext;
        }
    }
}
