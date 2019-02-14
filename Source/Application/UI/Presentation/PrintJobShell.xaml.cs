using MahApps.Metro.Controls;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using Prism.Regions;
using System;
using System.Windows;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class PrintJobShell : MetroWindow, IWhitelisted
    {
        public InteractiveWorkflowManager InteractiveWorkflowManager { get; }

        public PrintJobShell(IRegionManager regionManager, IInteractiveWorkflowManagerFactory interactiveWorkflowManagerFactory, PrintJobShellViewModel viewModel,
            IHightlightColorRegistration hightlightColorRegistration, ICurrentSettingsProvider currentSettingsProvider, ViewCustomization viewCustomization)
        {
            DataContext = viewModel;
            InitializeComponent();
            InteractiveWorkflowManager = interactiveWorkflowManagerFactory.CreateInteractiveWorkflowManager(regionManager, currentSettingsProvider);
            Closing += (sender, args) => InteractiveWorkflowManager.Cancel = true;
            hightlightColorRegistration.RegisterHighlightColorResource(this);

            if (viewCustomization.CustomizationEnabled)
            {
                Title = viewCustomization.PrintJobWindowCaption;
            }
        }

        private async void PrintJobShell_OnLoaded(object sender, RoutedEventArgs e)
        {
            await InteractiveWorkflowManager.Run();
            await Dispatcher.BeginInvoke(new Action(Close));
        }
    }
}
